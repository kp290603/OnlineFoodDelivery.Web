using Microsoft.EntityFrameworkCore;
using OnlineFoodDelivery.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using OnlineFoodDelivery.Web.Models;
using Stripe;

namespace OnlineFoodDelivery.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationDbContextConnection")));

            builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders().AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddScoped<IDbIntializer, DbIntializer>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddRazorPages();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.ConfigureApplicationCookie(config =>
            {
                config.LoginPath = "/Identity/Account/Login";
            });
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            

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
            DataSeeding();
            app.UseRouting();
            app.UseAuthentication();
            StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
            app.UseAuthorization();
            app.UseSession();
            app.MapRazorPages();
            app.MapControllerRoute(
                name: "default",
                //pattern: "{area=Customer}/{controller=Carts}/{action=Index}/{id?}");
                pattern: "{area=customer}/{controller=Homes}/{action=Home}/{id?}");
                //pattern: "{area=admin}/{controller=categories}/{action=Index}/{id?}");
                //pattern: "{controller=Home}/{action=Index}/{id?}");
            app.Run();
            void DataSeeding()
            {
                using (var scope = app.Services.CreateScope())
                {
                    var DbInitializer = scope.ServiceProvider.GetRequiredService<IDbIntializer>();
                    DbInitializer.Initialize();
                }
            }
        }
    }
}
