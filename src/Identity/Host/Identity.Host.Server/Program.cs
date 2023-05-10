using Identity.Clients.Users;
using Identity.Host.Server;
using Identity.Infrastructure.DataAccess;
using IdentityServer4;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Identity.Infrastructure.Registrar;
using Identity.Domain;
using Identity.Infrastructure.DataAccess.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using IdentityServer4.AccessTokenValidation;

var seed = args.Contains("/seed");
if (seed)
{
    args = args.Except(new[] { "/seed" }).ToArray();
}

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

if (seed)
{
    SeedData.EnsureSeedData(config);
}

builder.Services.AddServiceRegistrationModule();
builder.Services.AddAspNetIdentityServices();
builder.Services.AddIdentityServerServices(config);
builder.Services.AddAuthenticationServices();



//builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles(); //delete?
app.UseRouting();

app.UseIdentityServer();
app.UseAuthentication(); //
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});


app.Run();

