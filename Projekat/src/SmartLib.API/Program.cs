// SmartLib API — Entry Point

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
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

        // Remove surrounding single/double quotes if present (common in .env files)
        if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
            (value.StartsWith("'") && value.EndsWith("'")))
        {
            value = value[1..^1];
        }

        Environment.SetEnvironmentVariable(key, value);
    }
}

static string? BuildRedisConfiguration() => SmartLib.Infrastructure.Services.RedisConfigHelper.BuildRedisConfiguration();

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

// Distributed Cache — auto-detect Redis, fallback to in-memory
var redisConfig = BuildRedisConfiguration();
if (!string.IsNullOrWhiteSpace(redisConfig))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConfig;
        options.InstanceName = "smartlib:";
    });
    Console.WriteLine("[Cache] Using Upstash Redis distributed cache");
}
else
{
    builder.Services.AddDistributedMemoryCache();
    Console.WriteLine("[Cache] Using in-memory distributed cache (no Redis credentials)");
}

builder.Services.AddScoped<IKorisnikRepository, KorisnikRepository>();
builder.Services.AddScoped<IKorisnikRepository, KorisnikRepository>();
builder.Services.AddScoped<IKnjigaRepository, KnjigaRepository>();
builder.Services.AddScoped<IPrimjerakRepository, PrimjerakRepository>();
builder.Services.AddScoped<IKategorijaRepository, KategorijaRepository>();
builder.Services.AddScoped<IZaduzenjeRepository, ZaduzenjeRepository>();
builder.Services.AddScoped<IRezervacijaRepository, RezervacijaRepository>();
builder.Services.AddHostedService<DeactivatedAccountCleanupService>();
builder.Services.AddSingleton<SingleFlightCache>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<AuditLogService>();


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

// Health Check Endpoint
app.MapGet("/api/health/redis", () =>
{
    var isRedis = !string.IsNullOrWhiteSpace(BuildRedisConfiguration());
    return Results.Ok(new
    {
        status = "Healthy",
        backend = isRedis ? "Upstash Redis" : "In-Memory",
        time = DateTime.UtcNow.ToString("o")
    });
});

app.Run();

public partial class Program { }
