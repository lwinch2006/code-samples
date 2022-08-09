namespace ApplicationBuildingBlocks.Builder.Mvc;

public static class MvcServices
{
	public static WebApplicationBuilder AddMvcServices(this WebApplicationBuilder builder)
	{
		builder.Services
			.AddApplicationInsightsTelemetry()
			.AddMvc()
			.AddRazorRuntimeCompilation();

		return builder;
	}
}