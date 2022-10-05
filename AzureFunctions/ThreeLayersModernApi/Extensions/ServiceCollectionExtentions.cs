using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Exceptions;

namespace ThreeLayersModernApi.Extensions;

public static class ServiceCollectionExtentions
{
    public static IServiceCollection AddSerilogLogger(this IServiceCollection services, IConfiguration configuration, string environmentName)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.WithProperty("Environment", environmentName)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .CreateLogger();
        
        services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());

        return services;
    }    
}