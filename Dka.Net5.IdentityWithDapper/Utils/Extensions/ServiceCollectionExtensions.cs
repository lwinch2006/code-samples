using Dka.Net5.IdentityWithDapper.Infrastructure;
using Dka.Net5.IdentityWithDapper.Infrastructure.Mapping;
using Dka.Net5.IdentityWithDapper.Infrastructure.Repositories;
using Dka.Net5.IdentityWithDapper.Logic;
using Dka.Net5.IdentityWithDapper.Mapping;
using Dka.Net5.IdentityWithDapper.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Dka.Net5.IdentityWithDapper.Utils.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMinIdentityWithDapper(this IServiceCollection services)
        {
            services.AddInfrastructure();
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
        }

        public static void AddFullIdentityWithDapper(this IServiceCollection services)
        {
            services.AddInfrastructure();
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

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IInfrastructure, Infrastructure.Infrastructure>();

            services.AddAutoMapper(typeof(IdentityWithDapperProfile));
        }

        private static void AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
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