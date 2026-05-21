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
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;
using Xunit;

namespace SmartLib.Tests.Integration
{
    public class RezervacijaTestFixture : WebApplicationFactory<Program>
    {
        private readonly string _dbName = "RezervacijaTestDb_" + Guid.NewGuid();

        public int KnjigaBezDostupnihId { get; private set; }
        public int KnjigaSaDostupnimId { get; private set; }
        public int KnjigaSaRezervacijomId { get; private set; }
        // Rezervacija na KnjigaSaRezervacijomId — koristi se samo za duplikat test, NE otkazivati
        public int DuplikatRezervacijaId { get; private set; }
        // Rezervacija na posebnoj knjizi — koristi se za testove otkazivanja
        public int OtkazRezervacijaId { get; private set; }

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
            var ids = RezervacijaDbSeeder.Seed(db);
            KnjigaBezDostupnihId = ids.knjigaBezDostupnihId;
            KnjigaSaDostupnimId = ids.knjigaSaDostupnimId;
            KnjigaSaRezervacijomId = ids.knjigaSaRezervacijomId;
            DuplikatRezervacijaId = ids.duplikatRezervacijaId;
            OtkazRezervacijaId = ids.otkazRezervacijaId;

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

        private async Task<HttpClient> DrugiclanClientAsync()
        {
            var token = await GetTokenAsync("drugiClan@testlib.ba", "Test123!");
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        // ─── GET /api/rezervacija ──────────────────────────────────────

        [Fact]
        public async Task GetActive_BezAuth_Vraca401()
        {
            var resp = await _factory.CreateClient().GetAsync("/api/rezervacija");
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task GetActive_KaoBibliotekar_VracaAktivneRezervacije()
        {
            var client = await BibliotekarClientAsync();
            var resp = await client.GetAsync("/api/rezervacija");
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);
        }

        [Fact]
        public async Task GetActive_KaoClan_Vraca403()
        {
            var client = await ClanClientAsync();
            var resp = await client.GetAsync("/api/rezervacija");
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }

        // ─── GET /api/rezervacija/moje ────────────────────────────────

        [Fact]
        public async Task GetMine_BezAuth_Vraca401()
        {
            var resp = await _factory.CreateClient().GetAsync("/api/rezervacija/moje");
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task GetMine_KaoClan_VracaSamoVlastiteRezervacije()
        {
            var client = await ClanClientAsync();
            var resp = await client.GetAsync("/api/rezervacija/moje");
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);
            // Clan ima jednu aktivnu rezervaciju (za KnjigaSaRezervacijomId)
            Assert.True(doc.RootElement.GetArrayLength() >= 1);
        }

        // ─── POST /api/rezervacija ──────────────────────────────────────

        [Fact]
        public async Task Create_BezAuth_Vraca401()
        {
            var resp = await _factory.CreateClient().PostAsync("/api/rezervacija", null);
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task Create_KnjigaSaDostupnimPrimjercima_Vraca400()
        {
            var client = await ClanClientAsync();
            var resp = await client.PostAsJsonAsync("/api/rezervacija",
                new { knjigaId = _factory.KnjigaSaDostupnimId });
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        [Fact]
        public async Task Create_KnjigaBezDostupnih_Vraca201()
        {
            var client = await ClanClientAsync();
            var resp = await client.PostAsJsonAsync("/api/rezervacija",
                new { knjigaId = _factory.KnjigaBezDostupnihId });
            Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.True(doc.RootElement.GetProperty("id").GetInt32() > 0);
        }

        [Fact]
        public async Task Create_DuplikatAktivneRezervacije_Vraca409()
        {
            var client = await ClanClientAsync();
            // KnjigaSaRezervacijomId već ima aktivnu rezervaciju od ovog člana (seeded)
            var resp = await client.PostAsJsonAsync("/api/rezervacija",
                new { knjigaId = _factory.KnjigaSaRezervacijomId });
            Assert.Equal(HttpStatusCode.Conflict, resp.StatusCode);
        }

        // ─── DELETE /api/rezervacija/{id} ───────────────────────────────

        [Fact]
        public async Task Cancel_BezAuth_Vraca401()
        {
            var resp = await _factory.CreateClient().DeleteAsync("/api/rezervacija/1");
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task Cancel_VlastikaRezervacija_Vraca200()
        {
            var client = await ClanClientAsync();
            var resp = await client.DeleteAsync($"/api/rezervacija/{_factory.OtkazRezervacijaId}");
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.NotNull(doc.RootElement.GetProperty("poruka").GetString());
        }

        [Fact]
        public async Task Cancel_TudaRezervacija_Vraca403()
        {
            // Drugi clan pokušava otkazati rezervaciju prvog clana
            var client = await DrugiclanClientAsync();
            var resp = await client.DeleteAsync($"/api/rezervacija/{_factory.OtkazRezervacijaId}");
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }

        [Fact]
        public async Task Cancel_NepostojecaRezervacija_Vraca404()
        {
            var client = await ClanClientAsync();
            var resp = await client.DeleteAsync("/api/rezervacija/999999");
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }
    }

    internal static class RezervacijaDbSeeder
    {
        public static (int knjigaBezDostupnihId, int knjigaSaDostupnimId, int knjigaSaRezervacijomId, int duplikatRezervacijaId, int otkazRezervacijaId) Seed(ApplicationDbContext db)
        {
            IntegrationTestAccountSeeder.EnsureTestAccounts(db);

            // Dodaj drugog clana za testove tuđe rezervacije
            if (!db.Korisnici.Any(k => k.Email == "drugiClan@testlib.ba"))
            {
                var hash = SmartLib.Infrastructure.Security.PasswordHasher.HashPassword("Test123!");
                db.Korisnici.Add(new Korisnik
                {
                    Ime = "Drugi",
                    Prezime = "Clan",
                    Email = "drugiClan@testlib.ba",
                    LozinkaHash = hash,
                    UlogaId = 1,
                    Status = "aktivan",
                    DatumKreiranja = DateTime.UtcNow
                });
                db.SaveChanges();
            }

            var clan = db.Korisnici.First(k => k.Email == "clan@smartlib.ba");

            // Kategorija
            var kat = db.Kategorije.FirstOrDefault();
            if (kat == null)
            {
                kat = new Kategorija { Naziv = "Test kategorija", Opis = "Za testove" };
                db.Kategorije.Add(kat);
                db.SaveChanges();
            }

            // Knjiga 1: nema dostupnih primjeraka, bez rezervacije — za kreiranje
            var knjiga1 = new Knjiga
            {
                Naslov = "Test knjiga bez dostupnih",
                Autor = "Test autor",
                Isbn = "9780000000001",
                KategorijaId = kat.Id,
                GodinaIzdanja = 2024
            };
            db.Knjige.Add(knjiga1);
            db.SaveChanges();
            db.Primjerci.Add(new Primjerak { KnjigaId = knjiga1.Id, InventarniBroj = "INV-T-001", Status = "zadužen" });
            db.SaveChanges();

            // Knjiga 2: ima dostupan primjerak — za test odbijanja
            var knjiga2 = new Knjiga
            {
                Naslov = "Test knjiga sa dostupnim",
                Autor = "Test autor",
                Isbn = "9780000000002",
                KategorijaId = kat.Id,
                GodinaIzdanja = 2024
            };
            db.Knjige.Add(knjiga2);
            db.SaveChanges();
            db.Primjerci.Add(new Primjerak { KnjigaId = knjiga2.Id, InventarniBroj = "INV-T-002", Status = "dostupan" });
            db.SaveChanges();

            // Knjiga 3: nema dostupnih, ima rezervaciju od clana — za duplikat test (NE otkazivati)
            var knjiga3 = new Knjiga
            {
                Naslov = "Test knjiga sa rezervacijom (duplikat)",
                Autor = "Test autor",
                Isbn = "9780000000003",
                KategorijaId = kat.Id,
                GodinaIzdanja = 2024
            };
            db.Knjige.Add(knjiga3);
            db.SaveChanges();
            db.Primjerci.Add(new Primjerak { KnjigaId = knjiga3.Id, InventarniBroj = "INV-T-003", Status = "zadužen" });
            db.SaveChanges();
            var duplikatRez = new Rezervacija
            {
                KorisnikId = clan.Id,
                KnjigaId = knjiga3.Id,
                DatumRezervacije = DateTime.UtcNow,
                DatumIsteka = DateTime.UtcNow.AddDays(7),
                Status = "aktivna"
            };
            db.Rezervacije.Add(duplikatRez);
            db.SaveChanges();

            // Knjiga 4: nema dostupnih, ima rezervaciju od clana — isključivo za testove otkazivanja
            var knjiga4 = new Knjiga
            {
                Naslov = "Test knjiga za otkazivanje",
                Autor = "Test autor",
                Isbn = "9780000000004",
                KategorijaId = kat.Id,
                GodinaIzdanja = 2024
            };
            db.Knjige.Add(knjiga4);
            db.SaveChanges();
            db.Primjerci.Add(new Primjerak { KnjigaId = knjiga4.Id, InventarniBroj = "INV-T-004", Status = "zadužen" });
            db.SaveChanges();
            var otkazRez = new Rezervacija
            {
                KorisnikId = clan.Id,
                KnjigaId = knjiga4.Id,
                DatumRezervacije = DateTime.UtcNow,
                DatumIsteka = DateTime.UtcNow.AddDays(7),
                Status = "aktivna"
            };
            db.Rezervacije.Add(otkazRez);
            db.SaveChanges();

            return (knjiga1.Id, knjiga2.Id, knjiga3.Id, duplikatRez.Id, otkazRez.Id);
        }
    }
}
