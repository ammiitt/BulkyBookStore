using Book.DataAccess.Data;
using Book.DataAccess.DbInitializer;
using Book.DataAccess.Repository;
using Book.DataAccess.Repository.IRepository;
using Book.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;

namespace BookStore
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            try
            {
                // Add services
                builder.Services.AddControllersWithViews();

                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

                builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

                builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

                builder.Services.ConfigureApplicationCookie(options =>
                {
                    options.LoginPath = $"/Identity/Account/Login";
                    options.LogoutPath = $"/Identity/Account/Logout";
                    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
                });

                builder.Services.AddAuthentication().AddFacebook(option =>
                {
                    option.AppId = "918050477677195";
                    option.AppSecret = "b5b58efd3c57407d9b92c70c0ef78cf8";
                    option.Scope.Add("email");
                });

                builder.Services.AddDistributedMemoryCache();

                builder.Services.AddSession(options =>
                {
                    options.IdleTimeout = TimeSpan.FromMinutes(100);
                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
                });

                builder.Services.AddScoped<IDbInitializer, DbInitializer>();
                builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
                builder.Services.AddScoped<IEmailSender, EmailSender>();

                builder.Services.AddRazorPages();

                var app = builder.Build();

                // Pipeline
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();

                // Stripe config
                StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

                app.UseRouting();

                app.UseAuthentication();
                app.UseAuthorization();
                app.UseSession();

                // 🔥 SAFE DB SEED
                try
                {
                    using (var scope = app.Services.CreateScope())
                    {
                        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                        dbInitializer.Initialize();
                    }
                }
                catch (Exception ex)
                {
                    System.IO.File.WriteAllText("seed_error.txt", ex.ToString());
                }

                app.MapRazorPages();

                app.MapControllerRoute(
                    name: "areas",
                    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}"
                );

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );

                app.Run();
            }
            catch (Exception ex)
            {
                // 🔴 GLOBAL STARTUP FAILURE LOG
                System.IO.File.WriteAllText("startup_error.txt", ex.ToString());
                throw;
            }




        }
    }
}
