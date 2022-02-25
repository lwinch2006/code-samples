using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using Application.Logic.ServiceBus;
using Application.Mapping;
using Application.Models;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Infrastructure;
using Infrastructure.Mapping;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Scrutor;
using ServiceBusPublisher;
using ServiceBusSubscriber;

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

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.ClaimActions.MapAll();
                
                options.ClaimActions.MapUniqueJsonKey("iss", "iss"); 
                options.ClaimActions.MapUniqueJsonKey("idp", "idp"); 
                
                options.Events.OnTicketReceived = ctx =>
                {
                    var tokens = ctx.Properties.GetTokens().ToList(); 

                    tokens.Add(new AuthenticationToken
                    {
                        Name = "TicketReceived", 
                        Value = DateTime.UtcNow.ToString()
                    });

                    ctx.Properties.StoreTokens(tokens);

                    return Task.CompletedTask;
                };
            });

            services.Configure<CookieAuthenticationOptions>(IdentityConstants.ExternalScheme,
                options =>
                {
                    options.Events.OnSigningIn = ctx =>
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

        public static IServiceCollection AddServiceBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(new ServiceBusClient(configuration[ApplicationConstants.ServiceBus.Configuration.ConnnectionStringPath]));
            services.AddSingleton(new ServiceBusAdministrationClient(configuration[ApplicationConstants.ServiceBus.Configuration.ConnnectionStringPath]));
            services.AddSingleton(
                new ServiceBusSubscriberConfiguration
                {
                    DeserializationDictionary = new Dictionary<(int, string), Type>
                    {
                    }
                });

            services.AddScoped<IServiceBusPublisher, ServiceBusPublisher.ServiceBusPublisher>();
            services.AddScoped<IServiceBusSubscriber, ServiceBusSubscriber.ServiceBusSubscriber>();
            services.AddScoped<IServiceBusSubscriberForDeadLetter, ServiceBusSubscriber.ServiceBusSubscriber>();

            var sp = services.BuildServiceProvider();
            var serviceBusPublisher = sp.GetRequiredService<IServiceBusPublisher>();
            var serviceBusSubscriber = sp.GetRequiredService<IServiceBusSubscriber>();
            
            foreach (var topic in ApplicationConstants.ServiceBus.Publish.Queues)
            {
                serviceBusPublisher.EnsureQueue(topic).GetAwaiter().GetResult();
            }
            
            foreach (var topic in ApplicationConstants.ServiceBus.Receive.Queues)
            {
                serviceBusPublisher.EnsureQueue(topic).GetAwaiter().GetResult();
            }
            
            foreach (var topic in ApplicationConstants.ServiceBus.Publish.Topics)
            {
                serviceBusPublisher.EnsureTopic(topic).GetAwaiter().GetResult();
            }
            
            foreach (var topic in ApplicationConstants.ServiceBus.Receive.Topics)
            {
                serviceBusPublisher.EnsureTopic(topic).GetAwaiter().GetResult();
            }

            foreach (var (topic, subscription) in ApplicationConstants.ServiceBus.Receive.TopicSubscriptions)
            {
                serviceBusSubscriber.EnsureTopicSubscription(topic, subscription, CancellationToken.None).GetAwaiter().GetResult();;
            }            
            
            services.AddScoped<IApplicationServiceBusClient, ApplicationServiceBusClient>();
            
            return services;
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