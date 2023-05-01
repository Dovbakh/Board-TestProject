using AutoMapper;
using Board.Application.AppData.Contexts.Comments.Repositories;
using Board.Application.AppData.Contexts.Notifications.Services;
using Board.Application.AppData.Contexts.Users.Repositories;
using Board.Contracts.Contexts.Comments;
using Board.Contracts.Contexts.Users;
using FluentValidation;
using Identity.Clients.Users;
using Identity.Contracts.Clients.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Board.Application.AppData.Contexts.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;
        private readonly IUserClient _userClient;
        private readonly IValidator<UserLoginRequest> _userLoginValidator;
        private readonly IValidator<UserRegisterRequest> _userRegisterValidator;
        private readonly IValidator<UserUpdateRequest> _userUpdateValidator;
        private readonly INotificationService _notificationService;
        private readonly IDistributedLockFactory _distributedLockFactory;
        private readonly ICommentRepository _commentRepository;

        public UserService(IConfiguration configuration, IUserClient boardClient, IMapper mapper, IHttpContextAccessor contextAccessor,
            IValidator<UserLoginRequest> userLoginValidator, IValidator<UserRegisterRequest> userRegisterValidator, IValidator<UserUpdateRequest> userUpdateValidator,
            INotificationService notificationService, IDistributedLockFactory distributedLockFactory, ICommentRepository commentRepository)
        {
            _configuration = configuration;
            _userClient = boardClient;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _userLoginValidator = userLoginValidator;
            _userRegisterValidator = userRegisterValidator;
            _userUpdateValidator = userUpdateValidator;
            _notificationService = notificationService;
            _distributedLockFactory = distributedLockFactory;
            _commentRepository = commentRepository;
        }

        public async Task<IReadOnlyCollection<UserSummary>> GetAll(int? offset, int? count, CancellationToken cancellationToken)
        {           
            var clientResponse = await _userClient.GetAll(offset.GetValueOrDefault(), count.GetValueOrDefault(), cancellationToken);
            var users = _mapper.Map<IReadOnlyCollection<UserSummaryClientResponse>, IReadOnlyCollection<UserSummary>>(clientResponse);
            
            return users;
        }

        public async Task<UserDetails> GetById(Guid id, CancellationToken cancellationToken)
        {          
            var clientResponse = await _userClient.GetById(id, cancellationToken);
            var user = _mapper.Map<UserDetailsClientResponse, UserDetails>(clientResponse);

            var rating = await _commentRepository.GetAverageRating(id, cancellationToken);
            user.Rating = rating;

            return user;
        }

        public async Task<UserDetails> GetCurrent(CancellationToken cancellationToken)
        {
            var claims = _contextAccessor.HttpContext.User.Claims;
            var claimId = claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            var userId = Guid.Parse(claimId);

            var clientResponse = await _userClient.GetById(userId, cancellationToken);
            var user = _mapper.Map<UserDetailsClientResponse, UserDetails>(clientResponse);

            return user;
        }

        public async Task<Guid> GetCurrentId(CancellationToken cancellationToken)
        {
            var claims = _contextAccessor.HttpContext.User.Claims;
            var claimId = claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            var userId = Guid.Parse(claimId);

            return userId;
        }

        public async Task<string> Login(UserLoginRequest loginRequest, CancellationToken cancellationToken)
        {
            var validationResult = _userLoginValidator.Validate(loginRequest);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException(validationResult.ToString("~"));
            }

            var loginClientRequest = _mapper.Map<UserLoginRequest, UserLoginClientRequest>(loginRequest);
            var tokenResponse = await _userClient.Login(loginClientRequest, cancellationToken);

            if(tokenResponse.IsError)
            {
                throw new ArgumentException(tokenResponse.ErrorDescription);
            }

            return tokenResponse.AccessToken;
        }

        public async Task<Guid> Register(UserRegisterRequest registerRequest, CancellationToken cancellationToken)
        {
            var validationResult = _userRegisterValidator.Validate(registerRequest);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException(validationResult.ToString("~"));
            }

            var registerClientRequest = _mapper.Map<UserRegisterRequest, UserRegisterClientRequest>(registerRequest);
            var userId = Guid.Empty;


            var resource = "RegisterEmailKey_" + registerRequest.Email;
            var expiry = TimeSpan.FromSeconds(30);
            var wait = TimeSpan.FromSeconds(10);
            var retry = TimeSpan.FromSeconds(1);
            await using (var redLock = await _distributedLockFactory.CreateLockAsync(resource, expiry, wait, retry, cancellationToken))
            {
                if (redLock.IsAcquired)
                {
                    userId = await _userClient.Register(registerClientRequest, cancellationToken);
                }
            }

            if (userId == Guid.Empty) 
            {
                throw new ArgumentException();
            }
         
            return userId;
        }

        public async Task<UserDetails> UpdateAsync(Guid id, UserUpdateRequest updateRequest, CancellationToken cancellationToken)
        {
            var validationResult = _userUpdateValidator.Validate(updateRequest);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException(validationResult.ToString("~"));
            }

            var updateClientRequest = _mapper.Map<UserUpdateRequest, UserUpdateClientRequest>(updateRequest);
            var clientResponse = await _userClient.Update(id, updateClientRequest, cancellationToken);
            var updatedUser = _mapper.Map<UserDetailsClientResponse, UserDetails>(clientResponse);

            return updatedUser;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            await _userClient.Delete(id, cancellationToken);
        }

        public async Task SendEmailTokenAsync(UserGenerateEmailTokenRequest request, string changeLink, CancellationToken cancellationToken)
        {
            var clientRequest = _mapper.Map<UserGenerateEmailTokenRequest, UserGenerateEmailTokenClientRequest>(request);
            var token = await _userClient.GenerateEmailTokenAsync(clientRequest, cancellationToken);          
            

            string messageWarning = $"Поступил запрос на изменение электронной почты на <b>{request.NewEmail}</b>. " +
                                    $"Если это были вы, то ничего делать не нужно. В противном случае напишите в поддержку.";
            string subjectWarning = "Изменение электронной почты";
            await _notificationService.SendMessage(request.CurrentEmail, subjectWarning, messageWarning);

            changeLink = changeLink.Replace("tokenValue", token);
            string messageConfirm = $"Для изменения почты перейдите по <a href='{changeLink}'>ссылке</a>";
            string subjectConfirm = "Изменение электронной почты";
            await _notificationService.SendMessage(request.NewEmail, subjectConfirm, messageConfirm);
        }

        public async Task SendEmailConfirmationTokenAsync(UserGenerateEmailConfirmationTokenRequest request, string confirmLink, CancellationToken cancellationToken)
        {
            var clientRequest = _mapper.Map<UserGenerateEmailConfirmationTokenRequest, UserGenerateEmailConfirmationTokenClientRequest>(request);
            var token = await _userClient.GenerateEmailConfirmationTokenAsync(clientRequest, cancellationToken);


            confirmLink = confirmLink.Replace("tokenValue", token);
            string messageConfirm = $"Для подтверждения почты перейдите по <a href='{confirmLink}'>ссылке</a>";
            string subjectConfirm = "Подтверждение электронной почты";
            await _notificationService.SendMessage(request.Email, subjectConfirm, messageConfirm);
        }

        /// <inheritdoc/>
        public async Task ChangeEmailAsync(string newEmail, string token, CancellationToken cancellationToken)
        {
            //var validationResult = _validatorEmail.Validate(newEmail);
            //if (!validationResult.IsValid)
            //{
            //    throw new Exception(validationResult.ToString("~"));
            //}


            var currentUser = await GetCurrent(cancellationToken);

            var clientRequest = new UserChangeEmailClientRequest { CurrentEmail = currentUser.Email, NewEmail = newEmail, Token = token };

            await _userClient.ChangeEmailAsync(clientRequest, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task ConfirmEmailAsync(string email, string token, CancellationToken cancellationToken)
        {
            //var validationResult = _validatorEmail.Validate(newEmail);
            //if (!validationResult.IsValid)
            //{
            //    throw new Exception(validationResult.ToString("~"));
            //}


            var currentUser = await GetCurrent(cancellationToken);

            var clientRequest = new UserEmailConfirmClientRequest { Email = currentUser.Email, Token = token };

            await _userClient.ConfirmEmailAsync(clientRequest, cancellationToken);
        }

    }
}
