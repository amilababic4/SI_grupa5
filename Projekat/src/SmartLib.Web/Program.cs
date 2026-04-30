using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;
using SmartLib.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// SERVISI
// --------------------

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();

// Repositories
builder.Services.AddScoped<IKorisnikRepository, KorisnikRepository>();
builder.Services.AddScoped<IKnjigaRepository, KnjigaRepository>();
builder.Services.AddScoped<IPrimjerakRepository, PrimjerakRepository>();
builder.Services.AddScoped<IKategorijaRepository, KategorijaRepository>();

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

// Auto-migrate i seed na startu
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    // Seed kategorija ako tabela prazna
    if (!db.Kategorije.Any())
    {
        db.Kategorije.AddRange(
            new Kategorija { Naziv = "Beletristika", Opis = "Romani, pripovijetke i ostala beletristika" },
            new Kategorija { Naziv = "Naučna fantastika", Opis = "SF i fantastična književnost" },
            new Kategorija { Naziv = "Historija", Opis = "Historijska literatura i memoari" },
            new Kategorija { Naziv = "Nauka i tehnika", Opis = "Naučno-stručna literatura" },
            new Kategorija { Naziv = "Filozofija", Opis = "Filozofska i društvena literatura" },
            new Kategorija { Naziv = "Biografija", Opis = "Biografije i autobiografije" },
            new Kategorija { Naziv = "Dječija literatura", Opis = "Knjige za djecu i mlade" },
            new Kategorija { Naziv = "Udžbenici", Opis = "Obrazovni udžbenici i priručnici" },
            new Kategorija { Naziv = "Ostalo", Opis = "Ostale kategorije" }
        );
        db.SaveChanges();
    }
}

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
