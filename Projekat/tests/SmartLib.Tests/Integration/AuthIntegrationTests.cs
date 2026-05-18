using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartLib.Infrastructure.Data;
using SmartLib.Infrastructure.Security;
using Xunit;

namespace SmartLib.Tests.Integration
{
    public class AuthTestFixture : WebApplicationFactory<Program>
    {
        private readonly string _dbName = "AuthTestDb_" + Guid.NewGuid();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((_, cfg) =>
                cfg.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "TestSecretKeyForIntegrationTests_MinLen32!!",
                    ["Jwt:Issuer"] = "SmartLibTest",
                    ["Jwt:Audience"] = "SmartLibTest",
                    ["Jwt:ExpirationMinutes"] = "60"
                }));

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<ApplicationDbContext>(opts =>
                    opts.UseInMemoryDatabase(_dbName));
            });
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = base.CreateHost(builder);

            using var scope = host.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();
            AuthDbSeeder.Seed(db);

            return host;
        }
    }

    public class AuthIntegrationTests : IClassFixture<AuthTestFixture>
    {
        private readonly AuthTestFixture _factory;

        public AuthIntegrationTests(AuthTestFixture factory)
        {
            _factory = factory;
        }

        private async Task<string> GetTokenAsync(string email, string lozinka)
        {
            var client = _factory.CreateClient();
            var resp = await client.PostAsJsonAsync("/api/auth/login", new { email, lozinka });
            resp.EnsureSuccessStatusCode();
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            return doc.RootElement.GetProperty("token").GetString()!;
        }

        private async Task<HttpClient> AuthedClientAsync(string email, string lozinka)
        {
            var token = await GetTokenAsync(email, lozinka);
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        // ─── POST /api/auth/login ─────────────────────────────────────

        [Fact]
        public async Task Login_ValidniPodaci_Vraca200STokenom()
        {
            var client = _factory.CreateClient();

            var resp = await client.PostAsJsonAsync("/api/auth/login", new
            {
                email = "clan@smartlib.ba",
                lozinka = "Test123!"
            });

            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.False(string.IsNullOrWhiteSpace(doc.RootElement.GetProperty("token").GetString()));
            Assert.Equal("Test", doc.RootElement.GetProperty("ime").GetString());
            Assert.Equal("Član", doc.RootElement.GetProperty("uloga").GetString());
        }

        [Fact]
        public async Task Login_Bibliotekar_VracaUloguBibliotekar()
        {
            var client = _factory.CreateClient();

            var resp = await client.PostAsJsonAsync("/api/auth/login", new
            {
                email = "bibliotekar@smartlib.ba",
                lozinka = "Test123!"
            });

            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.Equal("Bibliotekar", doc.RootElement.GetProperty("uloga").GetString());
        }

        [Fact]
        public async Task Login_PogresnaLozinka_Vraca401()
        {
            var client = _factory.CreateClient();

            var resp = await client.PostAsJsonAsync("/api/auth/login", new
            {
                email = "clan@smartlib.ba",
                lozinka = "Pogresna"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task Login_DeaktiviranKorisnik_Vraca401()
        {
            var client = _factory.CreateClient();

            var resp = await client.PostAsJsonAsync("/api/auth/login", new
            {
                email = "deaktiviran@smartlib.ba",
                lozinka = "Test123!"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task Login_NepostojeciEmail_Vraca401()
        {
            var client = _factory.CreateClient();

            var resp = await client.PostAsJsonAsync("/api/auth/login", new
            {
                email = "nepostoji@smartlib.ba",
                lozinka = "Test123!"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task Login_Neuspjeh_NeOtkrivaDetalje()
        {
            var client = _factory.CreateClient();

            var resp = await client.PostAsJsonAsync("/api/auth/login", new
            {
                email = "nepostoji@smartlib.ba",
                lozinka = "Test123!"
            });

            var text = await resp.Content.ReadAsStringAsync();

            Assert.DoesNotContain("email", text, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("lozinka", text, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Login_PrazanEmail_Vraca400()
        {
            var client = _factory.CreateClient();

            var resp = await client.PostAsJsonAsync("/api/auth/login", new
            {
                email = "",
                lozinka = "Test123!"
            });

            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        // ─── POST /api/auth/logout ────────────────────────────────────

        [Fact]
        public async Task Logout_BezAuth_Vraca401()
        {
            var resp = await _factory.CreateClient().PostAsync("/api/auth/logout", null);
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task Logout_SaTokenom_Vraca200()
        {
            var client = await AuthedClientAsync("clan@smartlib.ba", "Test123!");

            var resp = await client.PostAsync("/api/auth/logout", null);

            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.Equal("Korisnik je odjavljen.", doc.RootElement.GetProperty("message").GetString());
        }

        // ─── Zaštićene rute ───────────────────────────────────────────

        [Fact]
        public async Task ZasticenaRuta_BezAuth_Vraca401()
        {
            var resp = await _factory.CreateClient().GetAsync("/api/korisnik");
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task ZasticenaRuta_SaTokenom_Vraca200()
        {
            var client = await AuthedClientAsync("bibliotekar@smartlib.ba", "Test123!");
            var resp = await client.GetAsync("/api/korisnik");
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        }
    }

    internal static class AuthDbSeeder
    {
        public static void Seed(ApplicationDbContext db)
        {
            IntegrationTestAccountSeeder.EnsureTestAccounts(db);
        }
    }
}
