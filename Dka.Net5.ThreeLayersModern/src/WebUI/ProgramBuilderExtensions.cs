namespace WebUI;

public static class ProgramBuilderExtensions
{
    public static WebApplicationBuilder Configure(this WebApplicationBuilder builder)
    {
        BuildLogger(builder.Environment.EnvironmentName, builder.Configuration);

        builder.Services.AddSingleton<IHostEnvironment>(builder.Environment);
        
        if (builder.Environment.IsProduction())
        {
            var secretClient = new SecretClient(
                new Uri($"https://{builder.Configuration["KeyVault:Name"]}.vault.azure.net/"),
                new DefaultAzureCredential());

            builder.Configuration.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
        }

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

        builder.Services
            .AddAutoMapper(typeof(WebUIProfile))
            .AddApplication()
            .AddFullIdentityWithDapper(builder.Configuration)
            .AddMicrosoftAuthentication(builder.Configuration)
            .AddAzureAd(builder.Configuration)
            .AddTwitterAuthentication(builder.Configuration)
            .ConfigureAuthenticationCookies()
            .AddApplicationInsightsTelemetry()        
            .AddMvc()
            .AddRazorRuntimeCompilation()
            .AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<CreateTenantCommandValidator>())
            .AddAzureAdUi();

        return builder;
    }
        
    private static void BuildLogger(string environmentName, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.WithProperty("Environment", environmentName)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .CreateLogger();            
    }        
}