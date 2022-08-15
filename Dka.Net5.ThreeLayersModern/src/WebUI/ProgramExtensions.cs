using ApplicationBuildingBlocks.Builder;
using ApplicationBuildingBlocks.Builder.Authentication;

namespace WebUI;

public static class ProgramExtensions
{
	public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
	{
		builder
			.ConfigureDefaultBuilder()
			.ConfigureBuilderServices(services => services.AddSingleton<IHostEnvironment>(builder.Environment))
			.ConfigureBuilderServices(services => services.AddFullIdentityWithDapper(builder.Configuration))
			.ConfigureBuilderServices(services => services.ConfigureAuthenticationCookies())
			.AddAuthenticationServicesFromProviders()
			.ConfigureBuilderServices(services => services.AddApplication())
			.ConfigureBuilderServices(services => services.AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<CreateTenantCommandValidator>()))
			.ConfigureBuilderServices(services => services.AddServiceBus(builder.Configuration));
		
		return builder;
	}
}