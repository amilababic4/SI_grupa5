// SmartLib API — Entry Point

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartLib.Core.Interfaces;
using SmartLib.Infrastructure.Data;
using SmartLib.Infrastructure.Repositories;
using SmartLib.Infrastructure.Services;

// Load .env file if it exists (for local development)
var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
if (!File.Exists(envPath))
    envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env");
if (File.Exists(envPath))
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

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<CacheVersionStore>();

builder.Services.AddScoped<IKorisnikRepository, KorisnikRepository>();
builder.Services.AddScoped<IKorisnikRepository, KorisnikRepository>();
builder.Services.AddScoped<IKnjigaRepository, KnjigaRepository>();
builder.Services.AddScoped<IPrimjerakRepository, PrimjerakRepository>();
builder.Services.AddScoped<IKategorijaRepository, KategorijaRepository>();
builder.Services.AddScoped<IZaduzenjeRepository, ZaduzenjeRepository>();
builder.Services.AddHostedService<DeactivatedAccountCleanupService>();

// JWT Authentication - US-07, US-08
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration.GetSection("Jwt");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,

            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt["Key"]!)
            )
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();

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
}

// --------------------
// MIDDLEWARE
// --------------------

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
