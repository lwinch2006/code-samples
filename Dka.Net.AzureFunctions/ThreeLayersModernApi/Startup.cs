using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ThreeLayersModernApi.Extensions;

[assembly: FunctionsStartup(typeof(ThreeLayersModernApi.Startup))]
namespace ThreeLayersModernApi;

public class Startup : FunctionsStartup
{
    private string _environmentName;
    private IConfiguration _configuration;
    
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services
            .AddSerilogLogger(_configuration, _environmentName);
    }
    
    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        var context = builder.GetContext();
        _environmentName = context.EnvironmentName;

        if (_environmentName.Equals("Development", StringComparison.OrdinalIgnoreCase))
        {
            builder.ConfigurationBuilder.AddUserSecrets<Startup>();
        }
        
        _configuration = builder.ConfigurationBuilder.Build();
    }    
}