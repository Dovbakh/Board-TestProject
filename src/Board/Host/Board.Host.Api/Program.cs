using Board.Host.Api.Middlewares;
using Board.Host.Middlewares;
using Board.Infrastructure.Registrar;
using Identity.Clients.Users;
using IdentityServer4.AccessTokenValidation;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;


builder.Host.AddCustomLogger(config);
builder.Services.AddServiceRegistrationModule(config);
builder.Services.AddHttpClients(config);
builder.Services.AddSwaggerServices();
builder.Services.AddAuthenticationServices(config);
builder.Services.AddAuthorizationServices();
builder.Services.AddSession();

builder.Services.AddControllers().AddNewtonsoftJson();

var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSession();
//app.UseMiddleware<TokenInjectorMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ErrorHandlerMiddleware>();

app.MapControllers().RequireAuthorization("ApiScope");

app.Run();
