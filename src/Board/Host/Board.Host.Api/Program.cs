using Board.Host.Api.Middlewares;
using Board.Infrastructure.Registrar;
using Identity.Clients.Users;
using IdentityServer4.AccessTokenValidation;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;



builder.Services.AddServiceRegistrationModule(config);
builder.Services.AddHttpClients(config);

//builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerServices();
//builder.Services.AddAspNetIdentityServices();

builder.Services.AddAuthenticationServices();
builder.Services.AddAuthorizationServices();

builder.Services.AddControllers().AddNewtonsoftJson();

var app = builder.Build();






if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ErrorHandlerMiddleware>();

app.MapControllers().RequireAuthorization("ApiScope");

app.Run();
