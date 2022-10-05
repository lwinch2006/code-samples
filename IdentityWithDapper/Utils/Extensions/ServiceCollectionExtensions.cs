using System;
using IdentityWithDapper.Infrastructure;
using IdentityWithDapper.Infrastructure.Mapping;
using IdentityWithDapper.Infrastructure.Repositories;
using IdentityWithDapper.Infrastructure.Utils;
using IdentityWithDapper.Infrastructure.Utils.Constants;
using IdentityWithDapper.Logic;
using IdentityWithDapper.Mapping;
using IdentityWithDapper.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityWithDapper.Utils.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMinIdentityWithDapper(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddInfrastructure(configuration);
            services.SetupInfrastructure();
            
            services
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                })
                .AddDefaultTokenProviders()
                .AddDefaultUI();

            services.AddTransient<IUserStore<ApplicationUser>, ApplicationUserStoreMin>();
            services.AddTransient<IRoleStore<ApplicationRole>, ApplicationRoleStoreMin>();
            services.AddAutoMapper(typeof(IdentityWithDapperProfile));

            return services;
        }

        public static IServiceCollection AddFullIdentityWithDapper(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddInfrastructure(configuration);
            services.SetupInfrastructure();
            
            services
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                })
                .AddDefaultTokenProviders()
                .AddDefaultUI();

            services.AddTransient<IUserStore<ApplicationUser>, ApplicationUserStoreFull>();
            services.AddTransient<IRoleStore<ApplicationRole>, ApplicationRoleStoreFull>();
            services.AddAutoMapper(typeof(IdentityWithDapperProfile));

            return services;
        }

        public static IServiceCollection ConfigureAuthenticationCookies(this IServiceCollection services)
        {
            services.Configure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, o =>
            {
                o.LogoutPath = new PathString("/Identity/Account/SignOut");
            });

            return services;
        }
        
        private static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var dbType = Enum.Parse<InfrastructureConstants.DbTypes>(configuration[InfrastructureConstants.DbTypeConfigParamName]);
            
            switch (dbType)
            {
                case InfrastructureConstants.DbTypes.Sqlite:
                    services.AddScoped<IUserRepository, Infrastructure.Repositories.SQLite.UserRepository>();
                    services.AddScoped<IRoleRepository, Infrastructure.Repositories.SQLite.RoleRepository>();
                    services.AddScoped<IUserTokenRepository, Infrastructure.Repositories.SQLite.UserTokenRepository>();
                    services.AddScoped<IUserClaimRepository, Infrastructure.Repositories.SQLite.UserClaimRepository>();
                    services.AddScoped<IUserLoginRepository, Infrastructure.Repositories.SQLite.UserLoginRepository>();
                    services.AddScoped<IRoleClaimRepository, Infrastructure.Repositories.SQLite.RoleClaimRepository>();
                    break;
                
                case InfrastructureConstants.DbTypes.Mssql:
                default:
                    services.AddScoped<IUserRepository, Infrastructure.Repositories.MSSQL.UserRepository>();
                    services.AddScoped<IRoleRepository, Infrastructure.Repositories.MSSQL.RoleRepository>();
                    services.AddScoped<IUserTokenRepository, Infrastructure.Repositories.MSSQL.UserTokenRepository>();
                    services.AddScoped<IUserClaimRepository, Infrastructure.Repositories.MSSQL.UserClaimRepository>();
                    services.AddScoped<IUserLoginRepository, Infrastructure.Repositories.MSSQL.UserLoginRepository>();
                    services.AddScoped<IRoleClaimRepository, Infrastructure.Repositories.MSSQL.RoleClaimRepository>();
                    break;                    
            }            
            
            services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
            services.AddScoped<IInfrastructure, Infrastructure.Infrastructure>();            
            services.AddAutoMapper(typeof(InfrastructureProfile));
        }

        private static void SetupInfrastructure(this IServiceCollection services)
        {
            var infrastucture = services.BuildServiceProvider().GetService<IInfrastructure>();
            infrastucture.Setup();
        }
    }
}