using Microsoft.Extensions.DependencyInjection;
using Web.Mapping;

namespace Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddWeb(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(WebProfile));
        }
    }
}