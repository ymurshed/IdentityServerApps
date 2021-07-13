using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Newtonsoft.Json;

namespace ids.SeedDataManager
{
    public static class Config
    {
        public static object Address => new
        {
            street_address = "Niketon",
            locality = "Gulshan-1",
            postal_code = 1212,
            country = "Bangladesh"
        };

        public static List<TestUser> Users =>
            new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "100200",
                    Username = "ymurshed",
                    Password = "ymurshed@03",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name, "Yaad Murshed"),
                        new Claim(JwtClaimTypes.GivenName, "Yaad"),
                        new Claim(JwtClaimTypes.FamilyName, "Murshed"),
                        new Claim(JwtClaimTypes.Email, "murshed.yaad@gmail.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.Role, "true", "admin"),
                        new Claim(JwtClaimTypes.Address, JsonConvert.SerializeObject(Address), IdentityServerConstants.ClaimValueTypes.Json)
                    }
                },
                new TestUser
                {
                    SubjectId = "100201",
                    Username = "tuser",
                    Password = "tuser@01",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name, "Test User"),
                        new Claim(JwtClaimTypes.GivenName, "Test"),
                        new Claim(JwtClaimTypes.FamilyName, "User"),
                        new Claim(JwtClaimTypes.Email, "test.user@gmail.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.Role, "true", "guest"),
                        new Claim(JwtClaimTypes.Address, JsonConvert.SerializeObject(Address), IdentityServerConstants.ClaimValueTypes.Json)
                    }
                }
            };

        public static IEnumerable<IdentityResource> IdentityResources =>
            new[]
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
            new[]
            {
                new ApiScope("weatherapi.read"),
                new ApiScope("weatherapi.write")
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new[]
            {
                new ApiResource("weatherapi")
                {
                    Scopes = new List<string> { "weatherapi.read" , "weatherapi.write" },
                    ApiSecrets = new List<Secret> {new Secret("ScopeSecret".Sha256())},
                    UserClaims = new List<string> {"role"}
                }
            };

        public static IEnumerable<Client> Clients =>
            new[]
            {
                // m2m client credentials flow client
                new Client
                {
                    ClientId = "m2m.client",
                    ClientName = "Client Credentials Client",
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "weatherapi.read", "weatherapi.write" },
                },

                // interactive client using code flow + pkce
                new Client
                {
                    ClientId = "interactive",
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    RedirectUris = { "https://localhost:5444/signin-oidc" },
                    FrontChannelLogoutUri = "https://localhost:5444/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:5444/signout-callback-oidc" },

                    AllowedGrantTypes = GrantTypes.Code,
                    AllowedScopes = { "openid", "profile", "weatherapi.read" }
                    //AllowOfflineAccess = true,
                }
            };
    }
}
