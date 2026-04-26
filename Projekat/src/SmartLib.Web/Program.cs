using Microsoft.AspNetCore.Authentication.Cookies;
using SmartLib.Core.Interfaces;
using SmartLib.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// SERVISI
// --------------------

builder.Services.AddControllersWithViews();

// Repository
builder.Services.AddScoped<IKorisnikRepository, KorisnikRepository>();

// Authentication (COOKIE - za Web)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/Login";

        // (sesija)
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

// Authorization 
builder.Services.AddAuthorization();

var app = builder.Build();

// --------------------
// MIDDLEWARE
// --------------------

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ROUTING
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();