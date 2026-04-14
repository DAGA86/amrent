using AMRent.BackOffice.Extensions;
using LinqKit;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;

namespace AMRent.BackOffice
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
                        options.AccessDeniedPath = "/Home/Forbidden";
                    }
                );

                // Identity and Authorization
                builder.Services.AddScoped<dCore.Identity.Providers.IUser, dCore.Identity.Providers.User>();
                builder.Services.AddScoped<dCore.Identity.Providers.IRole, dCore.Identity.Providers.Role>();
                builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
                builder.Services.AddScoped<IPermissionService, PermissionService>();

                builder.Services.AddAuthorization(options =>
                {
                    options.AddPolicy("PermissionPolicy", policy =>
                    {
                        policy.RequireAuthenticatedUser();
                    });
                });

                // Add services to the container.
                builder.Services.AddControllersWithViews();
                
                builder.Services.Configure<FormOptions>(options =>
                {
                    options.ValueCountLimit = int.MaxValue; // número máximo de campos
                    options.MultipartBodyLengthLimit = long.MaxValue; // uploads grandes
                    options.BufferBodyLengthLimit = long.MaxValue;
                });

                builder.Services.AddScoped<dCore.MultiLanguage.Providers.TranslationProvider>();

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
                    MinimumSameSitePolicy = SameSiteMode.Strict,
                };
                app.UseCookiePolicy(cookiePolicyOptions);
                app.UseAuthorization();

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                if (app.Environment.IsDevelopment() || dCore.Licensing.Actions.IsValid(builder.Configuration["License"]))
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