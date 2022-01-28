namespace WebUI;
public static class ProgramAppExtensions
{
    public static WebApplication ConfigureWebApp(this WebApplication app)
    {
        app.Configure();
        return app;
    }
}