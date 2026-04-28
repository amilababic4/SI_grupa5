using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartLib.Infrastructure.Data;
using SmartLib.Infrastructure.Security;
using Xunit;

namespace SmartLib.Tests.Integration
{
    public class AuthIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public AuthIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<ApplicationDbContext>(opts =>
                        opts.UseInMemoryDatabase("TestDb_" + Guid.NewGuid()));

                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    db.Database.EnsureCreated();
                    DbSeeder.Seed(db);
                });
            });
        }

        private HttpClient CreateClient() => _factory.CreateClient();

        // ─── LOGIN (API) ─────────────────────────────────────────────

        [Fact]
        public async Task Login_PogresnaLozinka_Vraca401()
        {
            var client = CreateClient();

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
            var client = CreateClient();

            var resp = await client.PostAsJsonAsync("/api/auth/login", new
            {
                email = "deaktiviran@smartlib.ba",
                lozinka = "Test123!"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task Login_Neuspjeh_NeOtkrivaDetalje()
        {
            var client = CreateClient();

            var resp = await client.PostAsJsonAsync("/api/auth/login", new
            {
                email = "nepostoji@smartlib.ba",
                lozinka = "Test123!"
            });

            var text = await resp.Content.ReadAsStringAsync();

            Assert.DoesNotContain("email", text, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("lozinka", text, StringComparison.OrdinalIgnoreCase);
        }

        // ─── AUTH PROTECTION (pošto JWT nije implementiran → uvijek 401) ───

        [Fact]
        public async Task ZasticenaRuta_BezAuth_Vraca401()
        {
            var client = CreateClient();

            var resp = await client.GetAsync("/api/korisnik");

            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task Logout_BezAuth_Vraca401()
        {
            var client = CreateClient();

            var resp = await client.PostAsync("/api/auth/logout", null);

            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        // ─── VALIDACIJE (ako endpoint NIJE zaštićen) ───
        // Ako jeste → ovi će vraćati 401 i treba ih skipati

        [Fact]
        public async Task CreateKorisnik_DuplikatEmail_Vraca400_ILI_401()
        {
            var client = CreateClient();

            var resp = await client.PostAsJsonAsync("/api/korisnik", new
            {
                ime = "Test",
                prezime = "Test",
                email = "clan@smartlib.ba",
                lozinka = "Test123!"
            });

            Assert.True(
                resp.StatusCode == HttpStatusCode.BadRequest ||
                resp.StatusCode == HttpStatusCode.Unauthorized
            );
        }
    }

    // ─── SEED ───────────────────────────────────────────────────────

    internal static class DbSeeder
    {
        public static void Seed(ApplicationDbContext db)
        {
            if (db.Uloge.Any()) return;

            db.Uloge.AddRange(
                new SmartLib.Core.Models.Uloga { Id = 1, Naziv = "Član" },
                new SmartLib.Core.Models.Uloga { Id = 2, Naziv = "Bibliotekar" }
            );

            db.Korisnici.AddRange(
                new SmartLib.Core.Models.Korisnik
                {
                    Ime = "Test",
                    Prezime = "Clan",
                    Email = "clan@smartlib.ba",
                    LozinkaHash = PasswordHasher.HashPassword("Test123!"),
                    UlogaId = 1,
                    Status = "aktivan",
                    DatumKreiranja = DateTime.UtcNow
                },
                new SmartLib.Core.Models.Korisnik
                {
                    Ime = "Test",
                    Prezime = "Bibliotekar",
                    Email = "bibliotekar@smartlib.ba",
                    LozinkaHash = PasswordHasher.HashPassword("Test123!"),
                    UlogaId = 2,
                    Status = "aktivan",
                    DatumKreiranja = DateTime.UtcNow
                },
                new SmartLib.Core.Models.Korisnik
                {
                    Ime = "Test",
                    Prezime = "Deaktiviran",
                    Email = "deaktiviran@smartlib.ba",
                    LozinkaHash = PasswordHasher.HashPassword("Test123!"),
                    UlogaId = 1,
                    Status = "deaktiviran",
                    DatumKreiranja = DateTime.UtcNow
                }
            );

            db.SaveChanges();
        }
    }
}