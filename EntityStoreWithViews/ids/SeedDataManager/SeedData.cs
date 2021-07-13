using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Storage;
using ids.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Serilog;

namespace ids.SeedDataManager
{
    public class SeedData
    {
        public static void EnsureSeedData(string connectionString)
        {
            var migrationAssembly = typeof(SeedData).Assembly.FullName;

            var services = new ServiceCollection();

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString, sqlOptions => sqlOptions.MigrationsAssembly(migrationAssembly)));

            services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            services.AddOperationalDbContext(options =>
            {
                options.ConfigureDbContext = db => db.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationAssembly));
            });
            services.AddConfigurationDbContext(options =>
            {
                options.ConfigureDbContext = db => db.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationAssembly));
            });
            
            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            scope.ServiceProvider.GetService<PersistedGrantDbContext>().Database.Migrate();

            var configurationDbContext = scope.ServiceProvider.GetService<ConfigurationDbContext>();
            configurationDbContext.Database.Migrate();
            EnsureSeedData(configurationDbContext);

            var applicationDbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
            applicationDbContext.Database.Migrate();
            EnsureUsers(scope);
        }

        private static void EnsureUsers(IServiceScope scope)
        {
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var user = userMgr.FindByNameAsync("ymurshed").Result;

            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = "ymurshed",
                    Email = "murshed.yaad@gmail.com",
                    EmailConfirmed = true
                };

                var result = userMgr.CreateAsync(user, "Pass@123").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                object address = new 
                {
                    street_address = "Niketon",
                    locality = "Gulshan-1",
                    postal_code = 1212,
                    country = "Bangladesh"
                };

                result = userMgr.AddClaimsAsync(user, new[]
                {
                      new Claim(JwtClaimTypes.Name, "Yaad Murshed"),
                      new Claim(JwtClaimTypes.GivenName, "Yaad"),
                      new Claim(JwtClaimTypes.FamilyName, "Murshed"),
                      new Claim(JwtClaimTypes.Email, "murshed.yaad@gmail.com"),
                      new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                      new Claim(JwtClaimTypes.Role, "true", "admin"),
                      new Claim(JwtClaimTypes.WebSite, "http://ymurshed.com"),
                      new Claim(JwtClaimTypes.Address, JsonConvert.SerializeObject(address), IdentityServerConstants.ClaimValueTypes.Json)
                }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                Log.Debug("ymurshed created");
            }
            else
            {
                Log.Debug("ymurshed already exists");
            }
        }

        private static void EnsureSeedData(ConfigurationDbContext context)
        {
            if (!context.Clients.Any())
            {
                Log.Debug("Clients being populated");
                foreach (var client in Config.Clients.ToList())
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }
            else
            {
                Log.Debug("Clients already populated");
            }

            if (!context.IdentityResources.Any())
            {
                Log.Debug("IdentityResources being populated");
                foreach (var resource in Config.IdentityResources.ToList())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
            else
            {
                Log.Debug("IdentityResources already populated");
            }

            if (!context.ApiScopes.Any())
            {
                Log.Debug("ApiScopes being populated");
                foreach (var resource in Config.ApiScopes.ToList())
                {
                    context.ApiScopes.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
            else
            {
                Log.Debug("ApiScopes already populated");
            }

            if (!context.ApiResources.Any())
            {
                Log.Debug("ApiResources being populated");
                foreach (var resource in Config.ApiResources.ToList())
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
            else
            {
                Log.Debug("ApiResources already populated");
            }
        }
    }
}
