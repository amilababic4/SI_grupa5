using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
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

        // Remove surrounding single/double quotes if present (common in .env files)
        if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
            (value.StartsWith("'") && value.EndsWith("'")))
        {
            value = value[1..^1];
        }

        Environment.SetEnvironmentVariable(key, value);
    }
}

static string? BuildRedisConfiguration()
{
    var connString = Environment.GetEnvironmentVariable("UPSTASH_REDIS_CONNECTION_STRING");
    if (!string.IsNullOrWhiteSpace(connString))
        return connString;

    var redisUrl = Environment.GetEnvironmentVariable("UPSTASH_REDIS_URL");
    if (string.IsNullOrWhiteSpace(redisUrl))
        return null;

    if (!Uri.TryCreate(redisUrl, UriKind.Absolute, out var redisUri))
        return null;

    var password = Environment.GetEnvironmentVariable("UPSTASH_REDIS_PASSWORD");
    if (string.IsNullOrWhiteSpace(password) && !string.IsNullOrWhiteSpace(redisUri.UserInfo))
    {
        var parts = redisUri.UserInfo.Split(':', 2);
        if (parts.Length == 2)
            password = parts[1];
    }

    if (string.IsNullOrWhiteSpace(password))
        return null;

    var port = redisUri.Port > 0 ? redisUri.Port : 6379;
    return $"{redisUri.Host}:{port},password={password},ssl=True,abortConnect=False";
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
builder.Services.AddSingleton<CacheVersionStore>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Distributed Cache — Upstash Redis kad su dostupne TCP varijable, memorija lokalno kao fallback
var redisConfig = BuildRedisConfiguration();
if (!string.IsNullOrWhiteSpace(redisConfig))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConfig;
        options.InstanceName = "smartlib_web:";
    });
    Console.WriteLine("[Redis] Using Upstash Redis distributed cache");
}
else
{
    builder.Services.AddDistributedMemoryCache();
    Console.WriteLine("[Redis] Using in-memory distributed cache (missing Redis TCP credentials)");
}

