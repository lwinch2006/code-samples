using System.Reflection;
using System.Threading.Tasks;
using Application.Mapping;
using Infrastructure;
using Infrastructure.Mapping;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Scrutor;

namespace Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddInfrastructure();
            services.SetupInfrastructure();
            
            services.AddAutoMapper(typeof(ApplicationProfile));
            services.AddMediatR(Assembly.GetExecutingAssembly());

            return services;
        }

        public static IServiceCollection AddMicrosoftAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
            {
                microsoftOptions.ClientId = configuration["MicrosoftAuthentication:ClientId"];
                microsoftOptions.ClientSecret = configuration["MicrosoftAuthentication:ClientSecret"];
            });

            return services;
        }

        public static IServiceCollection AddTwitterAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication().AddTwitter(twitterOptions =>
            {
                twitterOptions.ConsumerKey = configuration["TwitterAuthentication:ClientId"];
                twitterOptions.ConsumerSecret = configuration["TwitterAuthentication:ClientSecret"];
                twitterOptions.RetrieveUserDetails = true;
            });

            return services;
        }

        public static IServiceCollection AddAzureAd(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMicrosoftIdentityWebAppAuthentication(configuration, "AzureAd", displayName: "Azure AD", subscribeToOpenIdConnectMiddlewareDiagnosticsEvents: true);

            services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.SignInScheme = IdentityConstants.ExternalScheme;
                
                options.Events.OnSignedOutCallbackRedirect += context =>
                {
                    return Task.CompletedTask;
                };

                options.Events.OnRemoteSignOut += context =>
                {
                    return Task.CompletedTask;
                };
            });
            
            return services;
        }

        public static IMvcBuilder AddAzureAdUi(this IMvcBuilder builder)
        {
            builder.AddMicrosoftIdentityUI();

            return builder;
        }

        private static void AddInfrastructure(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(InfrastructureProfile));
            services.Scan(scan =>
            {
                scan
                    .FromAssemblyOf<ITenantsRepository>().AddClasses().UsingRegistrationStrategy(RegistrationStrategy.Skip).AsImplementedInterfaces().WithScopedLifetime()
                    .FromAssemblyOf<TenantsRepository>().AddClasses().UsingRegistrationStrategy(RegistrationStrategy.Skip).AsSelf().WithScopedLifetime();
            });
        }

        private static void SetupInfrastructure(this IServiceCollection services)
        {
            var sp = services.BuildServiceProvider();
            var infrastucture = sp.GetService<IInfrastructure>();
            infrastucture.Setup();
        }
    }
}