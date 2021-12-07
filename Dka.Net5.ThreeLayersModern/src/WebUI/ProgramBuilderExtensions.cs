namespace WebUI;

public static class ProgramBuilderExtensions
{
    public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
    {
        builder
            .ConfigureBuilderConfiguration()
            .ConfigureBuilderLogger()
            .ConfigureServices()
            .ConfigureBuilderWebHost();

        return builder;
    }
}