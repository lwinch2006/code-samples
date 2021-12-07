namespace WebUI.Utils.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication Configure(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            //app.UseDeveloperExceptionPage();
            app.UseExceptionHandler("/Error/500");
            var telemetryConfiguration = app.Services.GetRequiredService<TelemetryConfiguration>();
            telemetryConfiguration.DisableTelemetry = true;
        }
        else
        {
            app.UseExceptionHandler("/Error/500");
            app.UseHsts();
        }

        app
            .UseLogContextMiddleware()
            .UseStatusCodePagesWithReExecute("/Error/{0}")
            .UseHttpsRedirection()
            .UseStaticFiles()
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseEndpoints(endpoints => 
            {
                endpoints.MapControllerRoute("areas", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

        return app;
    }
}