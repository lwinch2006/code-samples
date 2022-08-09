using Serilog;
using Serilog.Exceptions;

namespace ApplicationBuildingBlocks.Builder.Logging;

public static class WebApplicationBuilderLoggingExtensions
{
	public static WebApplicationBuilder AddLoggingServices(this WebApplicationBuilder builder)
	{
		builder
			.AddSerilogServices();
		
		return builder;
	}

	public static WebApplicationBuilder AddSerilogServices(this WebApplicationBuilder builder)
	{
		Log.Logger = new LoggerConfiguration()
			.ReadFrom.Configuration(builder.Configuration)
			.Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
			.Enrich.FromLogContext()
			.Enrich.WithExceptionDetails()
			.CreateLogger();

		builder.WebHost.UseSerilog();
        
		return builder;
	}
}