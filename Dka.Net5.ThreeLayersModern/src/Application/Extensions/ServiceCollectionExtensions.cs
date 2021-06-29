using System.Reflection;
using Application.Mapping;
using FluentValidation;
using Infrastructure;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddInfrastructure();
            services.SetupInfrastructure();
            
            services.AddAutoMapper(typeof(ApplicationProfile));
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
        }

        private static void AddInfrastructure(this IServiceCollection services)
        {
            services.Scan(scan =>
            {
                scan
                    .FromAssemblyOf<ITenantsRepository>().AddClasses().UsingRegistrationStrategy(RegistrationStrategy.Skip).AsImplementedInterfaces().WithScopedLifetime()
                    .FromAssemblyOf<TenantsRepository>().AddClasses().UsingRegistrationStrategy(RegistrationStrategy.Skip).AsSelf().WithScopedLifetime();
            });
        }

        private static void SetupInfrastructure(this IServiceCollection services)
        {
            var infrastucture = services.BuildServiceProvider().GetService<IInfrastructure>();
            infrastucture.Setup();
        }
    }
}