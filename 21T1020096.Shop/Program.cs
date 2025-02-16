using Microsoft.AspNetCore.Authentication.Cookies;
using SV21T1020096.Shop.AppCodes;
using SV21T1020096.Shop.Models;

namespace SV21T1020096.Shop
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
            builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
            builder.Services.AddScoped<ShoppingCartService>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            //app.Use(async (context, next) =>
            //{
            //    context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            //    context.Response.Headers["Expires"] = "-1";
            //    context.Response.Headers["Pragma"] = "no-cache";
            //    await next();
            //});
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
