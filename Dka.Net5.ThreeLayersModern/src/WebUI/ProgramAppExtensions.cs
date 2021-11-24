namespace WebUI;
public static class ProgramAppExtensions
{
    public static WebApplication Configure(this WebApplication app)
    {
        var telemetryConfiguration = app.Services.GetRequiredService<TelemetryConfiguration>();

        if (app.Environment.IsDevelopment())
        {
            //app.UseDeveloperExceptionPage();
            app.UseExceptionHandler("/Error/500");
            telemetryConfiguration.DisableTelemetry = true;
        }
        else
        {
            app.UseExceptionHandler("/Error/500");
            app.UseHsts();
        }

        app.UseLogContextMiddleware();
        app.UseStatusCodePagesWithReExecute("/Error/{0}");
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute("areas", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            endpoints.MapRazorPages();
        });

        return app;
    }
}