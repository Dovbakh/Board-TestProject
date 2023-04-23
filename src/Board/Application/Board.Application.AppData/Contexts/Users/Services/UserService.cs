using AutoMapper;
using Board.Application.AppData.Contexts.Users.Repositories;
using Board.Contracts.Contexts.Users;
using FluentValidation;
using Identity.Clients.Users;
using Identity.Contracts.Clients.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;
        private readonly IUserClient _userClient;
        private readonly IValidator<UserLoginRequest> _userLoginValidator;
        private readonly IValidator<UserRegisterRequest> _userRegisterValidator;
        private readonly IValidator<UserUpdateRequest> _userUpdateValidator;

        public UserService(IUserRepository userRepository, IConfiguration configuration, IUserClient boardClient, IMapper mapper, IHttpContextAccessor contextAccessor, 
            IValidator<UserLoginRequest> userLoginValidator, IValidator<UserRegisterRequest> userRegisterValidator, IValidator<UserUpdateRequest> userUpdateValidator)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _userClient = boardClient;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _userLoginValidator = userLoginValidator;
            _userRegisterValidator = userRegisterValidator;
            _userUpdateValidator = userUpdateValidator;
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
            var userId = await _userClient.Register(registerClientRequest, cancellationToken);

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
    }
}
