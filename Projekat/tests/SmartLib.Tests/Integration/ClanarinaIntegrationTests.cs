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
using Xunit;

namespace SmartLib.Tests.Integration
{
    public class ClanarinaTestFixture : WebApplicationFactory<Program>
    {
        private readonly string _dbName = "ClanarinaTestDb_" + Guid.NewGuid();

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
            ClanarinaDbSeeder.Seed(db);

            return host;
        }
    }

    public class ClanarinaIntegrationTests : IClassFixture<ClanarinaTestFixture>
    {
        private readonly ClanarinaTestFixture _factory;

        public ClanarinaIntegrationTests(ClanarinaTestFixture factory)
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

        private async Task<HttpClient> BibliotekarClientAsync()
        {
            var token = await GetTokenAsync("bibliotekar@smartlib.ba", "Test123!");
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        private async Task<HttpClient> ClanClientAsync()
        {
            var token = await GetTokenAsync("clan@smartlib.ba", "Test123!");
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        private static async Task AssertTodoResponseAsync(HttpResponseMessage resp)
        {
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.Equal("TODO", doc.RootElement.GetProperty("message").GetString());
        }

        // ─── GET /api/clanarina/{korisnikId} ──────────────────────────

        [Fact]
        public async Task GetByKorisnik_BezAuth_Vraca401()
        {
            var resp = await _factory.CreateClient().GetAsync("/api/clanarina/1");
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task GetByKorisnik_KaoClan_Vraca403()
        {
            var client = await ClanClientAsync();
            var resp = await client.GetAsync("/api/clanarina/1");
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }

        [Fact]
        public async Task GetByKorisnik_KaoBibliotekar_Vraca200TODO()
        {
            var client = await BibliotekarClientAsync();
            var resp = await client.GetAsync("/api/clanarina/2");
            await AssertTodoResponseAsync(resp);
        }

        // ─── POST /api/clanarina ──────────────────────────────────────

        [Fact]
        public async Task Create_BezAuth_Vraca401()
        {
            var resp = await _factory.CreateClient().PostAsync("/api/clanarina", null);
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task Create_KaoClan_Vraca403()
        {
            var client = await ClanClientAsync();
            var resp = await client.PostAsync("/api/clanarina", null);
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }

        [Fact]
        public async Task Create_KaoBibliotekar_Vraca200TODO()
        {
            var client = await BibliotekarClientAsync();
            var resp = await client.PostAsync("/api/clanarina", null);
            await AssertTodoResponseAsync(resp);
        }

        // ─── PUT /api/clanarina/{id} ──────────────────────────────────

        [Fact]
        public async Task Update_BezAuth_Vraca401()
        {
            var resp = await _factory.CreateClient().PutAsync("/api/clanarina/1", null);
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task Update_KaoClan_Vraca403()
        {
            var client = await ClanClientAsync();
            var resp = await client.PutAsync("/api/clanarina/1", null);
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }

        [Fact]
        public async Task Update_KaoBibliotekar_Vraca200TODO()
        {
            var client = await BibliotekarClientAsync();
            var resp = await client.PutAsync("/api/clanarina/1", null);
            await AssertTodoResponseAsync(resp);
        }
    }

    internal static class ClanarinaDbSeeder
    {
        public static void Seed(ApplicationDbContext db)
        {
            IntegrationTestAccountSeeder.EnsureTestAccounts(db);
        }
    }
}
