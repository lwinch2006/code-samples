using ApplicationBuildingBlocks.Builder;
using ApplicationBuildingBlocks.Builder.Authentication;

namespace WebUI;

public static class ProgramExtensions
{
	public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
	{
		builder
			.ConfigureDefaultBuilder()
			.Configure(() => { builder.Services.AddSingleton<IHostEnvironment>(builder.Environment); })
			.Configure(() => { builder.Services.AddFullIdentityWithDapper(builder.Configuration); })
			.Configure(() => { builder.Services.ConfigureAuthenticationCookies(); })
			.AddAuthenticationServicesFromProviders()
			.Configure(() => { builder.Services.AddApplication(); })
			.Configure(() => { builder.Services.AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<CreateTenantCommandValidator>()); })
			.Configure(() => { builder.Services.AddServiceBus(builder.Configuration); });
		
		return builder;
	}
}