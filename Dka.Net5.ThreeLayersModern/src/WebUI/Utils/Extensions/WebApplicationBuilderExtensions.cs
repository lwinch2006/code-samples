namespace WebUI.Utils.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder ConfigureBuilderLogger(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .CreateLogger(); 
        
        return builder;
    }

    public static WebApplicationBuilder ConfigureBuilderConfiguration(this WebApplicationBuilder builder)
    {
        if (builder.Environment.IsProduction())
        {
            var secretClient = new SecretClient(
                new Uri($"https://{builder.Configuration["KeyVault:Name"]}.vault.azure.net/"),
                new DefaultAzureCredential());

            builder.Configuration.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
        }

        return builder;
    }

    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddSingleton<IHostEnvironment>(builder.Environment)
            .AddAutoMapper(typeof(WebUIProfile))
            .AddApplication()
            .AddFullIdentityWithDapper(builder.Configuration)
            .AddMicrosoftAuthentication(builder.Configuration)
            .AddAzureAd(builder.Configuration)
            .AddTwitterAuthentication(builder.Configuration)
            .AddServiceBus(builder.Configuration)
            .ConfigureAuthenticationCookies()
            .AddApplicationInsightsTelemetry()        
            .AddMvc()
            .AddRazorRuntimeCompilation()
            .AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<CreateTenantCommandValidator>())
            .AddAzureAdUi();

        return builder;
    }

    public static WebApplicationBuilder ConfigureBuilderWebHost(this WebApplicationBuilder builder)
    {
        builder.WebHost.UseSerilog();

        if (builder.Environment.IsDevelopment())
        {
            var urls = builder.Configuration.GetSection("host:urls")
                .AsEnumerable()
                .Select(t => t.Value)
                .Where(t => t != null)
                .ToArray();

            builder.WebHost.UseUrls(urls);
        }

        return builder;
    }
}