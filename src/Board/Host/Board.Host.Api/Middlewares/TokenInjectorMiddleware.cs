using Board.Host.Middlewares;
using IdentityModel;
using IdentityServer4.Extensions;
using System.IdentityModel.Tokens.Jwt;

namespace Board.Host.Api.Middlewares
{
    public class TokenInjectorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TokenInjectorMiddleware> _logger;

        public TokenInjectorMiddleware(RequestDelegate next, ILogger<TokenInjectorMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Session.GetString(OidcConstants.TokenResponse.AccessToken);
            if (!string.IsNullOrEmpty(token))
            {
                if (context.Request.Headers.Authorization.IsNullOrEmpty())
                {
                    context.Request.Headers.Authorization = "Bearer " + token;
                }
            }


            await _next(context);
        }
    }
}
