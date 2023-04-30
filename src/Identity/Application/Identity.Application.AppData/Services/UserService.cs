using FluentValidation;
using Identity.Application.AppData.Helpers;
using Identity.Application.AppData.Repositories;
using Identity.Contracts.Contexts.Users;
using IdentityServer4;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.AppData.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IdentityServerTools _identityServerTools;
        private readonly IValidator<UserLoginRequest> _userLoginValidator;
        private readonly IValidator<UserRegisterRequest> _userRegisterValidator;
        private readonly IValidator<UserUpdateRequest> _userUpdateValidator;
        private readonly IValidator<UserGenerateEmailTokenRequest> _userGenerateEmailTokenValidator;
        private readonly IValidator<UserGenerateEmailConfirmationTokenRequest> _userGenerateEmailConfirmationTokenValidator;

        public UserService(IUserRepository userRepository, IConfiguration configuration, IHttpContextAccessor contextAccessor, IdentityServerTools identityServerTools,
            IValidator<UserLoginRequest> userLoginValidator, IValidator<UserRegisterRequest> userRegisterValidator, IValidator<UserUpdateRequest> userUpdateValidator,
            IValidator<UserGenerateEmailTokenRequest> userGenerateEmailTokenValidator, IValidator<UserGenerateEmailConfirmationTokenRequest> userGenerateEmailConfirmationTokenValidator)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _contextAccessor = contextAccessor;
            _identityServerTools = identityServerTools;
            _userLoginValidator = userLoginValidator;
            _userRegisterValidator = userRegisterValidator;
            _userUpdateValidator = userUpdateValidator;
            _userGenerateEmailTokenValidator = userGenerateEmailTokenValidator;
            _userGenerateEmailConfirmationTokenValidator = userGenerateEmailConfirmationTokenValidator;
        }

        public Task<IReadOnlyCollection<UserSummary>> GetAll(int? offset, int? count, CancellationToken cancellationToken)
        {
            if (count == 0)
            {
                count = 10;
            }
            return _userRepository.GetAll(offset.GetValueOrDefault(), count.GetValueOrDefault(), cancellationToken);
        }

        public Task<UserDetails> GetById(Guid id, CancellationToken cancellationToken)
        {
            return _userRepository.GetById(id, cancellationToken);
        }

        public async Task<UserDetails> GetCurrent(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            //_logger.LogInformation("Получение данных о текущем пользователе и их обновление.");
            //var claims1 = await _claimsAccessor.GetClaims(cancellationToken);

            //var claimId1 = claims1.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            //var claims = _contextAccessor.HttpContext.User.Claims;
            //var claimId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            //if (string.IsNullOrWhiteSpace(claimId))
            //{
            //    return null;
            //}
            //var id = Guid.Parse(claimId);

            //var user = await _userRepository.GetById(id, cancellationToken);

            //return user;

        }
        public async Task<Guid> Register(UserRegisterRequest registerRequest, CancellationToken cancellationToken)
        {
            var validationResult = _userRegisterValidator.Validate(registerRequest);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException(validationResult.ToString("~"));
            }

            var existingUser = await _userRepository.GetByEmail(registerRequest.Email, cancellationToken);
            if (existingUser != null)
            {
                throw new ArgumentException($"Пользователь с почтой '{registerRequest.Email}' уже зарегистрирован!");
            }


            return await _userRepository.AddAsync(registerRequest, cancellationToken);
        }

        public async Task<string> Login(UserLoginRequest loginRequest, CancellationToken cancellationToken)
        {
            var validationResult = _userLoginValidator.Validate(loginRequest);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException(validationResult.ToString("~"));
            }

            // TODO: сделать ручную генерацию токена для хэндлинга ошибок _identityServerTools

            var existingUser = await _userRepository.GetByEmail(loginRequest.Email, cancellationToken);
            if (existingUser == null)
            {
                throw new ArgumentException("Введен неверный email или пароль.");
            }

            var isPasswordVerified = await _userRepository.CheckPasswordAsync(loginRequest.Email, loginRequest.Password, cancellationToken);
            if (!isPasswordVerified)
            {
                throw new ArgumentException("Введен неверный email или пароль.");
            }

            var token = "";// GenerateToken(existingUser);


            return token;
        }



        public Task<UserDetails> UpdateAsync(Guid id, UserUpdateRequest updateRequest, CancellationToken cancellationToken)
        {
            var validationResult = _userUpdateValidator.Validate(updateRequest);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException(validationResult.ToString("~"));
            }

            return _userRepository.UpdateAsync(id, updateRequest, cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            //var currentUser = await GetCurrent(cancellationToken);
            //;
            //if (currentUser.Id != id && currentUser.RoleName != "admin")
            //{
            //    throw new AccessViolationException("Нет прав.");
            //}

            await _userRepository.DeleteAsync(id, cancellationToken);
        }

        public Task<bool> IsInRoleRole(Guid userId, string role, CancellationToken cancellationToken)
        {
            return _userRepository.IsInRoleRole(userId, role, cancellationToken);
        }

        public async Task<EmailChangeToken> GenerateEmailTokenAsync(UserGenerateEmailTokenRequest request, CancellationToken cancellationToken)
        {
            var validationResult = _userGenerateEmailTokenValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                throw new Exception(validationResult.ToString("~"));
            }

            var isPasswordVerified = await _userRepository.CheckPasswordAsync(request.CurrentEmail, request.Password, cancellationToken);
            if (!isPasswordVerified)
            {
                throw new ArgumentException("Введен неверный пароль.");
            }

            var token = await _userRepository.GenerateEmailTokenAsync(request.CurrentEmail, request.NewEmail, cancellationToken);

            return token;
        }

        public async Task<EmailConfirmationToken> GenerateEmailConfirmationTokenAsync(UserGenerateEmailConfirmationTokenRequest request, CancellationToken cancellationToken)
        {
            var validationResult = _userGenerateEmailConfirmationTokenValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                throw new Exception(validationResult.ToString("~"));
            }

            var token = await _userRepository.GenerateEmailConfirmationTokenAsync(request.Email, cancellationToken);

            return token;
        }
       

        public async Task ChangeEmailAsync(UserChangeEmailRequest request, CancellationToken cancellationToken)
        {
            //var validationResult = _validatorEmail.Validate(newEmail);
            //if (!validationResult.IsValid)
            //{
            //    throw new Exception(validationResult.ToString("~"));
            //}
            request.Token = request.Token.Replace(" ", "+");
            await _userRepository.ChangeEmailAsync(request.CurrentEmail, request.NewEmail, request.Token, cancellationToken);

        }

        public async Task ConfirmEmailAsync(UserEmailConfirmRequest request, CancellationToken cancellationToken)
        {
            //var validationResult = _validatorEmail.Validate(newEmail);
            //if (!validationResult.IsValid)
            //{
            //    throw new Exception(validationResult.ToString("~"));
            //}
            request.Token = request.Token.Replace(" ", "+");
            await _userRepository.ConfirmEmailAsync(request.Email, request.Token, cancellationToken);

        }
    }
}
