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
    {       
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                        new IdentityResources.OpenId(),
                        new IdentityResources.Profile(),
                        //new IdentityResource
                        //    {
                        //      Name = "role",
                        //      UserClaims = new List<string> {"role"}
                        //    }
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("Board.Host.Api"),
                new ApiScope("FileStorage.Host.Server"),
                new ApiScope("Identity.Host.Server")
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId = "Board.Client",
                    ClientName = "Board Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "FileStorage.Host.Server",
                        "Identity.Host.Server"
                    }
                 
                },

                new Client
                {
                    ClientId = "external",
                    ClientName = "External Client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = { new Secret("411536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,                       
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "Board.Host.Api"
                    },

                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    AlwaysIncludeUserClaimsInIdToken = true
                    
                }
            };


        public static IEnumerable<ApiResource> ApiResources => new[]
        {
              new ApiResource("Board.Host.Api")
              {
                Scopes = new List<string> { "Board.Host.Api" },
                ApiSecrets = new List<Secret> {new Secret("311536EF-F270-4058-80CA-1C89C192F69A".Sha256())},
              }
        };
    }
}
