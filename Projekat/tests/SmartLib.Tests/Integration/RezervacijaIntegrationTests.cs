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
    public class RezervacijaTestFixture : WebApplicationFactory<Program>
    {
        private readonly string _dbName = "RezervacijaTestDb_" + Guid.NewGuid();

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
            RezervacijaDbSeeder.Seed(db);

            return host;
        }
    }

    public class RezervacijaIntegrationTests : IClassFixture<RezervacijaTestFixture>
    {
        private readonly RezervacijaTestFixture _factory;

        public RezervacijaIntegrationTests(RezervacijaTestFixture factory)
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

        // ─── GET /api/rezervacija ──────────────────────────────────────

        [Fact]
        public async Task GetActive_BezAuth_Vraca401()
        {
            var resp = await _factory.CreateClient().GetAsync("/api/rezervacija");
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task GetActive_KaoClan_Vraca200TODO()
        {
            var client = await ClanClientAsync();
            var resp = await client.GetAsync("/api/rezervacija");
            await AssertTodoResponseAsync(resp);
        }

        [Fact]
        public async Task GetActive_KaoBibliotekar_Vraca200TODO()
        {
            var client = await BibliotekarClientAsync();
            var resp = await client.GetAsync("/api/rezervacija");
            await AssertTodoResponseAsync(resp);
        }

        // ─── GET /api/rezervacija/moje ────────────────────────────────

        [Fact]
        public async Task GetMine_BezAuth_Vraca401()
        {
            var resp = await _factory.CreateClient().GetAsync("/api/rezervacija/moje");
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task GetMine_KaoClan_Vraca200TODO()
        {
            var client = await ClanClientAsync();
            var resp = await client.GetAsync("/api/rezervacija/moje");
            await AssertTodoResponseAsync(resp);
        }

        // ─── POST /api/rezervacija ──────────────────────────────────────

        [Fact]
        public async Task Create_BezAuth_Vraca401()
        {
            var resp = await _factory.CreateClient().PostAsync("/api/rezervacija", null);
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task Create_KaoClan_Vraca200TODO()
        {
            var client = await ClanClientAsync();
            var resp = await client.PostAsync("/api/rezervacija", null);
            await AssertTodoResponseAsync(resp);
        }

        // ─── DELETE /api/rezervacija/{id} ───────────────────────────────

        [Fact]
        public async Task Cancel_BezAuth_Vraca401()
        {
            var resp = await _factory.CreateClient().DeleteAsync("/api/rezervacija/1");
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task Cancel_KaoClan_Vraca200TODO()
        {
            var client = await ClanClientAsync();
            var resp = await client.DeleteAsync("/api/rezervacija/1");
            await AssertTodoResponseAsync(resp);
        }
    }

    internal static class RezervacijaDbSeeder
    {
        public static void Seed(ApplicationDbContext db)
        {
            IntegrationTestAccountSeeder.EnsureTestAccounts(db);
        }
    }
}
