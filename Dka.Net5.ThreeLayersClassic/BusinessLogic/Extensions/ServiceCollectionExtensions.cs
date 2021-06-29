using BusinessLogic.Logic;
using BusinessLogic.Mapping;
using DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace BusinessLogic.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBusinessLogic(this IServiceCollection services)
        {
            services.AddDataAccess();
            services.SetupDataAccess();
            
            services.AddAutoMapper(typeof(BusinessLogicProfile));
            services.Scan(scan =>
            {
                scan
                    .FromAssemblyOf<ITenantsLogic>().AddClasses().UsingRegistrationStrategy(RegistrationStrategy.Skip).AsImplementedInterfaces().WithScopedLifetime()
                    .FromAssemblyOf<TenantsLogic>().AddClasses().UsingRegistrationStrategy(RegistrationStrategy.Skip).AsSelf().WithScopedLifetime();
            });            
        }

        private static void AddDataAccess(this IServiceCollection services)
        {
            services.Scan(scan =>
            {
                scan
                    .FromAssemblyOf<ITenantsRepository>().AddClasses().UsingRegistrationStrategy(RegistrationStrategy.Skip).AsImplementedInterfaces().WithScopedLifetime()
                    .FromAssemblyOf<TenantsRepository>().AddClasses().UsingRegistrationStrategy(RegistrationStrategy.Skip).AsSelf().WithScopedLifetime();
            });
        }

        private static void SetupDataAccess(this IServiceCollection services)
        {
            var migrator = services.BuildServiceProvider().GetService<IMigrator>();
            migrator.Run();
        }
    }
}