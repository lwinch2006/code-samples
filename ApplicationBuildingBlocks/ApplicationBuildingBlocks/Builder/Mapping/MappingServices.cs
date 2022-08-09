namespace ApplicationBuildingBlocks.Builder.Mapping;

public static class MappingServices
{
	public static WebApplicationBuilder AddMappingServices(this WebApplicationBuilder builder)
	{
		builder.AddAutoMapperServices();

		return builder;
	}

	public static WebApplicationBuilder AddAutoMapperServices(this WebApplicationBuilder builder)
	{
		builder.Services
			.AddAutoMapper(builder.GetType());

		return builder;
	}
}