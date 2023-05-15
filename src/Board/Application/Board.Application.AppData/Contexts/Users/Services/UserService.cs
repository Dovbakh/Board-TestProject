using AutoMapper;
using Board.Application.AppData.Contexts.Comments.Repositories;
using Board.Application.AppData.Contexts.Images.Services;
using Board.Application.AppData.Contexts.Notifications.Services;
using Board.Contracts.Contexts.Comments;
using Board.Contracts.Contexts.Users;
using Board.Contracts.Exceptions;
using Board.Contracts.Options;
using FluentValidation;
using Identity.Clients.Users;
using Identity.Contracts.Clients.Users;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Notifier.Contracts.Contexts.Messages;
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
        private readonly UserOptions _userOptions;
        private readonly Contracts.Options.CookieOptions _cookieOptions;
        private readonly IdentityClientOptions _identityClientOptions;

        public UserService(IUserClient boardClient, IMapper mapper, IHttpContextAccessor contextAccessor,
            IValidator<UserLoginRequest> userLoginValidator, IValidator<UserRegisterRequest> userRegisterValidator, IValidator<UserUpdateRequest> userUpdateValidator,
            INotificationService notificationService, IDistributedLockFactory distributedLockFactory, ICommentRepository commentRepository, ILogger<UserService> logger,
            IValidator<UserChangeEmailRequest> userChangeEmailValidator, IValidator<UserConfirmEmailRequest> userConfirmEmailValidator,
            IValidator<UserGenerateEmailTokenRequest> userGenerateEmailTokenValidator, IValidator<UserGenerateEmailConfirmationTokenRequest> userGenerateEmailConfirmationTokenValidator,
            IValidator<UserEmail> userEmailValidator, IImageService imageService, IOptions<UserOptions> userOptionsAccessor, IOptions<Contracts.Options.CookieOptions> cookieOptionsAccessor, 
            IOptions<IdentityClientOptions> identityClientOptionsAccessor)
        {
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
            _userOptions = userOptionsAccessor.Value;
            _cookieOptions = cookieOptionsAccessor.Value;
            _identityClientOptions = identityClientOptionsAccessor.Value;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<UserSummary>> GetAllAsync(int? offset, int? count, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение списка пользователей с параметрами: {2} = {3}, {4} = {5}",
                nameof(UserService), nameof(GetAllAsync), nameof(offset), offset, nameof(count), count);

            if (!count.HasValue)
            {
                count = _userOptions.ListDefaultCount;
            }
           
            var clientResponse = await _userClient.GetAllAsync(offset.GetValueOrDefault(), count.GetValueOrDefault(), cancellation);
            var users = _mapper.Map<IReadOnlyCollection<UserSummaryClientResponse>, IReadOnlyCollection<UserSummary>>(clientResponse);

            return users;
        }

        /// <inheritdoc />
        public async Task<UserDetails> GetByIdAsync(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение пользователя с ID: {2}",
                nameof(UserService), nameof(GetByIdAsync), id);

            var clientResponse = await _userClient.GetByIdAsync(id, cancellation);
            var user = _mapper.Map<UserDetailsClientResponse, UserDetails>(clientResponse);
            if(user == null)
            {
                throw new KeyNotFoundException($"Не найден пользователь с ID: {id}");
            }

            var rating = await _commentRepository.GetUserRatingAsync(id, cancellation);
            user.Rating = rating;

            return user;
        }

        /// <inheritdoc />
        public async Task<UserDetails> GetCurrentAsync(CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение текущего пользователя.",
                nameof(UserService), nameof(GetCurrentAsync));

            var currentUserId = GetCurrentId(cancellation);
            if(!currentUserId.HasValue)
            {
                throw new UnauthorizedAccessException("Пользователь не авторизован.");
            }

            var clientResponse = await _userClient.GetByIdAsync(currentUserId.Value, cancellation);
            var user = _mapper.Map<UserDetailsClientResponse, UserDetails>(clientResponse);

            return user;
        }

        /// <inheritdoc />
        public Guid? GetCurrentId(CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение ID текущего пользователя.",
                nameof(UserService), nameof(GetCurrentId));

            var claims = _contextAccessor.HttpContext.User.Claims;
            var claimId = claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            
            var isGuid = Guid.TryParse(claimId, out Guid userId);
            if(!isGuid)
            {
                return null;
            }
            
            return userId;
        }

        /// <inheritdoc />
        public string GetCurrentEmail(CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Получение ID текущего пользователя.",
                nameof(UserService), nameof(GetCurrentId));

            var claims = _contextAccessor.HttpContext.User.Claims;
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            return email;
        }

        /// <inheritdoc />
        public Guid GetAnonymousId(CancellationToken cancellation)
        {
            var cookie = _contextAccessor.HttpContext.Request.Cookies[_cookieOptions.AnonymousUserKey];
            var isGuid = Guid.TryParse(cookie, out Guid anonymousId);
            if (isGuid)
            {
                return anonymousId;
            }

            anonymousId = Guid.NewGuid();
            _contextAccessor.HttpContext.Response.Cookies.Append(_cookieOptions.AnonymousUserKey, anonymousId.ToString());

            return anonymousId;
        }

        /// <inheritdoc />
        public async Task<bool> IsLoginedAsync(CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Проверка текущего пользователя на логин.",
                nameof(UserService), nameof(GetCurrentId));

            var token = _contextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (token == null)
                return false;
            token = token.Replace("Bearer ", "");

            var clientCredentials = new IdentityClientCredentials
            {
                Id = _identityClientOptions.ApiResourseCredentials.Id,
                Secret = _identityClientOptions.ApiResourseCredentials.Secret
            };

            var response = await _userClient.IntrospectTokenAsync(clientCredentials, token, cancellation);

            return response.IsActive;
        }

        /// <inheritdoc />
        public bool HasPermission(Guid userId, CancellationToken cancellation)
        {
            var currentUserId = GetCurrentId(cancellation);
            if (!currentUserId.HasValue)
            {
                return false;
            }

            if(currentUserId.Value != userId)
            {
                return false;
            }

            var isAdmin = IsAdmin(userId, cancellation);
            if(!isAdmin)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        public bool IsCurrentUser(Guid userId, CancellationToken cancellation)
        {
            var claims = _contextAccessor.HttpContext.User.Claims;
            var claimId = claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            var isGuid = Guid.TryParse(claimId, out Guid currentUserId);
            if (!isGuid)
            {
                return false;
            }

            if (userId != currentUserId)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        public bool IsAdmin(Guid userId, CancellationToken cancellation)
        {
            var claims = _contextAccessor.HttpContext.User.Claims;
            var claimid = claims.FirstOrDefault(c => c.Type == "role")?.Value;

            if (claimid == "admin")
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public async Task<TokenResponse> LoginAsync(UserLoginRequest loginRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Аутентификация пользователя с email: {2}",
                nameof(UserService), nameof(LoginAsync), loginRequest.UserName);

            var validationResult = _userLoginValidator.Validate(loginRequest);
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Модель аутентификации пользователя не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }

            var clientCredentials = new IdentityClientCredentials
            {
                Id = _identityClientOptions.ExternalClientCredentials.Id,
                Secret = _identityClientOptions.ExternalClientCredentials.Secret,
                Scope = _identityClientOptions.ExternalClientCredentials.Scope
            };

            var userCredentials = new UserLoginClientRequest
            {
                UserName = loginRequest.UserName,
                Password = loginRequest.Password
            };

            var tokenResponse = await _userClient.GetTokenAsync(clientCredentials, userCredentials, cancellation);

            if(tokenResponse.IsError)
            {
                throw new ArgumentException($"Ошибка при получении токена: {tokenResponse.ErrorDescription}");
            }

            return tokenResponse;
        }

        /// <inheritdoc />
        public async Task<TokenResponse> LoginAsync(UserLoginRefreshRequest loginRefreshRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Аутентификация пользователя с помощью рефреш-токена",
                nameof(UserService), nameof(LoginAsync));

            var clientCredentials = new IdentityClientCredentials
            {
                Id = _identityClientOptions.ExternalClientCredentials.Id,
                Secret = _identityClientOptions.ExternalClientCredentials.Secret,
                Scope = _identityClientOptions.ExternalClientCredentials.Scope
            };

            var tokenResponse = await _userClient.GetTokenAsync(clientCredentials, loginRefreshRequest.refresh_token, cancellation);

            if (tokenResponse.IsError)
            {
                throw new ArgumentException($"Ошибка при получении токена: {tokenResponse.ErrorDescription}");
            }

            return tokenResponse;
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

            var registerClientRequest = _mapper.Map<UserRegisterRequest, UserRegisterClientRequest>(registerRequest);          
            var userId = await _userClient.RegisterAsync(registerClientRequest, cancellation);


            return userId;
        }

        /// <inheritdoc />
        public async Task<UserDetails> UpdateAsync(Guid id, UserUpdateRequest updateRequest, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Обновление информации о пользователе с ID: {2} со следующей моделью обновления {3}: {4}",
                nameof(UserService), nameof(UpdateAsync), id, nameof(UserUpdateRequest), JsonConvert.SerializeObject(updateRequest));

            var validationResult = _userUpdateValidator.Validate(updateRequest);
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Модель обновления пользователя не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }

            var hasPermission = HasPermission(id, cancellation);
            if (!hasPermission)
            {
                throw new ForbiddenException($"Нет доступа для обновления текущего пользователя.");
            }

            if (updateRequest.PhotoId.HasValue)
            {
                var isPhotoExists = await _imageService.IsImageExists(updateRequest.PhotoId.Value, cancellation);
                if (!isPhotoExists)
                {
                    throw new KeyNotFoundException($"На файлом сервисе не найдено изображение с ID: {updateRequest.PhotoId}");
                }
            }

            var updateClientRequest = _mapper.Map<UserUpdateRequest, UserUpdateClientRequest>(updateRequest);
            var clientResponse = await _userClient.UpdateAsync(id, updateClientRequest, cancellation);
            var updatedUser = _mapper.Map<UserDetailsClientResponse, UserDetails>(clientResponse);

            return updatedUser;
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid id, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Удаление пользователя с ID: {2}",
                nameof(UserService), nameof(DeleteAsync), id);

            await _userClient.DeleteAsync(id, cancellation);
        }

        /// <inheritdoc />
        public async Task SendEmailTokenAsync(UserGenerateEmailTokenRequest request, string changeLink, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Генерация токена смены email из модели {2} и отправка его на новый email.",
                nameof(UserService), nameof(SendEmailTokenAsync), JsonConvert.SerializeObject(request));

            var validationResult = _userGenerateEmailTokenValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Модель генерации токена для изменения почты не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }

            var clientRequest = _mapper.Map<UserGenerateEmailTokenRequest, UserGenerateEmailTokenClientRequest>(request);
            var token = await _userClient.GenerateEmailTokenAsync(clientRequest, cancellation);          
            
            var bodyWarning = $"Поступил запрос на изменение электронной почты на <b>{request.NewEmail}</b>. " +
                                    $"Если это были вы, то ничего делать не нужно. В противном случае напишите в поддержку.";
            var subjectWarning = "Изменение электронной почты";
            var messageWarning = new NotificationDetails { Receiver = request.CurrentEmail, Subject = subjectWarning, Body = bodyWarning };
            await _notificationService.SendMessage(messageWarning);

            changeLink = changeLink.Replace("tokenValue", token);
            var bodyConfirm = $"Для изменения почты перейдите по <a href='{changeLink}'>ссылке</a>";
            var subjectConfirm = "Изменение электронной почты";
            var messageConfirm = new NotificationDetails { Receiver = request.CurrentEmail, Subject = subjectConfirm, Body = bodyConfirm };
            await _notificationService.SendMessage(messageConfirm);
        }

        /// <inheritdoc />
        public async Task SendEmailConfirmationTokenAsync(UserGenerateEmailConfirmationTokenRequest request, string confirmLink, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Генерация токена подтверждения email из модели {2} и отправка его на email.",
                nameof(UserService), nameof(SendEmailConfirmationTokenAsync), JsonConvert.SerializeObject(request));

            var validationResult = _userGenerateEmailConfirmationTokenValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Модель генерации токена для подтверждения почты не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }

            var clientRequest = _mapper.Map<UserGenerateEmailConfirmationTokenRequest, UserGenerateEmailConfirmationTokenClientRequest>(request);
            var token = await _userClient.GenerateEmailConfirmationTokenAsync(clientRequest, cancellation);

            confirmLink = confirmLink.Replace("tokenValue", token);
            var bodyConfirm = $"Для подтверждения почты перейдите по <a href='{confirmLink}'>ссылке</a>";
            var subjectConfirm = "Подтверждение электронной почты";
            var messageConfirm = new NotificationDetails { Receiver = request.Email, Subject = subjectConfirm, Body = bodyConfirm };
            await _notificationService.SendMessage(messageConfirm);
        }

        /// <inheritdoc/>
        public async Task ChangeEmailAsync(string newEmail, string token, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Изменение email текущего пользователя с ID: {2} на {3}",
                nameof(UserService), nameof(ChangeEmailAsync), GetCurrentId(cancellation), newEmail);

            var validationResult = _userEmailValidator.Validate(new UserEmail { Value = newEmail});
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Модель изменения почты не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }

            var currentUserEmail = GetCurrentEmail(cancellation);

            var clientRequest = new UserChangeEmailClientRequest { CurrentEmail = currentUserEmail, NewEmail = newEmail, Token = token };
            await _userClient.ChangeEmailAsync(clientRequest, cancellation);
        }

        /// <inheritdoc/>
        public async Task ConfirmEmailAsync(string email, string token, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Подтверждение email: {2} текущего пользователя с ID: {3}",
                nameof(UserService), nameof(ChangeEmailAsync), email, GetCurrentId(cancellation));

            var validationResult = _userEmailValidator.Validate(new UserEmail { Value = email });
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Модель подтверждения почты не прошла валидацию. Ошибки: {JsonConvert.SerializeObject(validationResult)}");
            }

            var currentUser = await GetCurrentAsync(cancellation);

            var clientRequest = new UserEmailConfirmClientRequest { Email = email, Token = token };
            await _userClient.ConfirmEmailAsync(clientRequest, cancellation);
        }

    }
}
