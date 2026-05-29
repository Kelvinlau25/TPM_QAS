using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using TPM_QAS.DAL;
using TPM_QAS.Helpers;

namespace TPM_QAS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add MVC services
            builder.Services.AddControllersWithViews()
                .AddNewtonsoftJson();

            // Add session support
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                var timeoutMinutes = builder.Configuration.GetValue<int>("SessionTimeout", 30);
                options.IdleTimeout = TimeSpan.FromMinutes(timeoutMinutes);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Register IHttpContextAccessor for DI
            builder.Services.AddHttpContextAccessor();

            // Register IConfiguration as singleton (already available, but explicit for clarity)
            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

            var app = builder.Build();

            // Set configuration for Database class (replaces ConfigurationManager usage)
            Database.SetConfiguration(app.Configuration);

            // Set HttpContextAccessor for static access in non-controller classes
            var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
            HttpContextHelper.Configure(httpContextAccessor);

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
