using ApplicationBuildingBlocks.Builder.Authentication;
using ApplicationBuildingBlocks.Builder.Configuration;
using ApplicationBuildingBlocks.Builder.Localization;
using ApplicationBuildingBlocks.Builder.Logging;
using ApplicationBuildingBlocks.Builder.Mapping;
using ApplicationBuildingBlocks.Builder.Mvc;
using WebUI.ApplicationSetupExtensions.Builder.Authentication;

namespace ApplicationBuildingBlocks.Builder;

public static class WebApplicationBuilderExtensions
{
	public static WebApplicationBuilder ConfigureDefaultBuilder(this WebApplicationBuilder builder)
	{
		builder
			.ConfigureDefaultServices();

		return builder;
	}

	public static WebApplicationBuilder ConfigureDefaultServices(this WebApplicationBuilder builder)
	{
		builder
			.AddConfiguration()
			.AddLoggingServices()
			.AddAuthenticationServices()
			.AddLocalizationServices()
			.AddMappingServices()
			.AddMvcServices();

		return builder;
	}
	
	public static WebApplication CreateApp(this WebApplicationBuilder builder)
	{
		return builder.Build();
	}
	
	public static WebApplicationBuilder Configure(this WebApplicationBuilder builder, Action action)
	{
		action();
		return builder;
	}
}