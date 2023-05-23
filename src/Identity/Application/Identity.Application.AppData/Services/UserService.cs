using FluentValidation;
using Identity.Application.AppData.Helpers;
using Identity.Application.AppData.Repositories;
using Identity.Contracts.Contexts.Users;
using Identity.Contracts.Options;
using Identity.Domain;
using IdentityServer4;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
    /// <inheritdoc />
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
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
        private readonly UserOptions _userOptions;



        public UserService(IUserRepository userRepository, IHttpContextAccessor contextAccessor, IdentityServerTools identityServerTools,
            IValidator<UserLoginRequest> userLoginValidator, IValidator<UserRegisterRequest> userRegisterValidator, IValidator<UserUpdateRequest> userUpdateValidator,
            IValidator<UserGenerateEmailTokenRequest> userGenerateEmailTokenValidator, IValidator<UserGenerateEmailConfirmationTokenRequest> userGenerateEmailConfirmationTokenValidator,
            ILogger<UserService> logger, IValidator<UserChangeEmailRequest> userChangeEmailValidator, IValidator<UserConfirmEmailRequest> userConfirmEmailValidator, 
            IOptions<UserOptions> userOptionsAccessor)
        {
            _userRepository = userRepository;
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
            _userOptions = userOptionsAccessor.Value;
        }
        /// <inheritdoc />
        public Task<IReadOnlyCollection<UserSummary>> GetAllAsync(int? offset, int? count, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение списка пользователей с параметрами: {2} = {3}, {4} = {5}",
                nameof(UserService), nameof(GetAllAsync), nameof(offset), offset, nameof(count), count);

            if (!count.HasValue)
            {
                count = _userOptions.ListDefaultCount;
            }

            return _userRepository.GetAllAsync(offset.GetValueOrDefault(), count.GetValueOrDefault(), cancellation);
        }
        /// <inheritdoc />
        public Task<UserDetails> GetByIdAsync(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение пользователя с ID: {2}",
                nameof(UserService), nameof(GetByIdAsync), id);

            return _userRepository.GetByIdAsync(id, cancellation);
        }
        /// <inheritdoc />
        public async Task<Guid> RegisterAsync(UserRegisterRequest registerRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Регистрация пользователя со следующим email: {2}",
                nameof(UserService), nameof(RegisterAsync), registerRequest.Email);

            var validationResult = _userRegisterValidator.Validate(registerRequest);
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Модель регистрации нового пользователя не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }
            
            var newUserId = await _userRepository.AddIfNotExistsAsync(registerRequest, cancellation);

            return newUserId;
        }
        /// <inheritdoc />
        public Task<UserDetails> UpdateAsync(Guid id, UserUpdateRequest updateRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Обновление информации о пользователе с ID: {2} со следующей моделью обновления {3}: {4}",
                nameof(UserService), nameof(UpdateAsync), id, nameof(UserUpdateRequest), JsonConvert.SerializeObject(updateRequest));

            var validationResult = _userUpdateValidator.Validate(updateRequest);
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Модель обновления пользователя не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }

            return _userRepository.UpdateAsync(id, updateRequest, cancellation);
        }
        /// <inheritdoc />
        public async Task DeleteAsync(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Удаление пользователя с ID: {2}",
                nameof(UserService), nameof(DeleteAsync), id);

            await _userRepository.DeleteAsync(id, cancellation);
        }
        /// <inheritdoc />
        public Task<bool> IsInRoleAsync(Guid userId, string role, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение информации имеет ли пользователь с ID: {2} роль {3}",
                nameof(UserService), nameof(IsInRoleAsync), userId, role);

            return _userRepository.IsInRoleAsync(userId, role, cancellation);
        }
        /// <inheritdoc />
        public async Task<EmailChangeToken> GenerateEmailTokenAsync(UserGenerateEmailTokenRequest request, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Генерация токена изменения email с {2} на {3}",
                nameof(UserService), nameof(GenerateEmailTokenAsync), request.CurrentEmail, request.NewEmail);

            var validationResult = _userGenerateEmailTokenValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Модель генерации токена не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }

            var isPasswordVerified = await _userRepository.CheckPasswordAsync(request.CurrentEmail, request.Password, cancellation);
            if (!isPasswordVerified)
            {
                throw new ArgumentException("Введен неверный пароль.");
            }

            var token = await _userRepository.GenerateEmailTokenAsync(request.CurrentEmail, request.NewEmail, cancellation);

            return token;
        }
        /// <inheritdoc />
        public async Task<EmailConfirmationToken> GenerateEmailConfirmationTokenAsync(UserGenerateEmailConfirmationTokenRequest request, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Генерация токена подтверждения email: {2}",
                nameof(UserService), nameof(GenerateEmailConfirmationTokenAsync), request.Email);

            var validationResult = _userGenerateEmailConfirmationTokenValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Модель генерации токена не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }

            var token = await _userRepository.GenerateEmailConfirmationTokenAsync(request.Email, cancellation);

            return token;
        }

        /// <inheritdoc />
        public async Task ChangeEmailAsync(UserChangeEmailRequest request, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Изменение email пользователя с {2} на {3}",
                nameof(UserService), nameof(ChangeEmailAsync), request.CurrentEmail, request.NewEmail);

            var validationResult = _userChangeEmailValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Модель изменения почты не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }

            request.Token = request.Token.Replace(" ", "+");
            await _userRepository.ChangeEmailAsync(request.CurrentEmail, request.NewEmail, request.Token, cancellation);

        }
        /// <inheritdoc />
        public async Task ConfirmEmailAsync(UserConfirmEmailRequest request, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Подтверждение email пользователя: {2}",
                nameof(UserService), nameof(ConfirmEmailAsync), request.Email);

            var validationResult = _userConfirmEmailValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Модель подтверждения почты не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }
            request.Token = request.Token.Replace(" ", "+");
            await _userRepository.ConfirmEmailAsync(request.Email, request.Token, cancellation);

        }
    }
}
