var builder = WebApplication
    .CreateBuilder(args)
    .Configure();

var app = builder
    .Build()
    .Configure();

app.Logger.LogInformation("Host started {DateTime} UTC", DateTime.UtcNow);
app.Run();
app.Logger.LogInformation("Host finished {DateTime} UTC", DateTime.UtcNow);