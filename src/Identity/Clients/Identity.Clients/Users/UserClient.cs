using AutoMapper;
using Identity.Contracts.Clients.Users;
using Identity.Contracts.Contexts.Users;
using IdentityModel.Client;
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

        public UserClient(HttpClient httpClient, IConfiguration configuration, IMapper mapper)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _mapper = mapper;
        }


        //get all
        //get by id
        //get current
        //register
        //update
        //delete

        public async Task<IReadOnlyCollection<UserSummaryClientResponse>> GetAll(int offset, int count, CancellationToken cancellation)
        {            
            var uri = $"v1/user?{nameof(offset)}={offset.ToString()}&{nameof(count)}={count.ToString()}";  //"?" + nameof(offset) + "=" + offset.ToString() + 
            using var response = await _httpClient.GetAsync(uri, cancellation);           
            response.EnsureSuccessStatusCode();

            var users = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<UserSummary>>();
            var clientResponse = _mapper.Map<IReadOnlyCollection<UserSummary>, IReadOnlyCollection<UserSummaryClientResponse>>(users);

            return clientResponse;
        }

        public async Task<UserDetailsClientResponse> GetById(Guid id, CancellationToken cancellation)
        {
            var uri = $"v1/user/{id.ToString()}";
            using var response = await _httpClient.GetAsync(uri, cancellation);
            response.EnsureSuccessStatusCode();

            var user = await response.Content.ReadFromJsonAsync<UserDetails>();
            var clientResponse = _mapper.Map<UserDetails, UserDetailsClientResponse>(user);

            return clientResponse;
        }

        public async Task<UserDetailsClientResponse> GetCurrent(CancellationToken cancellation)
        {
            var uri = $"v1/user/current";
            using var response = await _httpClient.GetAsync(uri, cancellation);
            response.EnsureSuccessStatusCode();

            var user = await response.Content.ReadFromJsonAsync<UserDetails>();
            var clientResponse = _mapper.Map<UserDetails, UserDetailsClientResponse>(user);

            return clientResponse;
        }
        
        public async Task<Guid> Register(UserRegisterClientRequest registerClientRequest, CancellationToken cancellation)
        {
            var registerRequest = _mapper.Map<UserRegisterClientRequest, UserRegisterRequest>(registerClientRequest);

            var uri = $"v1/user/register";          
            using var response = await _httpClient.PostAsJsonAsync(uri, registerRequest, cancellation);
            response.EnsureSuccessStatusCode();

            var userId = await response.Content.ReadFromJsonAsync<Guid>();

            return userId;
        }

        public async Task<TokenResponse> Login(UserLoginClientRequest loginClientRequest, /*string IdentityClientId, string IdentityScopeName, */CancellationToken cancellation)
        {
            //var loginRequest = _mapper.Map<UserLoginClientRequest, UserLoginRequest>(loginClientRequest);

            //var uri = $"v1/user/login";          
            //using var response = await _httpClient.PostAsJsonAsync(uri, loginRequest, cancellation);
            //response.EnsureSuccessStatusCode();

            //var accessToken = await response.Content.ReadFromJsonAsync<string>();

            //return accessToken;

            // TODO: какой запрос отправлять?
            var tokenRequest = new PasswordTokenRequest
            {
                Address = "connect/token",
                ClientId = "external",/*IdentityClientId,*/
                Scope = "Board.Web",/*IdentityScopeName,*/
                UserName = loginClientRequest.Email,
                Password = loginClientRequest.Password
            };
            var response = await _httpClient.RequestPasswordTokenAsync(tokenRequest, cancellation);        
            
            return response;
        }

        public async Task<UserDetailsClientResponse> Update(Guid id, UserUpdateClientRequest updateClientRequest, CancellationToken cancellation)
        {
            var updateRequest = _mapper.Map<UserUpdateClientRequest, UserUpdateRequest>(updateClientRequest);

            var uri = $"v1/user/{id.ToString()}";
            using var response = await _httpClient.PutAsJsonAsync(uri, updateRequest, cancellation);
            response.EnsureSuccessStatusCode();

            var user = await response.Content.ReadFromJsonAsync<UserDetails>();
            var clientResponse = _mapper.Map<UserDetails, UserDetailsClientResponse>(user);

            return clientResponse;
        }

        public async Task Delete(Guid id, CancellationToken cancellation)
        {
            var uri = $"v1/user/{id.ToString()}";
            using var response = await _httpClient.DeleteAsync(uri,cancellation);
            response.EnsureSuccessStatusCode();
        }
    }
}
