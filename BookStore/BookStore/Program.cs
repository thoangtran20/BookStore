using BookStore.DataAccess.Data;
using BookStore.DataAccess.DbInitializer;
using BookStore.DataAccess.Repository;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Utility;
using Humanizer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add services to the container.
services.AddControllersWithViews();
services.AddDbContext<ApplicationDbContext>(
        option=>option.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection")
     ));

services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

services.AddIdentity<IdentityUser, IdentityRole>().
    AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();


services.AddScoped<IUnitOfWork, UnitOfWork>();
services.AddScoped<IDbInitializer, DbInitializer>();
services.AddScoped<IEmailSender, EmailSender>();

services.AddRazorPages().AddRazorRuntimeCompilation();
services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = "750440449978339";
    options.AppSecret = "68676cd4f11a20eba060abc8f38f7f4f";
    options.CallbackPath = new PathString("/signin-facebook");
});
services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = "250509579896-a0vfdp89o3tp700t9pu4r3dm3e9jf525.apps.googleusercontent.com";
    options.ClientSecret = "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";
    options.CallbackPath = new PathString("/signin-google");
});

services.AddAuthentication().AddMicrosoftAccount(options =>
{
    options.ClientId = "c93ca76d-8c17-451e-84a3-27d7c8c33f16";
    options.ClientSecret = "f1a8Q~zKszvdZzf1nt~QkWZJJuB~QomTqHFGCcCh";
    options.CallbackPath = new PathString("/signin-microsoft");
});

// Configure authentication cookies, set options such as login path, cookie lifetime, and security options
services.ConfigureApplicationCookie(options =>
{
    // Set life time of cookie when login
    options.ExpireTimeSpan = TimeSpan.FromMinutes(120);
    // The cookie's lifetime will be renewed every time there is a new request from the user
    options.SlidingExpiration = true;
    // Mark cookies as important. This ensures that a cookie will be sent with each request, even if the user disables his or her cookies
    options.Cookie.IsEssential = true;
    // Set name of the authentication cookie. This is the name the browser will use to store and send the cookie
    options.Cookie.Name = "LoginTime";
    options.Cookie.Path = "/";
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
});

services.AddDistributedMemoryCache();
services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(100);
    option.Cookie.HttpOnly = true;  
    option.Cookie.IsEssential = true;   
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
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();
app.UseSession();
SeedDatabase();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();

void SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initialize();
    }
}
