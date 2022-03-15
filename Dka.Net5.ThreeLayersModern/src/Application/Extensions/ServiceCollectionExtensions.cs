﻿using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using Application.Logic.ServiceBus;
using Application.Mapping;
using Application.Models;
using Application.Models.Authentication;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Infrastructure;
using Infrastructure.Mapping;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Scrutor;
using ServiceBusPublisher;
using ServiceBusSubscriber;

// TODO: Move all these extensions to Web project. This is not connected to Application.

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
            
            services.Configure<MicrosoftAccountOptions>(MicrosoftAccountDefaults.AuthenticationScheme, options =>
            {
                options.SaveTokens = true;
                
                options.Events.OnTicketReceived = ctx =>
                {
                    return Task.CompletedTask;
                };
            });

            services.ConfigureCookieAuthenticationOptions();

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

        public static IServiceCollection AddOpenIdConnectAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddAuthentication()
                .AddOpenIdConnect(ApplicationConstants.Authentication.Schemes.OIDC, "OpenID Connect", options =>
                {
                    options.Authority = configuration["OpenIDConnectAuthentication:Authority"];
                    options.ResponseType = OpenIdConnectResponseType.Code;
                    options.ClientId = configuration["OpenIDConnectAuthentication:ClientId"];
                    options.ClientSecret = configuration["OpenIDConnectAuthentication:ClientSecret"];
                    options.CallbackPath = "/signin-plain-oidc";
                    options.SignedOutCallbackPath = "/signout-plain-oidc";
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                    
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("email");
                    options.Scope.Add("address");
                    options.Scope.Add("phone");
                    
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SaveTokens = true;
                    options.ClaimActions.MapAll();
                    
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = ClaimConstants.PreferredUserName,
                        RoleClaimType = ApplicationConstants.Authentication.Claims.Groups,
                        ValidateIssuer = true
                    };
                });
            
            services
                .AddOptions<OpenIdConnectOptions>(ApplicationConstants.Authentication.Schemes.OIDC)
                .Configure<SavedClaimsDictionary>((options, savedClaimsDictionary) =>
                {
                    options.Events.OnTicketReceived = ctx =>
                    {
                        if (ctx.Principal?.Identity is not ClaimsIdentity claimsIdentity || 
                            claimsIdentity.Name == null)
                        {
                            return Task.CompletedTask;
                        }

                        if (savedClaimsDictionary.ContainsKey(claimsIdentity.Name))
                        {
                            savedClaimsDictionary[claimsIdentity.Name] = ctx.Principal.Claims;
                        }
                        else
                        {
                            savedClaimsDictionary.Add(claimsIdentity.Name, ctx.Principal.Claims);
                        }

                        return Task.CompletedTask;
                    };
                });            
            
            services.ConfigureCookieAuthenticationOptions();
            
            return services;
        }
        
        public static IServiceCollection AddAzureAd(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMicrosoftIdentityWebAppAuthentication(configuration, "AzureAd", displayName: "Azure AD", subscribeToOpenIdConnectMiddlewareDiagnosticsEvents: true);

            services
                .AddOptions<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme)
                .Configure<SavedClaimsDictionary>((options, savedClaimsDictionary) =>
                {
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                
                    options.Events.OnSignedOutCallbackRedirect += ctx =>
                    {
                        return Task.CompletedTask;
                    };

                    options.Events.OnRemoteSignOut += ctx =>
                    {
                        return Task.CompletedTask;
                    };

                    options.ClaimActions.MapAll();
                
                    options.Events.OnTicketReceived = ctx =>
                    {
                        if (ctx.Principal?.Identity is not ClaimsIdentity claimsIdentity || 
                            claimsIdentity.Name == null)
                        {
                            return Task.CompletedTask;
                        }

                        if (savedClaimsDictionary.ContainsKey(claimsIdentity.Name))
                        {
                            savedClaimsDictionary[claimsIdentity.Name] = ctx.Principal.Claims;
                        }
                        else
                        {
                            savedClaimsDictionary.Add(claimsIdentity.Name, ctx.Principal.Claims);
                        }

                        return Task.CompletedTask;
                    };
                });

            services.ConfigureCookieAuthenticationOptions();
            
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

        private static IServiceCollection ConfigureCookieAuthenticationOptions(this IServiceCollection services)
        {
            services.TryAddSingleton<SavedClaimsDictionary>();
            
            services.Configure<CookieAuthenticationOptions>(IdentityConstants.ExternalScheme,
                options =>
                {
                    options.Events.OnSigningIn = ctx =>
                    {
                        return Task.CompletedTask;
                    };
                });

            services
                .AddOptions<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme)
                .Configure<SavedClaimsDictionary>((options, savedClaimsDictionary) =>
                {
                    options.Events.OnSigningIn = ctx =>
                    {
                        if (ctx.Principal?.Identity is not ClaimsIdentity claimsIdentity 
                            || claimsIdentity.Name == null
                            || !savedClaimsDictionary.TryGetValue(claimsIdentity.Name, out var savedClaims))
                        {
                            return Task.CompletedTask;
                        }

                        var newClaimsIdentity = new ClaimsIdentity(savedClaims, IdentityConstants.ExternalScheme);
                        ctx.Principal.AddIdentity(newClaimsIdentity);
                        savedClaimsDictionary.Remove(claimsIdentity.Name);
                        return Task.CompletedTask;
                    };
                });

            return services;
        }
    }
}