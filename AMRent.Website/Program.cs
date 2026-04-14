using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Web;
using AMRent.Website.Extensions;

namespace AMRent.Website
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = LogManager.Setup()
                .LoadConfigurationFromAppSettings()
                .GetCurrentClassLogger();

            logger.Debug("App starting...");

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add Database
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                builder.Services.AddDbContext<Data.Contexts.FullDatabaseContext>(options =>
                    options.UseSqlServer(connectionString));
                builder.Services.AddDbContext<dCore.Identity.Contexts.IdentityContext>(options =>
                    options.UseSqlServer(connectionString));
                builder.Services.AddDbContext<dCore.MultiLanguage.Contexts.TranslationContext>(options =>
                    options.UseSqlServer(connectionString));

                // Authentication
                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, (options) =>
                    {
                        options.LoginPath = "/Account/Login";
                        options.LogoutPath = "/Account/Logout";
                        options.ExpireTimeSpan = TimeSpan.FromHours(8);
                        options.AccessDeniedPath = "/Forbidden/";
                    }
                );

                // Add services to the container.
                builder.Services.AddControllersWithViews();
                builder.Services.AddSession(options =>
                {
                    options.IdleTimeout = TimeSpan.FromHours(8);
                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
                });

                builder.Services.AddScoped<dCore.MultiLanguage.Providers.TranslationProvider>();
                builder.Services.AddHttpClient<Shared.Providers.PayPal>();

                // NLog: Setup NLog for Dependency injection
                builder.Logging.ClearProviders();
                builder.Host.UseNLog();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();

                app.UseRouting();

                app.UseAuthentication();
                var cookiePolicyOptions = new CookiePolicyOptions
                {
                    MinimumSameSitePolicy = SameSiteMode.Strict
                };
                app.UseCookiePolicy(cookiePolicyOptions);
                app.UseAuthorization();
                app.UseSession();

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{segmentId?}");

                app.Run();
            }
            catch (Exception exception)
            {
                // NLog: catch setup errors
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }
    }
}