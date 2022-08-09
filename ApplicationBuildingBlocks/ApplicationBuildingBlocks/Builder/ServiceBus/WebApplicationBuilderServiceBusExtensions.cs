namespace ApplicationBuildingBlocks.Builder.ServiceBus;

public static class WebApplicationBuilderServiceBusExtensions
{
	public static WebApplicationBuilder AddServiceBusServices(this WebApplicationBuilder builder)
	{
		// TODO - To be decided later if this would be a part of this package
		// builder.Services
		// 	.AddServiceBus(builder.Configuration);
		
		return builder;
	}
}