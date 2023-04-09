using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using IdentityServer4;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.VisualBasic;

namespace Identity.Host.Server
{
    public static class Config
    {        public static IEnumerable<IdentityResource> IdentityResources =>
    new IdentityResource[]
    {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource
                    {
                      Name = "role",
                      UserClaims = new List<string> {"role"}
                    }
    };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("Board.Host.Api"),
                new ApiScope("Board.Web"),
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId = "test.client",
                    ClientName = "Test client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "Board.Web",
                        "Board.Host.Api"
                    }
                },

                new Client
                {
                    ClientId = "external",
                    ClientName = "External Client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    RequireClientSecret = false,

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "Board.Web"
                    }
                }
            };


        //public static IEnumerable<ApiResource> ApiResources => new[]
        //{
        //      new ApiResource("BoardApi")
        //      {
        //        Scopes = new List<string> { "Board.Host.Api", "Board.Web"},
        //        ApiSecrets = new List<Secret> {new Secret("ScopeSecret".Sha256())},
        //        UserClaims = new List<string> {"role"}
        //      }
        //};
    }
}
