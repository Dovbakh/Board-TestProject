using AutoMapper;
using Identity.Contracts.Clients.Users;
using Identity.Contracts.Contexts.Users;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public UserClient(HttpClient httpClient, IConfiguration configuration, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _mapper = mapper;
            _contextAccessor = contextAccessor;

            SetToken();
        }


        public async Task<IReadOnlyCollection<UserSummaryClientResponse>> GetAllAsync(int offset, int count, CancellationToken cancellation)
        {            
            var uri = $"v1/user?{nameof(offset)}={offset.ToString()}&{nameof(count)}={count.ToString()}";  //"?" + nameof(offset) + "=" + offset.ToString() + 
            using var response = await _httpClient.GetAsync(uri, cancellation);           
            response.EnsureSuccessStatusCode();

            var users = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<UserSummary>>();
            var clientResponse = _mapper.Map<IReadOnlyCollection<UserSummary>, IReadOnlyCollection<UserSummaryClientResponse>>(users);

            return clientResponse;
        }

        public async Task<UserDetailsClientResponse> GetByIdAsync(Guid id, CancellationToken cancellation)
        {
            var uri = $"v1/user/{id.ToString()}";
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
            var registerRequest = _mapper.Map<UserRegisterClientRequest, UserRegisterRequest>(registerClientRequest);

            var uri = $"v1/user/register";          
            using var response = await _httpClient.PostAsJsonAsync(uri, registerRequest, cancellation);
            response.EnsureSuccessStatusCode();

            var userId = await response.Content.ReadFromJsonAsync<Guid>();

            return userId;
        }

        public async Task<TokenResponse> LoginAsync(PasswordTokenRequest tokenRequest, CancellationToken cancellation)
        {
            var response = await _httpClient.RequestPasswordTokenAsync(tokenRequest, cancellation);        
            
            return response;
        }

        public async Task<TokenResponse> LoginAsync(RefreshTokenRequest tokenRequest, /*string IdentityClientId, string IdentityScopeName, */CancellationToken cancellation)
        {
            var response = await _httpClient.RequestRefreshTokenAsync(tokenRequest);

            return response;
        }

        public async Task<TokenIntrospectionResponse> IsLoginedAsync(CancellationToken cancellation)
        {
            //var loginRequest = _mapper.Map<UserLoginClientRequest, UserLoginRequest>(loginClientRequest);

            //var uri = $"v1/user/login";
            //using var response = await _httpClient.PostAsJsonAsync(uri, loginRequest, cancellation);
            //response.EnsureSuccessStatusCode();

            //var accessToken = await response.Content.ReadFromJsonAsync<string>();

            //return accessToken;

            //TODO: какой запрос отправлять?
           var token = _contextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (token == null)
                return null;

            token = token.Replace("Bearer ", "");

            var tokenRequest = new TokenIntrospectionRequest
            {
                Address = "connect/introspect",
                ClientId = "external",/*IdentityClientId,*/
                Token = token
            };

            var response = await _httpClient.IntrospectTokenAsync(tokenRequest, cancellation);

            return response;
        }

        public async Task<UserDetailsClientResponse> UpdateAsync(Guid id, UserUpdateClientRequest updateClientRequest, CancellationToken cancellation)
        {
            var updateRequest = _mapper.Map<UserUpdateClientRequest, UserUpdateRequest>(updateClientRequest);

            var uri = $"v1/user/{id.ToString()}";
            using var response = await _httpClient.PutAsJsonAsync(uri, updateRequest, cancellation);
            response.EnsureSuccessStatusCode();

            var user = await response.Content.ReadFromJsonAsync<UserDetails>();
            var clientResponse = _mapper.Map<UserDetails, UserDetailsClientResponse>(user);

            return clientResponse;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellation)
        {
            var uri = $"v1/user/{id.ToString()}";
            using var response = await _httpClient.DeleteAsync(uri,cancellation);
            response.EnsureSuccessStatusCode();
        }

        public async Task<string> GenerateEmailTokenAsync(UserGenerateEmailTokenClientRequest clientRequest, CancellationToken cancellation)
        {
            var request = _mapper.Map<UserGenerateEmailTokenClientRequest, UserGenerateEmailTokenRequest>(clientRequest);

            var uri = $"v1/user/generate-email-token";
            using var response = await _httpClient.PostAsJsonAsync(uri, request, cancellation);
            response.EnsureSuccessStatusCode();

            var token = await response.Content.ReadFromJsonAsync<EmailChangeToken>();

            return token.Value;
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(UserGenerateEmailConfirmationTokenClientRequest clientRequest, CancellationToken cancellation)
        {
            var request = _mapper.Map<UserGenerateEmailConfirmationTokenClientRequest, UserGenerateEmailConfirmationTokenRequest>(clientRequest);

            var uri = $"v1/user/generate-email-confirmation-token";
            using var response = await _httpClient.PostAsJsonAsync(uri, request, cancellation);
            response.EnsureSuccessStatusCode();

            var token = await response.Content.ReadFromJsonAsync<EmailConfirmationToken>();

            return token.Value;
        }

        public async Task ChangeEmailAsync(UserChangeEmailClientRequest clientRequest, CancellationToken cancellation)
        {
            var request = _mapper.Map<UserChangeEmailClientRequest, UserChangeEmailRequest>(clientRequest);

            var uri = $"v1/user/change-email";
            using var response = await _httpClient.PostAsJsonAsync(uri, request, cancellation);
            response.EnsureSuccessStatusCode();
        }

        public async Task ConfirmEmailAsync(UserEmailConfirmClientRequest clientRequest, CancellationToken cancellation)
        {
            var request = _mapper.Map<UserEmailConfirmClientRequest, UserConfirmEmailRequest>(clientRequest);

            var uri = $"v1/user/confirm-email";
            using var response = await _httpClient.PostAsJsonAsync(uri, request, cancellation);
            response.EnsureSuccessStatusCode();
        }

        private async void SetToken()
        {
            var token = _contextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (token == null)
                return;

            token = token.Replace("Bearer ", "");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
