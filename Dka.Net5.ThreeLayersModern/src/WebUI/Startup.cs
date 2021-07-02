using Application.Extensions;
using Application.Logic.Tenants.Commands;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebUI.Mapping;

namespace WebUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            
        }        
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(WebUIProfile));
            services.AddApplication();
            services
                .AddMvc()
                .AddRazorRuntimeCompilation()
                .AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<CreateTenantCommandValidator>());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error/500");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/Error/{0}");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("areas", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();                
            });
        }
    }
}