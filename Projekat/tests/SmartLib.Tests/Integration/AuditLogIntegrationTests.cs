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
    public class AuditLogTestFixture : WebApplicationFactory<Program>
    {
        private readonly string _dbName = "AuditLogTestDb_" + Guid.NewGuid();

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
            AuditLogDbSeeder.Seed(db);

            return host;
        }
    }

    public class AuditLogIntegrationTests : IClassFixture<AuditLogTestFixture>
    {
        private readonly AuditLogTestFixture _factory;

        public AuditLogIntegrationTests(AuditLogTestFixture factory)
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

        // ─── GET /api/auditlog ────────────────────────────────────────

        [Fact]
        public async Task GetAll_BezAuth_Vraca401()
        {
            var resp = await _factory.CreateClient().GetAsync("/api/auditlog");
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task GetAll_KaoBibliotekar_Vraca403()
        {
            var client = await AuthedClientAsync("bibliotekar@smartlib.ba", "Test123!");
            var resp = await client.GetAsync("/api/auditlog");
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }

        [Fact]
        public async Task GetAll_KaoClan_Vraca403()
        {
            var client = await AuthedClientAsync("clan@smartlib.ba", "Test123!");
            var resp = await client.GetAsync("/api/auditlog");
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }
    }

    internal static class AuditLogDbSeeder
    {
        public static void Seed(ApplicationDbContext db)
        {
            IntegrationTestAccountSeeder.EnsureTestAccounts(db);
        }
    }
}
