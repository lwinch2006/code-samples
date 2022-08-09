using ApplicationBuildingBlocks.Builder;
using ApplicationBuildingBlocks.Builder.Authentication;

namespace WebUI.Utils.Extensions;

public static class WebApplicationBuilderExtensions
{
	public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
	{
		builder
			.ConfigureDefaultBuilder()
			.AddAuthenticationServicesFromProviders()
			.Services
			.AddSingleton<IHostEnvironment>(builder.Environment)
			.AddFluentValidation(config =>
				config.RegisterValidatorsFromAssemblyContaining<CreateTenantCommandValidator>())
			.AddApplication()
			.AddServiceBus(builder.Configuration)
			.AddFullIdentityWithDapper(builder.Configuration)
			.ConfigureAuthenticationCookies();

		return builder;
	}
}