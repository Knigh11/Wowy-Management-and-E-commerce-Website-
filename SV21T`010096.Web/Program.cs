using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.AspNetCore.Authentication.Cookies;
using SV21T1020096.Web.AppCodes;
using SV21T1020096.Web.Models;

namespace SV21T1020096.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            //Khởi tạo cấu hình cho BusinessLayer


            // Add services to the container.
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllersWithViews().AddMvcOptions(option =>
            {
                option.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            });
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
            {
                option.Cookie.Name = "AuthenticationCookie";
                option.LoginPath = "/Account/Login";
                option.AccessDeniedPath = "/Account/AccessDenined";
                option.ExpireTimeSpan = TimeSpan.FromDays(365);
                option.ClaimsIssuer = "SV21T1020096";
                option.Cookie.SameSite = SameSiteMode.Lax;
            });
            builder.Services.AddSession(option =>
            {
                option.IdleTimeout = TimeSpan.FromMinutes(60);
                option.Cookie.HttpOnly = true;
                option.Cookie.IsEssential = true;
            });
            builder.Services.AddAWSService<IAmazonS3>(new AWSOptions
            {
                Credentials = new BasicAWSCredentials(
                    builder.Configuration["AWS:AccessKey"],
                    builder.Configuration["AWS:SecretKey"]
                ),
                Region = RegionEndpoint.GetBySystemName(builder.Configuration["AWS:Region"])
            });
            builder.Services.AddScoped<IS3Service, S3Service>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            ApplicationContext.Configure
                (
                    context: app.Services.GetRequiredService<IHttpContextAccessor>(),
                    enviroment: app.Services.GetRequiredService<IWebHostEnvironment>()
                );

            string connectionString = builder.Configuration.GetConnectionString("LiteCommerceDB");
            SV21T1020096.BusinessLayers.Configuration.Initialize(connectionString!);
            app.Run();
        }
    }
}
