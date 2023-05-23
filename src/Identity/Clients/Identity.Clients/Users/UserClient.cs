using AutoMapper;
using Identity.Contracts.Clients.Users;
using Identity.Contracts.Contexts.Users;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Identity.Clients.Users
{
    public class UserClient : IUserClient
    { 
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<UserClient> _logger;

        public UserClient(HttpClient httpClient, IMapper mapper, IHttpContextAccessor contextAccessor, IHttpClientFactory httpClientFactory, ILogger<UserClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = _httpClientFactory.CreateClient("UserClient");
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }


        public async Task<IReadOnlyCollection<UserSummaryClientResponse>> GetAllAsync(int offset, int count, CancellationToken cancellation)
        {           
            var uri = $"v1/user?{nameof(offset)}={offset.ToString()}&{nameof(count)}={count.ToString()}";
            _logger.LogInformation("{0}:{1} -> {2}{3} -> Получение списка пользователей с параметрами {4}: {5}, {6}: {7}",
                nameof(UserClient), nameof(GetAllAsync), _httpClient.BaseAddress, uri, nameof(offset), offset, nameof(count), count);

            using var response = await _httpClient.GetAsync(uri, cancellation);           
            response.EnsureSuccessStatusCode();

            var users = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<UserSummary>>();

            var clientResponse = _mapper.Map<IReadOnlyCollection<UserSummary>, IReadOnlyCollection<UserSummaryClientResponse>>(users);
            return clientResponse;
        }

        public async Task<UserDetailsClientResponse> GetByIdAsync(Guid id, CancellationToken cancellation)
        {
            var uri = $"v1/user/{id.ToString()}";
            _logger.LogInformation("{0}:{1} -> {2}{3} -> Получение пользователя с ID: {4}",
                nameof(UserClient), nameof(GetByIdAsync), _httpClient.BaseAddress, uri, id);

            using var response = await _httpClient.GetAsync(uri, cancellation);
            response.EnsureSuccessStatusCode();

            if(response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return null;
            }
 
            var user = await response.Content.ReadFromJsonAsync<UserDetails>();

            var clientResponse = _mapper.Map<UserDetails, UserDetailsClientResponse>(user);
            return clientResponse;
        }
        
        public async Task<Guid> RegisterAsync(UserRegisterClientRequest registerClientRequest, CancellationToken cancellation)
        {            
            var uri = $"v1/user/register";
            _logger.LogInformation("{0}:{1} -> {2}{3} -> Регистрация пользователя со следующим email: {4}",
                nameof(UserClient), nameof(RegisterAsync), _httpClient.BaseAddress, uri, registerClientRequest.Email);

            var registerRequest = _mapper.Map<UserRegisterClientRequest, UserRegisterRequest>(registerClientRequest);
            using var response = await _httpClient.PostAsJsonAsync(uri, registerRequest, cancellation);
            response.EnsureSuccessStatusCode();

            var userId = await response.Content.ReadFromJsonAsync<Guid>();
            return userId;
        }

        public async Task<TokenResponse> GetTokenAsync(IdentityClientCredentials clientCredentials, UserLoginClientRequest userCredentials, CancellationToken cancellation)
        {
            var uri = "connect/token";
            _logger.LogInformation("{0}:{1} -> {2}{3} -> Получение токена для пользователя со следующим email: {4}",
                nameof(UserClient), nameof(GetTokenAsync), _httpClient.BaseAddress, uri, userCredentials.UserName);

            var tokenRequest = new PasswordTokenRequest
            {
                Address = uri,
                ClientId = clientCredentials.Id,
                ClientSecret = clientCredentials.Secret,
                Scope = clientCredentials.Scope,
                UserName = userCredentials.UserName,
                Password = userCredentials.Password
            };

            var tokenResponse = await _httpClient.RequestPasswordTokenAsync(tokenRequest, cancellation);                   
            return tokenResponse;
        }

        public async Task<TokenResponse> GetTokenAsync(IdentityClientCredentials clientCredentials, string refreshToken, CancellationToken cancellation)
        {
            var uri = "connect/token";
            _logger.LogInformation("{0}:{1} -> {2}{3} -> Получение токена для пользователя с помощью рефреш токена.",
                nameof(UserClient), nameof(GetTokenAsync), _httpClient.BaseAddress, uri);

            var tokenRequest = new RefreshTokenRequest
            {
                Address = uri,
                ClientId = clientCredentials.Id,
                ClientSecret = clientCredentials.Secret,
                Scope = clientCredentials.Scope,
                RefreshToken = refreshToken
            };

            var tokenResponse = await _httpClient.RequestRefreshTokenAsync(tokenRequest);
            return tokenResponse;
        }

        public async Task<TokenIntrospectionResponse> IntrospectTokenAsync(IdentityClientCredentials clientCredentials, string token, CancellationToken cancellation)
        {
            var uri = "connect/introspect";
            _logger.LogInformation("{0}:{1} -> {2}{3} -> Валидация токена для пользователя.",
                nameof(UserClient), nameof(IntrospectTokenAsync), _httpClient.BaseAddress, uri);

            var tokenRequest = new TokenIntrospectionRequest
            {
                Address = uri,
                ClientId = clientCredentials.Id,
                ClientSecret = clientCredentials.Secret,
                Token = token
            };

            var tokenResponse = await _httpClient.IntrospectTokenAsync(tokenRequest, cancellation);
            return tokenResponse;
        }

        public async Task<UserDetailsClientResponse> UpdateAsync(Guid id, UserUpdateClientRequest updateClientRequest, CancellationToken cancellation)
        {           
            var uri = $"v1/user/{id.ToString()}";
            _logger.LogInformation("{0}:{1} -> {2}{3} -> Обновление информации о пользователе с ID: {4} со следующей моделью обновления {5}: {6}",
                nameof(UserClient), nameof(UpdateAsync), _httpClient.BaseAddress, uri, id, nameof(UserUpdateRequest), JsonConvert.SerializeObject(updateClientRequest));

            var updateRequest = _mapper.Map<UserUpdateClientRequest, UserUpdateRequest>(updateClientRequest);
            using var response = await _httpClient.PutAsJsonAsync(uri, updateRequest, cancellation);
            response.EnsureSuccessStatusCode();

            var user = await response.Content.ReadFromJsonAsync<UserDetails>();
            var clientResponse = _mapper.Map<UserDetails, UserDetailsClientResponse>(user);

            return clientResponse;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellation)
        {
            var uri = $"v1/user/{id.ToString()}";
            _logger.LogInformation("{0}:{1} -> {2}{3} -> Удаление пользователя с ID: {4}",
                nameof(UserClient), nameof(DeleteAsync), _httpClient.BaseAddress, uri, id);

            using var response = await _httpClient.DeleteAsync(uri,cancellation);
            response.EnsureSuccessStatusCode();
        }

        public async Task<string> GenerateEmailTokenAsync(UserGenerateEmailTokenClientRequest clientRequest, CancellationToken cancellation)
        {            
            var uri = $"v1/user/generate-email-token";
            _logger.LogInformation("{0}:{1} -> {2}{3} -> Генерация токена смены email из модели {4}",
                nameof(UserClient), nameof(GenerateEmailTokenAsync), _httpClient.BaseAddress, uri, JsonConvert.SerializeObject(clientRequest));

            var request = _mapper.Map<UserGenerateEmailTokenClientRequest, UserGenerateEmailTokenRequest>(clientRequest);
            using var response = await _httpClient.PostAsJsonAsync(uri, request, cancellation);
            response.EnsureSuccessStatusCode();

            var token = await response.Content.ReadFromJsonAsync<EmailChangeToken>();

            return token.Value;
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(UserGenerateEmailConfirmationTokenClientRequest clientRequest, CancellationToken cancellation)
        {           
            var uri = $"v1/user/generate-email-confirmation-token";
            _logger.LogInformation("{0}:{1} -> {2}{3} -> Генерация токена подтверждения email из модели {4}",
                nameof(UserClient), nameof(GenerateEmailConfirmationTokenAsync), _httpClient.BaseAddress, uri, JsonConvert.SerializeObject(clientRequest));

            var request = _mapper.Map<UserGenerateEmailConfirmationTokenClientRequest, UserGenerateEmailConfirmationTokenRequest>(clientRequest);
            using var response = await _httpClient.PostAsJsonAsync(uri, request, cancellation);
            response.EnsureSuccessStatusCode();

            var token = await response.Content.ReadFromJsonAsync<EmailConfirmationToken>();

            return token.Value;
        }

        public async Task ChangeEmailAsync(UserChangeEmailClientRequest clientRequest, CancellationToken cancellation)
        {            
            var uri = $"v1/user/change-email";
            _logger.LogInformation("{0}:{1} -> {2}{3} -> Изменение email текущего пользователя с ID: {4} на {5}",
                nameof(UserClient), nameof(ChangeEmailAsync), _httpClient.BaseAddress, uri, clientRequest.CurrentEmail, clientRequest.NewEmail);

            var request = _mapper.Map<UserChangeEmailClientRequest, UserChangeEmailRequest>(clientRequest);
            using var response = await _httpClient.PostAsJsonAsync(uri, request, cancellation);
            response.EnsureSuccessStatusCode();
        }

        public async Task ConfirmEmailAsync(UserEmailConfirmClientRequest clientRequest, CancellationToken cancellation)
        {          
            var uri = $"v1/user/confirm-email";
            _logger.LogInformation("{0}:{1} -> {2}{3} -> Подтверждение email: {4}",
                nameof(UserClient), nameof(ConfirmEmailAsync), _httpClient.BaseAddress, uri, clientRequest.Email);

            var request = _mapper.Map<UserEmailConfirmClientRequest, UserConfirmEmailRequest>(clientRequest);
            using var response = await _httpClient.PostAsJsonAsync(uri, request, cancellation);
            response.EnsureSuccessStatusCode();
        }
    }
}
