namespace ApplicationBuildingBlocks.Builder.Localization;

public static class WebApplicationBuilderLocalizationExtensions
{
	public static WebApplicationBuilder AddLocalizationServices(this WebApplicationBuilder builder)
	{
		builder.Services
			.AddLocalization();
		
		return builder;
	}
}