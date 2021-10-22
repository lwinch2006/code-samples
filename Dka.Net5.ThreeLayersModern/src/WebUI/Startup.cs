using Application.Extensions;
using Application.Logic.Tenants.Commands;
using Dka.Net5.IdentityWithDapper.Utils.Extensions;
using FluentValidation.AspNetCore;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using WebUI.Mapping;
using WebUI.Middleware;

namespace WebUI
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }        
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(WebUIProfile));
            services.AddApplication();

            services
                .AddFullIdentityWithDapper(_configuration)
                .AddMicrosoftAuthentication(_configuration)
                .AddAzureAd(_configuration)
                .AddTwitterAuthentication(_configuration);

            services.ConfigureAuthenticationCookies();
            
            //CheckDebugInfo(services);
            
            services.AddApplicationInsightsTelemetry();
            
            services
                .AddMvc()
                .AddRazorRuntimeCompilation()
                .AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<CreateTenantCommandValidator>())
                .AddAzureAdUi();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, TelemetryConfiguration telemetryConfiguration)
        {
            if (env.IsDevelopment())
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
        }

        private void CheckDebugInfo(IServiceCollection services)
        {
            var sp = services.BuildServiceProvider();
            
            var schemeProvider = sp.GetRequiredService<IAuthenticationSchemeProvider>();
            var handlerProvider = sp.GetRequiredService<IAuthenticationHandlerProvider>();
            var authService = sp.GetRequiredService<IAuthenticationService>();

            var optionsMonitor = sp.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>();
            
            var schemes = schemeProvider.GetAllSchemesAsync().GetAwaiter().GetResult();

            var requestHandlerSchemes = schemeProvider.GetRequestHandlerSchemesAsync().GetAwaiter().GetResult();

            var cookieOptions1 = optionsMonitor.Get(IdentityConstants.ApplicationScheme);
            var cookieOptions2 = optionsMonitor.Get(IdentityConstants.ExternalScheme);
            
            var test = 0;
        }
    }
}