// Repositories
builder.Services.AddScoped<IKorisnikRepository, KorisnikRepository>();
builder.Services.AddScoped<IKnjigaRepository, KnjigaRepository>();
builder.Services.AddScoped<IPrimjerakRepository, PrimjerakRepository>();
builder.Services.AddScoped<IKategorijaRepository, KategorijaRepository>();
builder.Services.AddScoped<IZaduzenjeRepository, ZaduzenjeRepository>();
builder.Services.AddScoped<IClanarinaRepository, ClanarinaRepository>();
builder.Services.AddScoped<IRezervacijaRepository, RezervacijaRepository>();
builder.Services.AddScoped<IForumRepository, ForumRepository>();
builder.Services.AddScoped<IRecenzijaRepository, RecenzijaRepository>();
builder.Services.AddScoped<IVijestRepository, VijestRepository>();
builder.Services.AddScoped<IDogadjajRepository, DogadjajRepository>();
builder.Services.AddHostedService<DeactivatedAccountCleanupService>();
builder.Services.AddScoped<IIzvjestajService, IzvjestajService>();

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

    try
    {
        db.Database.ExecuteSqlRaw(@"
            ALTER TABLE Knjige ADD COLUMN Opis TEXT NULL;
        ");
    }
    catch (Exception) { }

    try
    {
        db.Database.ExecuteSqlRaw(@"
            ALTER TABLE Knjige ADD COLUMN SlikaUrl VARCHAR(512) NULL;
        ");
    }
    catch (Exception) { }

    try
    {
        db.Database.ExecuteSqlRaw(@"
            CREATE TABLE IF NOT EXISTS ForumObjave (
                Id INT AUTO_INCREMENT PRIMARY KEY,
                Naslov VARCHAR(200) NOT NULL,
                Sadrzaj TEXT NOT NULL,
                Kategorija VARCHAR(100) NOT NULL,
                DatumKreiranja DATETIME(6) NOT NULL,
                Zakljucana BOOLEAN NOT NULL DEFAULT 0,
                KorisnikId INT NOT NULL,
                FOREIGN KEY (KorisnikId) REFERENCES Korisnici(Id) ON DELETE RESTRICT
            );
        ");
        db.Database.ExecuteSqlRaw(@"
            CREATE TABLE IF NOT EXISTS ForumKomentari (
                Id INT AUTO_INCREMENT PRIMARY KEY,
                Sadrzaj TEXT NOT NULL,
                DatumKreiranja DATETIME(6) NOT NULL,
                ObjavaId INT NOT NULL,
                KorisnikId INT NOT NULL,
                FOREIGN KEY (ObjavaId) REFERENCES ForumObjave(Id) ON DELETE CASCADE,
                FOREIGN KEY (KorisnikId) REFERENCES Korisnici(Id) ON DELETE RESTRICT
            );
        ");
        db.Database.ExecuteSqlRaw(@"
            CREATE TABLE IF NOT EXISTS ForumReakcije (
                Id INT AUTO_INCREMENT PRIMARY KEY,
                Tip VARCHAR(50) NOT NULL,
                DatumKreiranja DATETIME(6) NOT NULL,
                ObjavaId INT NOT NULL,
                KorisnikId INT NOT NULL,
                FOREIGN KEY (ObjavaId) REFERENCES ForumObjave(Id) ON DELETE CASCADE,
                FOREIGN KEY (KorisnikId) REFERENCES Korisnici(Id) ON DELETE RESTRICT
            );
        ");
        db.Database.ExecuteSqlRaw(@"
            CREATE TABLE IF NOT EXISTS Recenzije (
                Id INT AUTO_INCREMENT PRIMARY KEY,
                KnjigaId INT NOT NULL,
                KorisnikId INT NOT NULL,
                Ocjena INT NOT NULL,
                Komentar TEXT,
                DatumKreiranja DATETIME(6) NOT NULL,
                FOREIGN KEY (KnjigaId) REFERENCES Knjige(Id) ON DELETE CASCADE,
                FOREIGN KEY (KorisnikId) REFERENCES Korisnici(Id) ON DELETE RESTRICT
            );
        ");
    }
    catch (Exception) { }

    try
    {
        db.Database.ExecuteSqlRaw(@"
            CREATE TABLE IF NOT EXISTS Vijesti (
                Id INT AUTO_INCREMENT PRIMARY KEY,
                Naslov VARCHAR(300) NOT NULL,
                Sadrzaj LONGTEXT NOT NULL,
                Kategorija VARCHAR(100) NOT NULL DEFAULT 'Obavještenje',
                SlikaUrl VARCHAR(512) NULL,
                DatumObjave DATETIME(6) NOT NULL,
                AutorId INT NOT NULL,
                FOREIGN KEY (AutorId) REFERENCES Korisnici(Id) ON DELETE RESTRICT
            );
        ");
        db.Database.ExecuteSqlRaw(@"
            CREATE TABLE IF NOT EXISTS Dogadjaji (
                Id INT AUTO_INCREMENT PRIMARY KEY,
                Naslov VARCHAR(300) NOT NULL,
                Opis TEXT NULL,
                Datum DATETIME(6) NOT NULL,
                Sat VARCHAR(20) NULL,
                Lokacija VARCHAR(200) NULL,
                Kategorija VARCHAR(100) NOT NULL DEFAULT 'Edukacija',
                AutorId INT NOT NULL,
                FOREIGN KEY (AutorId) REFERENCES Korisnici(Id) ON DELETE RESTRICT
            );
        ");
    }
    catch (Exception) { }

    // Hardkodirani seed podataka (Opis i Slika) za knjige kako se ne bismo oslanjali na nepouzdan Google Books/OpenLibrary API
    db.Database.ExecuteSqlRaw(@"
        UPDATE Knjige SET Opis = 'A heartfelt novel about a flawed man reflecting on his life, marriages, and friendships in Montreal.' WHERE Isbn = '3404921038' AND Opis IS NULL;
        UPDATE Knjige SET Opis = 'A dramatic family saga spanning across generations during the turbulent times in Germany.' WHERE Isbn = '3442410665' AND Opis IS NULL;
        UPDATE Knjige SET Opis = 'An intense legal thriller where a lawyer defends a man against a seemingly impossible murder charge.' WHERE Isbn = '3442446937' AND Opis IS NULL;
        UPDATE Knjige SET Opis = 'A contemplative novel about a nun experiencing spiritual visions and a crisis of faith.', SlikaUrl = 'https://m.media-amazon.com/images/I/41KTRWfA7tL.jpg' WHERE Isbn = '0375406328' AND Opis IS NULL;
        UPDATE Knjige SET Opis = 'The unforgettable novel of a childhood in a sleepy Southern town and the crisis of conscience that rocked it.', SlikaUrl = 'https://m.media-amazon.com/images/I/81gepf1eMqL._AC_UF1000,1000_QL80_.jpg' WHERE Isbn = '0446310786' AND Opis IS NULL;
        UPDATE Knjige SET Opis = 'The true story of the undersized Depression-era racehorse whose victories lifted the spirits of the nation.', SlikaUrl = 'https://m.media-amazon.com/images/I/81rY1F7yJ1L._AC_UF1000,1000_QL80_.jpg' WHERE Isbn = '0449005615' AND Opis IS NULL;
        UPDATE Knjige SET Opis = 'A mother and her adopted Cherokee daughter flee across America to escape tribal law separating them.', SlikaUrl = 'https://m.media-amazon.com/images/I/81h6tXW47lL._AC_UF1000,1000_QL80_.jpg' WHERE Isbn = '0060168013' AND Opis IS NULL;
        UPDATE Knjige SET Opis = 'Miss Zukas, a pragmatic librarian, finds herself embroiled in a mysterious murder investigation.', SlikaUrl = 'https://m.media-amazon.com/images/I/51A1Z9W7WGL._AC_UF1000,1000_QL80_.jpg' WHERE Isbn = '038078243X' AND Opis IS NULL;
        UPDATE Knjige SET Opis = 'A classic romance novel depicting the emotional development of the protagonist, Elizabeth Bennet.', SlikaUrl = 'https://m.media-amazon.com/images/I/71Q1tPupKjL._AC_UF1000,1000_QL80_.jpg' WHERE Isbn = '055321215X' AND Opis IS NULL;
        UPDATE Knjige SET Opis = 'A comprehensive guide on energy healing techniques and the power of human touch.', SlikaUrl = 'https://m.media-amazon.com/images/I/71qZ8x+37hL._AC_UF1000,1000_QL80_.jpg' WHERE Isbn = '067176537X' AND Opis IS NULL;
        UPDATE Knjige SET Opis = 'A captivating story of love, ambition, and societal pressures set in the vibrant downtown scene.', SlikaUrl = 'https://m.media-amazon.com/images/I/81f1C+rR6lL._AC_UF1000,1000_QL80_.jpg' WHERE Isbn = '0061099686' AND Opis IS NULL;
        UPDATE Knjige SET Opis = 'A gripping thriller involving scientists trapped on a shrinking iceberg with an explosive device.', SlikaUrl = 'https://m.media-amazon.com/images/I/81L67qOaE-L._AC_UF1000,1000_QL80_.jpg' WHERE Isbn = '0553582909' AND Opis IS NULL;
        UPDATE Knjige SET Opis = 'A suspenseful mystery where a woman discovers a look-alike victim and uncovers a deep conspiracy.', SlikaUrl = 'https://m.media-amazon.com/images/I/81s4b4xWf+L._AC_UF1000,1000_QL80_.jpg' WHERE Isbn = '0671888587' AND Opis IS NULL;
        UPDATE Knjige SET Opis = 'A supernatural thriller following a prodigy boy pursued by a relentless and terrifying killer.', SlikaUrl = 'https://m.media-amazon.com/images/I/81U+yE+xL-L._AC_UF1000,1000_QL80_.jpg' WHERE Isbn = '0553582747' AND Opis IS NULL;
        UPDATE Knjige SET Opis = 'Dr. Kay Scarpetta investigates a bizarre murder involving dog hair and uncovers a deadly plot.', SlikaUrl = 'https://m.media-amazon.com/images/I/81w+r+kG8LL._AC_UF1000,1000_QL80_.jpg' WHERE Isbn = '0425182908' AND Opis IS NULL;
        UPDATE Knjige SET Opis = 'Eve Dallas investigates a series of murders committed by an extremist vigilante group.', SlikaUrl = 'https://m.media-amazon.com/images/I/81R6v-2P6qL._AC_UF1000,1000_QL80_.jpg' WHERE Isbn = '042518630X' AND Opis IS NULL;
        UPDATE Knjige SET Opis = 'Kultna i poučna priča o ježu koji hrabro brani svoj dom i slobodu od šumskih zvijeri.', SlikaUrl = 'https://svjetlostkomerc.ba/wp-content/uploads/2021/04/Jezeva-kucica-4.jpg' WHERE Isbn = '9789531713269' AND Opis IS NULL;
        UPDATE Knjige SET Opis = 'Potresno i snažno svjedočanstvo djevojčice koja se skriva od nacističkog progona u Amsterdamu.', SlikaUrl = 'https://znanje.hr/product-images/655655c6-b51e-450e-a4b5-12c8b02c8c4a.jpg' WHERE Isbn = '9780822203070' AND Opis IS NULL;
        UPDATE Knjige SET Opis = 'A hilarious pop-culture satire involving chocolate statues and modern commercialism.', SlikaUrl = 'https://m.media-amazon.com/images/I/51A2X4Z5P9L.jpg' WHERE Isbn = '0425163091' AND Opis IS NULL;
        UPDATE Knjige SET Opis = 'A complete and fascinating history of the rat and its incredible role in human civilization.', SlikaUrl = 'https://m.media-amazon.com/images/I/51A8C8C7EBL.jpg' WHERE Naslov LIKE '%More Cunning Than Man%' AND Opis IS NULL;
        UPDATE Knjige SET Opis = 'A quirky, brilliant collection of short fables that mix innocence with existential dread.', SlikaUrl = 'https://m.media-amazon.com/images/I/61k8wD1CqQL._AC_UF1000,1000_QL80_.jpg' WHERE Naslov LIKE '%The Middle Stories%' AND Opis IS NULL;
        UPDATE Knjige SET Opis = 'A gripping legal thriller centered around a vanished partner, missing millions, and corporate greed.', SlikaUrl = 'https://m.media-amazon.com/images/I/81w8v+3Zp5L._AC_UF1000,1000_QL80_.jpg' WHERE Naslov LIKE '%Pleading Guilty%' AND Opis IS NULL;
    ");

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

    var seedKategorija = db.Kategorije.FirstOrDefault(k => k.Naziv == "Beletristika")
        ?? db.Kategorije.FirstOrDefault();

    if (seedKategorija != null)
    {
        var seedBooks = new List<Knjiga>
        {
            new Knjiga
            {
                Naslov = "Way of the Peaceful Warrior 20th Anniversary Edition: A Book That Changes Lives",
                Autor = "Dan Millman",
                Isbn = "0915811898",
                Izdavac = "H.J. Kramer",
                GodinaIzdanja = 2000,
                SlikaUrl = "http://images.amazon.com/images/P/0915811898.01.LZZZZZZZ.jpg"
            },
            new Knjiga
            {
                Naslov = "Running from Safety: An Adventure of the Spirit",
                Autor = "Richard Bach",
                Isbn = "0385315287",
                Izdavac = "Delta",
                GodinaIzdanja = 1995,
                SlikaUrl = "http://images.amazon.com/images/P/0385315287.01.LZZZZZZZ.jpg"
            },
            new Knjiga
            {
                Naslov = "The Bridge Across Forever",
                Autor = "Richard Bach",
                Isbn = "0440108268",
                Izdavac = "Dell",
                GodinaIzdanja = 1986,
                SlikaUrl = "http://images.amazon.com/images/P/0440108268.01.LZZZZZZZ.jpg"
            },
            new Knjiga
            {
                Naslov = "A Gift of Wings",
                Autor = "Richard Bach",
                Isbn = "0440204321",
                Izdavac = "Dell",
                GodinaIzdanja = 1989,
                SlikaUrl = "http://images.amazon.com/images/P/0440204321.01.LZZZZZZZ.jpg"
            },
            new Knjiga
            {
                Naslov = "One",
                Autor = "Richard Bach",
                Isbn = "044020562X",
                Izdavac = "Dell",
                GodinaIzdanja = 1989,
                SlikaUrl = "http://images.amazon.com/images/P/044020562X.01.LZZZZZZZ.jpg"
            },
            new Knjiga
            {
                Naslov = "The Secret of Shambhala : In Search of the Eleventh Insight",
                Autor = "James Redfield",
                Isbn = "0446676489",
                Izdavac = "Warner Books",
                GodinaIzdanja = 2001,
                SlikaUrl = "http://images.amazon.com/images/P/0446676489.01.LZZZZZZZ.jpg"
            },
            new Knjiga
            {
                Naslov = "Maps to Ecstasy: A Healing Journey for the Untamed Spirit",
                Autor = "Gabrielle Roth",
                Isbn = "1577310454",
                Izdavac = "New World Library",
                GodinaIzdanja = 1998,
                SlikaUrl = "http://images.amazon.com/images/P/1577310454.01.LZZZZZZZ.jpg"
            },
            new Knjiga
            {
                Naslov = "Bridget Jones: The Edge of Reason",
                Autor = "Helen Fielding",
                Isbn = "0140298479",
                Izdavac = "Penguin Books",
                GodinaIzdanja = 2001,
                SlikaUrl = "http://images.amazon.com/images/P/0140298479.01.LZZZZZZZ.jpg"
            },
            new Knjiga
            {
                Naslov = "Invitation",
                Autor = "Oriah Mountain Dreamer",
                Isbn = "0694524638",
                Izdavac = "HarperAudio",
                GodinaIzdanja = 2000,
                SlikaUrl = "http://images.amazon.com/images/P/0694524638.01.LZZZZZZZ.jpg"
            },
            new Knjiga
            {
                Naslov = "Chief Seattle (Northwest Mythic Landscape Series)",
                Autor = "David Buerge",
                Isbn = "0912365579",
                Izdavac = "Pub Group West",
                GodinaIzdanja = 1992,
                SlikaUrl = "http://images.amazon.com/images/P/0912365579.01.LZZZZZZZ.jpg"
            },
            new Knjiga
            {
                Naslov = "Write from the Heart: Unleashing the Power of Your Creativity",
                Autor = "Hal Zina Bennett",
                Isbn = "1882591275",
                Izdavac = "Pub Group West",
                GodinaIzdanja = 1995,
                SlikaUrl = "http://images.amazon.com/images/P/1882591275.01.LZZZZZZZ.jpg"
            }
        };

        foreach (var seed in seedBooks)
        {
            if (db.Knjige.Any(k => k.Isbn == seed.Isbn))
            {
                continue;
            }

            seed.KategorijaId = seedKategorija.Id;
            db.Knjige.Add(seed);
        }

        db.SaveChanges();

        foreach (var seed in seedBooks)
        {
            var saved = db.Knjige.FirstOrDefault(k => k.Isbn == seed.Isbn);
            if (saved == null)
            {
                continue;
            }

            var inv = $"SEED-{saved.Isbn}-01";
            if (!db.Primjerci.Any(p => p.InventarniBroj == inv))
            {
                db.Primjerci.Add(new Primjerak
                {
                    KnjigaId = saved.Id,
                    InventarniBroj = inv,
                    Status = "dostupan",
                    DatumNabave = DateTime.UtcNow
                });
            }
        }

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

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// ROUTING
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Redis Health Check Endpoint
app.MapGet("/health/redis", async (IDistributedCache cache) =>
{
    try
    {
        var testKey = "health_check_web";
        var testValue = DateTime.UtcNow.ToString("o");
        await cache.SetStringAsync(testKey, testValue, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
        });

        var retrieved = await cache.GetStringAsync(testKey);
        var isRedis = !string.IsNullOrWhiteSpace(BuildRedisConfiguration());
        return Results.Ok(new
        {
            status = "Healthy",
            backend = isRedis ? "Upstash Redis" : "In-Memory",
            writeReadOk = retrieved == testValue,
            time = testValue
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, title: "Redis connection failed");
    }
});

app.Run();
