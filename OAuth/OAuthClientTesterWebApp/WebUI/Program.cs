using Application.Extensions;
using Microsoft.AspNetCore.Mvc;
using OAuthClient.Extensions;
using WebUI.Models.Authorization;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.EnvironmentName.StartsWith("Development"))
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services
    .AddOAuthClient(builder.Configuration)
    .AddApplication(builder.Configuration)
    .AddAuthentication(AuthenticationConstants.AuthenticationScheme)
    .AddCookie(AuthenticationConstants.AuthenticationScheme, options =>
    {
        options.LoginPath = "/oauth/authorize";
    });


builder.Services.Configure<CookieTempDataProviderOptions>(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services
    .AddControllersWithViews()
    .AddRazorRuntimeCompilation();

if (builder.Environment.EnvironmentName.StartsWith("Development"))
{
    var urls = builder.Configuration.GetSection("host:urls")
        .AsEnumerable()
        .Select(t => t.Value)
        .Where(t => t != null)
        .ToArray();

    builder.WebHost.UseUrls(urls);
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.EnvironmentName.StartsWith("Development"))
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();