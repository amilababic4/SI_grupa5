using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;
using SmartLib.Infrastructure.Repositories;
using SmartLib.Infrastructure.Services;

// Load .env file if it exists (for local development)
string[] possiblePaths = {
    Path.Combine(Directory.GetCurrentDirectory(), ".env"),
    Path.Combine(Directory.GetCurrentDirectory(), "Projekat", ".env"),
    Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env"),
    Path.Combine(Directory.GetCurrentDirectory(), "..", ".env")
};
var envPath = possiblePaths.FirstOrDefault(File.Exists);

if (envPath != null)
{
    foreach (var line in File.ReadAllLines(envPath))
    {
        if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
            continue;
        var idx = line.IndexOf('=');
        if (idx <= 0) continue;
        var key = line[..idx].Trim();
        var value = line[(idx + 1)..].Trim();
        Environment.SetEnvironmentVariable(key, value);
    }
}

var builder = WebApplication.CreateBuilder(args);

// --------------------
// SERVISI
// --------------------

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

// Repositories
builder.Services.AddScoped<IKorisnikRepository, KorisnikRepository>();
builder.Services.AddScoped<IKnjigaRepository, KnjigaRepository>();
builder.Services.AddScoped<IPrimjerakRepository, PrimjerakRepository>();
builder.Services.AddScoped<IKategorijaRepository, KategorijaRepository>();
builder.Services.AddScoped<IZaduzenjeRepository, ZaduzenjeRepository>();
builder.Services.AddScoped<IClanarinaRepository, ClanarinaRepository>();
builder.Services.AddHostedService<DeactivatedAccountCleanupService>();

// Services
builder.Services.AddTransient<IEmailService, SmartLib.Infrastructure.Services.EmailService>();
builder.Services.AddScoped<IBookRecommender, SmartLib.Infrastructure.Services.BookRecommender>();

// Authentication (COOKIE - za Web)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/Login";

        // (sesija)
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
    });

// Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// Auto-migrate i seed na startu
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();

    // Manually add ResetToken columns if they don't exist (needed because EnsureCreated doesn't update schema)
    try
    {
        db.Database.ExecuteSqlRaw(@"
            ALTER TABLE Korisnici ADD COLUMN ResetToken VARCHAR(256) NULL;
        ");
    }
    catch (Exception)
    {
        // Column already exists — safe to ignore
    }

    try
    {
        db.Database.ExecuteSqlRaw(@"
            ALTER TABLE Korisnici ADD COLUMN ResetTokenExpiry DATETIME(6) NULL;
        ");
    }
    catch (Exception)
    {
        // Column already exists — safe to ignore
    }

    try
    {
        db.Database.ExecuteSqlRaw(@"
            ALTER TABLE Korisnici ADD COLUMN DatumDeaktivacije DATETIME(6) NULL;
        ");
    }
    catch (Exception)
    {
        // Column already exists — safe to ignore
    }

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

    // Seed demo knjiga/primjerak ako nema dostupnih primjeraka (za UI tokove)
    if (!db.Primjerci.Any(p => p.Status == "dostupan"))
    {
        var kategorija = db.Kategorije.FirstOrDefault();
        if (kategorija == null)
        {
            kategorija = new Kategorija { Naziv = "Demo", Opis = "Demo kategorija" };
            db.Kategorije.Add(kategorija);
            db.SaveChanges();
        }

        const string demoIsbn = "9780000000000";
        var knjiga = db.Knjige.FirstOrDefault(k => k.Isbn == demoIsbn);
        if (knjiga == null)
        {
            knjiga = new Knjiga
            {
                Naslov = "Demo knjiga",
                Autor = "Demo autor",
                Isbn = demoIsbn,
                KategorijaId = kategorija.Id,
                GodinaIzdanja = DateTime.UtcNow.Year
            };
            db.Knjige.Add(knjiga);
            db.SaveChanges();
        }

        var inv = $"INV-{knjiga.Id}-001";
        if (!db.Primjerci.Any(p => p.InventarniBroj == inv))
        {
            db.Primjerci.Add(new Primjerak
            {
                KnjigaId = knjiga.Id,
                InventarniBroj = inv,
                Status = "dostupan",
                DatumNabave = DateTime.UtcNow
            });
            db.SaveChanges();
        }
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
