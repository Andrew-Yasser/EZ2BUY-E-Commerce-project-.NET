using Ez2Buy.DataAccess.Contracts;
using Ez2Buy.DataAccess.Data;
using Ez2Buy.DataAccess.Repositories;
using Ez2Buy.Services.Contracts;
using Ez2Buy.Services.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Ez2Buy.Utility;
using Stripe;
using Ez2Buy.DataAccess.DbInitializer;

namespace Ez2BuyWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            //retrive the connection string from appsettings.json
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //inject the stripe settings
            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

            builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            // handle identity to go to right paths if user is not logged in
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });

            //add authentication for faceook,etc
            builder.Services.AddAuthentication().AddFacebook(option =>
            {
                option.AppId = "527508283572891";
                option.AppSecret = "45dc4bff335d44a644d207ed550bc692";
            });

            //add the session
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(100);  //set the session timeout if user is idle
                options.Cookie.HttpOnly = true;                    //this is to prevent the cookie from being accessed by javascript
                options.Cookie.IsEssential = true;                  //this is to make the session cookie essential(required) for the application
            });


            builder.Services.AddScoped<IDbInitializer, DbInitializer>();
            builder.Services.AddRazorPages();
            //register the repository
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            //register the email sender
            builder.Services.AddScoped<IEmailSender, EmailSender>();

            // Register Services




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
            StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            SeedDatabase(); //seed the database with initial data
            app.MapRazorPages();

            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

            app.Run();

            void SeedDatabase()
            {
                using (var scope = app.Services.CreateScope())
                {
                    var DbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                    DbInitializer.Initialize();
                }
            }
        }
    }
}
