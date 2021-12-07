var webapp = WebApplication
    .CreateBuilder(args)
    .ConfigureBuilder()
    .Build()
    .ConfigureWebApp();

webapp.Logger.LogInformation("Host started {DateTime} UTC", DateTime.UtcNow);
webapp.Run();
webapp.Logger.LogInformation("Host finished {DateTime} UTC", DateTime.UtcNow);