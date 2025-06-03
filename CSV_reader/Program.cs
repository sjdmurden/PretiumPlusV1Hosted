using System;
using CsvHelper;
using System.IO;
using System.Globalization;
using System.Linq;
using ClosedXML.Excel;
using CSV_reader.Services;
using CSV_reader.database;
using Microsoft.EntityFrameworkCore;
using CSV_reader.Configurations;
using QuestPDF.Infrastructure;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using static CSV_reader.Services.ExcelFileService;
using Org.BouncyCastle.Crypto.Engines;
using Serilog;


namespace CSV_reader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            /*string adminPass = BCrypt.Net.BCrypt.HashPassword("adminPassword");
            string normalPass = BCrypt.Net.BCrypt.HashPassword("normalPassword");

            Console.WriteLine("User1 Password Hash: " + adminPass);
            Console.WriteLine("User2 Password Hash: " + normalPass);*/

            /*string bobPass = BCrypt.Net.BCrypt.HashPassword("bobPassword");
            Console.WriteLine("Bob password hash: " + bobPass);*/
            // $2a$11$5z2CK8UrBddxdMJlbrt0Ye1uz3zqYJhgQjJOmT9RJPUjsUja4siWu


            var builder = WebApplication.CreateBuilder(args);

            // Configure serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/app-log.txt", rollingInterval: RollingInterval.Day)  //
                .CreateLogger();
            builder.Host.UseSerilog();
            
            builder.Services.AddSession();

            QuestPDF.Settings.License = LicenseType.Community;

            // builder.Services.Configure<ExcelFileSettings>(builder.Configuration.GetSection("ExcelFileSettings"));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add Cookie-based Authentication
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Login/Login"; // redirect path to your login page if user isn't logged in
                    // options.AccessDeniedPath = "/Login/Login"; an authorised user will be redirected here if they try and access a
                                                               // page they are restircted from. ie a non-admin trying to access admin only pages
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Set expiration time (e.g., 30 minutes)
                    options.SlidingExpiration = true; // Prevent cookie extension
                });

            // register services
            builder.Services.AddScoped<IClaimsService, ClaimsService>();
            builder.Services.AddScoped<ICalculationsService, CalculationsService>();
            builder.Services.AddScoped<IPDFService, PDFService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IGetHistoricDataForQuoteSearchService, GetHistoricDataForQuoteSearchService>();
            builder.Services.AddScoped<IExcelFileService, ExcelFileService>();
            builder.Services.AddScoped<IClaimsCalculationsService, ClaimsCalculationsService>();
            builder.Services.AddMemoryCache();

            builder.Services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            var app = builder.Build();


            


            // load appsettings.json into configuration
            var configuration = builder.Configuration;
            // Initialise the cache with the current filepath ie empty string ""
            // AppSettingsCache.ExcelFilePath = configuration["ExcelFilePath"]; 




            app.UseRouting();
            
            app.UseSession();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            
                                                     
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseAuthorization();
            


            // Define the route for your controllers
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Login}/{action=Login}/{id?}"
            );

            app.MapControllerRoute(
                name: "excel",
                pattern: "{controller=Excel}/{action=IndexExcel}/{id?}"
            );

            app.MapControllerRoute(
                name: "calculations",
                pattern: "{controller=Calculations}/{action=IndexCalculations}/{quoteId?}"
            );

           
            // this stuff was used to try and get the excel file and file path cashe to be deleted on app shutdown

            /*var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
            var excelFileService = app.Services.GetRequiredService<IExcelFileService>();

            // Register a cleanup action when the application is stopping
            lifetime.ApplicationStopping.Register(() =>
            {
                Log.Information("Application is stopping...");
                excelFileService.CleanupOnShutdown();
            });

            lifetime.ApplicationStopped.Register(() =>
            {
                Log.Information("Application has stopped.");
                Log.CloseAndFlush(); // Ensures logs are written before exit
            });

            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                Log.Information("Process is exiting...");
                Log.CloseAndFlush();
            };*/

            app.Run();

        }
    }
}
