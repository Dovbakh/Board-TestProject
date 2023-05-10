using AutoMapper;
using Board.Application.AppData.Contexts.Comments.Repositories;
using Board.Application.AppData.Contexts.Images.Services;
using Board.Application.AppData.Contexts.Notifications.Services;
using Board.Contracts.Contexts.Comments;
using Board.Contracts.Contexts.Users;
using FluentValidation;
using Identity.Clients.Users;
using Identity.Contracts.Clients.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
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
    /// <inheritdoc />
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;
        private readonly IUserClient _userClient;
        private readonly IValidator<UserLoginRequest> _userLoginValidator;
        private readonly IValidator<UserRegisterRequest> _userRegisterValidator;
        private readonly IValidator<UserUpdateRequest> _userUpdateValidator;
        private readonly IValidator<UserChangeEmailRequest> _userChangeEmailValidator;
        private readonly IValidator<UserConfirmEmailRequest> _userConfirmEmailValidator;
        private readonly IValidator<UserGenerateEmailTokenRequest> _userGenerateEmailTokenValidator;
        private readonly IValidator<UserGenerateEmailConfirmationTokenRequest> _userGenerateEmailConfirmationTokenValidator;
        private readonly IValidator<UserEmail> _userEmailValidator;
        private readonly INotificationService _notificationService;
        private readonly IDistributedLockFactory _distributedLockFactory;
        private readonly ICommentRepository _commentRepository;
        private readonly ILogger<UserService> _logger;
        private readonly IImageService _imageService;

        public UserService(IConfiguration configuration, IUserClient boardClient, IMapper mapper, IHttpContextAccessor contextAccessor,
            IValidator<UserLoginRequest> userLoginValidator, IValidator<UserRegisterRequest> userRegisterValidator, IValidator<UserUpdateRequest> userUpdateValidator,
            INotificationService notificationService, IDistributedLockFactory distributedLockFactory, ICommentRepository commentRepository, ILogger<UserService> logger,
            IValidator<UserChangeEmailRequest> userChangeEmailValidator, IValidator<UserConfirmEmailRequest> userConfirmEmailValidator,
            IValidator<UserGenerateEmailTokenRequest> userGenerateEmailTokenValidator, IValidator<UserGenerateEmailConfirmationTokenRequest> userGenerateEmailConfirmationTokenValidator,
            IValidator<UserEmail> userEmailValidator, IImageService imageService)
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
            _logger = logger;
            _userChangeEmailValidator = userChangeEmailValidator;
            _userConfirmEmailValidator = userConfirmEmailValidator;
            _userGenerateEmailTokenValidator = userGenerateEmailTokenValidator;
            _userGenerateEmailConfirmationTokenValidator = userGenerateEmailConfirmationTokenValidator;
            _userEmailValidator = userEmailValidator;
            _imageService = imageService;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<UserSummary>> GetAllAsync(int? offset, int? count, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{0} -> Получение всех пользователей.",
                nameof(GetAllAsync));

            var clientResponse = await _userClient.GetAllAsync(offset.GetValueOrDefault(), count.GetValueOrDefault(), cancellationToken);
            var users = _mapper.Map<IReadOnlyCollection<UserSummaryClientResponse>, IReadOnlyCollection<UserSummary>>(clientResponse);
            
            return users;
        }

        /// <inheritdoc />
        public async Task<UserDetails> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{0} -> Получение пользователя с ID: {1}",
                nameof(GetByIdAsync), id);

            var clientResponse = await _userClient.GetByIdAsync(id, cancellationToken);
            var user = _mapper.Map<UserDetailsClientResponse, UserDetails>(clientResponse);
            if(user == null)
            {
                throw new KeyNotFoundException($"Не найден пользователь с ID: {id}");
            }

            var rating = await _commentRepository.GetAverageRating(id, cancellationToken);
            user.Rating = rating;

            return user;
        }

        /// <inheritdoc />
        public async Task<UserDetails> GetCurrentAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{0} -> Получение текущего пользователя.",
                nameof(GetCurrentAsync));

            var claims = _contextAccessor.HttpContext.User.Claims;
            var claimId = claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            var userId = Guid.Parse(claimId);

            var clientResponse = await _userClient.GetByIdAsync(userId, cancellationToken);
            var user = _mapper.Map<UserDetailsClientResponse, UserDetails>(clientResponse);

            return user;
        }

        /// <inheritdoc />
        public Guid GetCurrentId(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{0} -> Получение ID текущего пользователя.",
                nameof(GetCurrentId));

            var claims = _contextAccessor.HttpContext.User.Claims;
            var claimId = claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            var userId = Guid.Parse(claimId);

            return userId;
        }

        /// <inheritdoc />
        public async Task<string> LoginAsync(UserLoginRequest loginRequest, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{0} -> Аутентификация пользователя с email: {1}",
                nameof(LoginAsync), loginRequest.Email);

            var validationResult = _userLoginValidator.Validate(loginRequest);
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Модель аутентификации пользователя не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }

            var loginClientRequest = _mapper.Map<UserLoginRequest, UserLoginClientRequest>(loginRequest);
            var tokenResponse = await _userClient.LoginAsync(loginClientRequest, cancellationToken);

            if(tokenResponse.IsError)
            {
                throw new ArgumentException($"Ошибка при получении токена: {tokenResponse.ErrorDescription}");
            }

            return tokenResponse.AccessToken;
        }

        /// <inheritdoc />
        public async Task<Guid> RegisterAsync(UserRegisterRequest registerRequest, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{0} -> Регистрация пользователя со следующим email: {1}",
                nameof(RegisterAsync), registerRequest.Email);

            var validationResult = _userRegisterValidator.Validate(registerRequest);
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Модель регистрации нового пользователя не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }

            var registerClientRequest = _mapper.Map<UserRegisterRequest, UserRegisterClientRequest>(registerRequest);          
            var userId = await _userClient.RegisterAsync(registerClientRequest, cancellationToken);


            return userId;
        }

        /// <inheritdoc />
        public async Task<UserDetails> UpdateAsync(Guid id, UserUpdateRequest updateRequest, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{0} -> Обновление информации о пользователе с ID: {1} со следующей моделью обновления {2}: {3}",
                nameof(UpdateAsync), id, nameof(UserUpdateRequest), JsonConvert.SerializeObject(updateRequest));

            var validationResult = _userUpdateValidator.Validate(updateRequest);
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Модель обновления пользователя не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }

            if (updateRequest.PhotoId.HasValue)
            {
                var isPhotoExists = await _imageService.IsImageExists(updateRequest.PhotoId.Value, cancellationToken);
                if (!isPhotoExists)
                {
                    throw new KeyNotFoundException($"На файлом сервисе не найдено изображение с ID: {updateRequest.PhotoId}");
                }
            }

            var updateClientRequest = _mapper.Map<UserUpdateRequest, UserUpdateClientRequest>(updateRequest);
            var clientResponse = await _userClient.UpdateAsync(id, updateClientRequest, cancellationToken);
            var updatedUser = _mapper.Map<UserDetailsClientResponse, UserDetails>(clientResponse);

            return updatedUser;
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{0} -> Удаление пользователя с ID: {1}",
                nameof(DeleteAsync), id);

            await _userClient.DeleteAsync(id, cancellationToken);
        }

        /// <inheritdoc />
        public async Task SendEmailTokenAsync(UserGenerateEmailTokenRequest request, string changeLink, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{0} -> Генерация токена смены email из модели {1} и отправка его на новый email.",
                nameof(SendEmailTokenAsync), JsonConvert.SerializeObject(request));

            var validationResult = _userGenerateEmailTokenValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Модель генерации токена для изменения почты не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }

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

        /// <inheritdoc />
        public async Task SendEmailConfirmationTokenAsync(UserGenerateEmailConfirmationTokenRequest request, string confirmLink, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{0} -> Генерация токена подтверждения email из модели {1} и отправка его на email.",
                nameof(SendEmailConfirmationTokenAsync), JsonConvert.SerializeObject(request));

            var validationResult = _userGenerateEmailConfirmationTokenValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Модель генерации токена для подтверждения почты не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }

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
            _logger.LogInformation("{0} -> Изменение email текущего пользователя с ID: {1} на {2}",
                nameof(ChangeEmailAsync), GetCurrentId(cancellationToken), newEmail);

            var validationResult = _userEmailValidator.Validate(new UserEmail { Value = newEmail});
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Модель изменения почты не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }

            var currentUser = await GetCurrentAsync(cancellationToken);

            var clientRequest = new UserChangeEmailClientRequest { CurrentEmail = currentUser.Email, NewEmail = newEmail, Token = token };
            await _userClient.ChangeEmailAsync(clientRequest, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task ConfirmEmailAsync(string email, string token, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{0} -> Подтверждение email: {1} текущего пользователя с ID: {2}",
                nameof(ChangeEmailAsync), email, GetCurrentId(cancellationToken));

            var validationResult = _userEmailValidator.Validate(new UserEmail { Value = email });
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Модель подтверждения почты не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }

            var currentUser = await GetCurrentAsync(cancellationToken);

            var clientRequest = new UserEmailConfirmClientRequest { Email = email, Token = token };
            await _userClient.ConfirmEmailAsync(clientRequest, cancellationToken);
        }

    }
}
