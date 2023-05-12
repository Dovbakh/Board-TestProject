using FluentValidation;
using Identity.Application.AppData.Helpers;
using Identity.Application.AppData.Repositories;
using Identity.Contracts.Contexts.Users;
using Identity.Domain;
using IdentityServer4;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RedLockNet;
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
        private readonly IValidator<UserChangeEmailRequest> _userChangeEmailValidator;
        private readonly IValidator<UserConfirmEmailRequest> _userConfirmEmailValidator;
        private readonly IValidator<UserGenerateEmailTokenRequest> _userGenerateEmailTokenValidator;
        private readonly IValidator<UserGenerateEmailConfirmationTokenRequest> _userGenerateEmailConfirmationTokenValidator;
        private readonly ILogger<UserService> _logger;
        private const int UserListCount = 10;


        public UserService(IUserRepository userRepository, IConfiguration configuration, IHttpContextAccessor contextAccessor, IdentityServerTools identityServerTools,
            IValidator<UserLoginRequest> userLoginValidator, IValidator<UserRegisterRequest> userRegisterValidator, IValidator<UserUpdateRequest> userUpdateValidator,
            IValidator<UserGenerateEmailTokenRequest> userGenerateEmailTokenValidator, IValidator<UserGenerateEmailConfirmationTokenRequest> userGenerateEmailConfirmationTokenValidator,
            ILogger<UserService> logger, IValidator<UserChangeEmailRequest> userChangeEmailValidator, IValidator<UserConfirmEmailRequest> userConfirmEmailValidator)
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
            _logger = logger;
            _userChangeEmailValidator = userChangeEmailValidator;
            _userConfirmEmailValidator = userConfirmEmailValidator;
        }

        public Task<IReadOnlyCollection<UserSummary>> GetAllAsync(int? offset, int? count, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{0} -> Получение всех пользователей.",
                nameof(GetAllAsync));

            if (count.HasValue)
            {
                return _userRepository.GetAllAsync(offset.GetValueOrDefault(), count.GetValueOrDefault(), cancellationToken);
            }

            try
            {
                count = Int32.Parse(_configuration.GetSection("Users").GetRequiredSection("ListDefaultCount").Value);
            }
            catch
            {
                _logger.LogWarning("{0} -> В конфигурации указано невалидное значение количества получаемых пользователей по умолчанию Users->ListDefaultCount",
                    nameof(GetAllAsync));
                count = UserListCount;
            }

            return _userRepository.GetAllAsync(offset.GetValueOrDefault(), count.GetValueOrDefault(), cancellationToken);
        }

        public Task<UserDetails> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{0} -> Получение пользователя с ID: {1}",
                nameof(GetByIdAsync), id);

            return _userRepository.GetByIdAsync(id, cancellationToken);
        }

        public async Task<Guid> RegisterAsync(UserRegisterRequest registerRequest, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{0} -> Регистрация пользователя со следующим email: {1}",
                nameof(RegisterAsync), registerRequest.Email);

            var validationResult = _userRegisterValidator.Validate(registerRequest);
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Модель регистрации нового пользователя не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }

            var existingUser = await _userRepository.GetByEmailAsync(registerRequest.Email, cancellationToken);
            if (existingUser != null)
            {
                throw new ArgumentException($"Пользователь с почтой '{registerRequest.Email}' уже зарегистрирован!");
            }
            

            return await _userRepository.AddAsync(registerRequest, cancellationToken);
        }

        public async Task<string> LoginAsync(UserLoginRequest loginRequest, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{0} -> Аутентификация пользователя с email: {1}",
                nameof(LoginAsync), loginRequest.UserName);

            var validationResult = _userLoginValidator.Validate(loginRequest);
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Модель аутентификации пользователя не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }

            // TODO: сделать ручную генерацию токена для хэндлинга ошибок _identityServerTools

            var existingUser = await _userRepository.GetByEmailAsync(loginRequest.UserName, cancellationToken);
            if (existingUser == null)
            {
                throw new KeyNotFoundException("Введен неверный email или пароль.");
            }

            var isPasswordVerified = await _userRepository.CheckPasswordAsync(loginRequest.UserName, loginRequest.Password, cancellationToken);
            if (!isPasswordVerified)
            {
                throw new ArgumentException("Введен неверный email или пароль.");
            }

            var token = "";// GenerateToken(existingUser);


            return token;
        }

        public Task<UserDetails> UpdateAsync(Guid id, UserUpdateRequest updateRequest, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{0} -> Обновление информации о пользователе с ID: {1} со следующей моделью обновления {2}: {3}",
                nameof(UpdateAsync), id, nameof(UserUpdateRequest), JsonConvert.SerializeObject(updateRequest));

            var validationResult = _userUpdateValidator.Validate(updateRequest);
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Модель обновления пользователя не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }

            return _userRepository.UpdateAsync(id, updateRequest, cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{0} -> Удаление пользователя с ID: {1}",
                nameof(DeleteAsync), id);
            //var currentUser = await GetCurrent(cancellationToken);
            //;
            //if (currentUser.Id != id && currentUser.RoleName != "admin")
            //{
            //    throw new AccessViolationException("Нет прав.");
            //}

            await _userRepository.DeleteAsync(id, cancellationToken);
        }

        public Task<bool> IsInRoleAsync(Guid userId, string role, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{0} -> Получение информации имеет ли пользователь с ID: {1} роль {2}",
                nameof(IsInRoleAsync), userId, role);

            return _userRepository.IsInRoleAsync(userId, role, cancellationToken);
        }

        public async Task<EmailChangeToken> GenerateEmailTokenAsync(UserGenerateEmailTokenRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{0} -> Генерация токена изменения email с {1} на {2}",
                nameof(GenerateEmailTokenAsync), request.CurrentEmail, request.NewEmail);

            var validationResult = _userGenerateEmailTokenValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Модель генерации токена не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
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
            _logger.LogInformation("{0} -> Генерация токена подтверждения email: {1}",
                nameof(GenerateEmailConfirmationTokenAsync), request.Email);

            var validationResult = _userGenerateEmailConfirmationTokenValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Модель генерации токена не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }

            var token = await _userRepository.GenerateEmailConfirmationTokenAsync(request.Email, cancellationToken);

            return token;
        }
       

        public async Task ChangeEmailAsync(UserChangeEmailRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{0} -> Изменение email пользователя с {1} на {2}",
                nameof(ChangeEmailAsync), request.CurrentEmail, request.NewEmail);

            var validationResult = _userChangeEmailValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Модель изменения почты не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }

            request.Token = request.Token.Replace(" ", "+");
            await _userRepository.ChangeEmailAsync(request.CurrentEmail, request.NewEmail, request.Token, cancellationToken);

        }

        public async Task ConfirmEmailAsync(UserConfirmEmailRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{0} -> Подтверждение email пользователя: {1}",
                nameof(ConfirmEmailAsync), request.Email);

            var validationResult = _userConfirmEmailValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Модель подтверждения почты не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }
            request.Token = request.Token.Replace(" ", "+");
            await _userRepository.ConfirmEmailAsync(request.Email, request.Token, cancellationToken);

        }
    }
}
