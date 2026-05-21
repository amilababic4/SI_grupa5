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

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<CacheVersionStore>();

var redisConfig = BuildRedisConfiguration();
if (!string.IsNullOrWhiteSpace(redisConfig))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConfig;
        options.InstanceName = "smartlib:";
    });
}
else
{
    builder.Services.AddDistributedMemoryCache();
}

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

// Redis Health Check Endpoint
app.MapGet("/api/health/redis", async (Microsoft.Extensions.Caching.Distributed.IDistributedCache cache) =>
{
    try
    {
        var testKey = "health_check_time";
        var testValue = DateTime.UtcNow.ToString();
        await cache.SetStringAsync(testKey, testValue, new Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
        });
        
        var retrievedValue = await cache.GetStringAsync(testKey);
        if (retrievedValue == testValue)
        {
            return Results.Ok(new { status = "Healthy", message = "Redis connection successful.", time = testValue });
        }
        return Results.StatusCode(500);
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, title: "Redis connection failed.");
    }
});

app.Run();

public partial class Program { }
