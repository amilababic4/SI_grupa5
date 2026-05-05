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
    // ─── Fixture ──────────────────────────────────────────────────────

    public class KorisnikTestFixture : WebApplicationFactory<Program>
    {
        private readonly string _dbName = "KorisnikTestDb_" + Guid.NewGuid();

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
            KorisnikDbSeeder.Seed(db);

            return host;
        }
    }

    // ─── Testovi ──────────────────────────────────────────────────────

    public class KorisnikIntegrationTests : IClassFixture<KorisnikTestFixture>
    {
        private readonly KorisnikTestFixture _factory;

        public KorisnikIntegrationTests(KorisnikTestFixture factory)
        {
            _factory = factory;
        }

        // ─── Pomoćne metode ───────────────────────────────────────────

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

        // Kreira novog korisnika kroz API i vraća njegov ID.
        // Destruktivni testovi (deaktivacija) koriste vlastite korisnike
        // kako ne bi dirali seedovane naloge od kojih ovise auth testovi.
        private async Task<int> KreirajKorisnikaAsync(HttpClient client,
            string email, string ime = "Test", string prezime = "Korisnik")
        {
            var resp = await client.PostAsJsonAsync("/api/korisnik", new
            {
                ime,
                prezime,
                email,
                lozinka = "Test123!"
            });
            resp.EnsureSuccessStatusCode();
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            return doc.RootElement.GetProperty("id").GetInt32();
        }

        // ─── GET /api/korisnik — pregled liste korisnika ──────────────

        [Fact]
        public async Task GetAll_BezAuth_Vraca401()
        {
            // US-08: Neprijavljeni korisnik ne smije pristupiti listi korisnika
            var resp = await _factory.CreateClient().GetAsync("/api/korisnik");
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task GetAll_KaoClan_Vraca403()
        {
            // KorisnikController zahtijeva ulogu Bibliotekar ili Administrator
            var client = await ClanClientAsync();
            var resp = await client.GetAsync("/api/korisnik");
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }

        [Fact]
        public async Task GetAll_KaoBibliotekar_VracaListuKorisnika()
        {
            // Bibliotekar može pregledati sve aktivne korisnike
            var client = await BibliotekarClientAsync();
            var resp = await client.GetAsync("/api/korisnik");

            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.True(doc.RootElement.GetArrayLength() >= 1);
        }

        [Fact]
        public async Task GetAll_SvakiElementImaOcekivanaPolja()
        {
            // Response mora sadržavati polja potrebna za prikaz u listi članova
            var client = await BibliotekarClientAsync();
            var resp = await client.GetAsync("/api/korisnik");
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            var prvi = doc.RootElement[0];

            Assert.True(prvi.TryGetProperty("id", out _));
            Assert.True(prvi.TryGetProperty("ime", out _));
            Assert.True(prvi.TryGetProperty("prezime", out _));
            Assert.True(prvi.TryGetProperty("email", out _));
            Assert.True(prvi.TryGetProperty("uloga", out _));
            Assert.True(prvi.TryGetProperty("status", out _));
        }

        [Fact]
        public async Task GetAll_VracaSamoAktivneKorisnike()
        {
            // GetAllAsync u repozitoriju filtrira samo Status == "aktivan"
            var client = await BibliotekarClientAsync();
            var resp = await client.GetAsync("/api/korisnik");
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());

            Assert.All(
                doc.RootElement.EnumerateArray(),
                k => Assert.Equal("aktivan", k.GetProperty("status").GetString())
            );
        }

        // ─── GET /api/korisnik/{id} ───────────────────────────────────

        [Fact]
        public async Task GetById_BezAuth_Vraca401()
        {
            // US-08: Direktan URL ne zaobilazi autentifikaciju
            var resp = await _factory.CreateClient().GetAsync("/api/korisnik/1");
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task GetById_KaoClan_Vraca403()
        {
            var client = await ClanClientAsync();
            var resp = await client.GetAsync("/api/korisnik/1");
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }

        [Fact]
        public async Task GetById_PostojeciId_VracaKorisnika()
        {
            // Seedovani bibliotekar ima id=1 u test bazi
            var client = await BibliotekarClientAsync();
            var resp = await client.GetAsync("/api/korisnik/1");

            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.True(doc.RootElement.TryGetProperty("email", out _));
        }

        [Fact]
        public async Task GetById_NepostojeciId_Vraca404()
        {
            var client = await BibliotekarClientAsync();
            var resp = await client.GetAsync("/api/korisnik/99999");
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }

        [Fact]
        public async Task GetById_VracaDeaktiviranogKorisnika()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var deaktivirani = db.Korisnici.First(k => k.Email == "deaktiviran@smartlib.ba");

            var client = await BibliotekarClientAsync();
            var resp = await client.GetAsync($"/api/korisnik/{deaktivirani.Id}");

            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.Equal("deaktiviran", doc.RootElement.GetProperty("status").GetString());
        }
        // ─── POST /api/korisnik — US-01, US-02, US-03 ────────────────

        [Fact]
        public async Task Create_BezAuth_Vraca401()
        {
            // US-08: Neprijavljeni korisnik ne može kreirati naloge
            var resp = await _factory.CreateClient().PostAsJsonAsync("/api/korisnik", new
            {
                ime = "Test",
                prezime = "Test",
                email = "novi@test.com",
                lozinka = "Test123!"
            });
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task Create_KaoClan_Vraca403()
        {
            // Samo bibliotekar i administrator mogu kreirati naloge
            var client = await ClanClientAsync();
            var resp = await client.PostAsJsonAsync("/api/korisnik", new
            {
                ime = "Test",
                prezime = "Test",
                email = "noviclan@test.com",
                lozinka = "Test123!"
            });
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }

        [Fact]
        public async Task Create_ValidanPayload_Vraca201()
        {
            // US-03: Kada su svi podaci ispravno uneseni, novi član se uspješno registruje
            var client = await BibliotekarClientAsync();
            var resp = await client.PostAsJsonAsync("/api/korisnik", new
            {
                ime = "Novi",
                prezime = "Clan",
                email = "noviclan1@smartlib.ba",
                lozinka = "Test123!"
            });

            Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.Equal("noviclan1@smartlib.ba", doc.RootElement.GetProperty("email").GetString());
        }

        [Fact]
        public async Task Create_NoviKorisnik_DobijaUloguClan()
        {
            var client = await BibliotekarClientAsync();
            var createResp = await client.PostAsJsonAsync("/api/korisnik", new
            {
                ime = "Novi",
                prezime = "Clan",
                email = "noviclan2@smartlib.ba",
                lozinka = "Test123!"
            });
            Assert.Equal(HttpStatusCode.Created, createResp.StatusCode);

            var created = JsonDocument.Parse(await createResp.Content.ReadAsStringAsync());
            var id = created.RootElement.GetProperty("id").GetInt32();

            var getResp = await client.GetAsync($"/api/korisnik/{id}");
            var doc = JsonDocument.Parse(await getResp.Content.ReadAsStringAsync());
            Assert.Equal("Član", doc.RootElement.GetProperty("uloga").GetString());
        }

        [Fact]
        public async Task Create_NoviKorisnik_StatusJeAktivan()
        {
            // US-03: Novokreirani nalog mora biti aktivan
            var client = await BibliotekarClientAsync();
            var createResp = await client.PostAsJsonAsync("/api/korisnik", new
            {
                ime = "Novi",
                prezime = "Clan",
                email = "noviclan3@smartlib.ba",
                lozinka = "Test123!"
            });
            Assert.Equal(HttpStatusCode.Created, createResp.StatusCode);

            var created = JsonDocument.Parse(await createResp.Content.ReadAsStringAsync());
            var id = created.RootElement.GetProperty("id").GetInt32();

            var getResp = await client.GetAsync($"/api/korisnik/{id}");
            var doc = JsonDocument.Parse(await getResp.Content.ReadAsStringAsync());
            Assert.Equal("aktivan", doc.RootElement.GetProperty("status").GetString());
        }



        [Fact]
        public async Task Create_NoviKorisnik_VidljivUGetAll()
        {
            // US-03: Novi član se pojavljuje u listi članova biblioteke
            var client = await BibliotekarClientAsync();
            await client.PostAsJsonAsync("/api/korisnik", new
            {
                ime = "Novi",
                prezime = "Clan",
                email = "noviclan4@smartlib.ba",
                lozinka = "Test123!"
            });

            var resp = await client.GetAsync("/api/korisnik");
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            var emailovi = doc.RootElement.EnumerateArray()
                .Select(k => k.GetProperty("email").GetString());

            Assert.Contains("noviclan4@smartlib.ba", emailovi);
        }

        [Fact]
        public async Task Create_EmailSeNormalizujeNaLowercase()
        {
            // GetByEmailAsync koristi ToLowerInvariant — email se mora normalizovati pri upisu
            var client = await BibliotekarClientAsync();
            var createResp = await client.PostAsJsonAsync("/api/korisnik", new
            {
                ime = "Novi",
                prezime = "Clan",
                email = "NoviClan5@SmartLib.BA",
                lozinka = "Test123!"
            });
            Assert.Equal(HttpStatusCode.Created, createResp.StatusCode);

            var created = JsonDocument.Parse(await createResp.Content.ReadAsStringAsync());
            var id = created.RootElement.GetProperty("id").GetInt32();

            var getResp = await client.GetAsync($"/api/korisnik/{id}");
            var doc = JsonDocument.Parse(await getResp.Content.ReadAsStringAsync());
            Assert.Equal("noviclan5@smartlib.ba", doc.RootElement.GetProperty("email").GetString());
        }


        [Fact]
        public async Task Create_DuplikatEmail_Vraca400()
        {
            // US-02: Kada email adresa već postoji, prikazuje se poruka o grešci
            var client = await BibliotekarClientAsync();
            // "clan@smartlib.ba" je seedovan
            var resp = await client.PostAsJsonAsync("/api/korisnik", new
            {
                ime = "Test",
                prezime = "Test",
                email = "clan@smartlib.ba",
                lozinka = "Test123!"
            });
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        [Fact]
        public async Task Create_DuplikatEmailRazlicitoKucanje_Vraca400()
        {
            // GetByEmailAsync normalizuje email — "CLAN@SMARTLIB.BA" == "clan@smartlib.ba"
            var client = await BibliotekarClientAsync();
            var resp = await client.PostAsJsonAsync("/api/korisnik", new
            {
                ime = "Test",
                prezime = "Test",
                email = "CLAN@SMARTLIB.BA",
                lozinka = "Test123!"
            });
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        [Fact]
        public async Task Create_PraznoIme_Vraca400()
        {
            // US-02: Sistem ne smije dozvoliti nastavak bez obaveznih podataka
            var client = await BibliotekarClientAsync();
            var resp = await client.PostAsJsonAsync("/api/korisnik", new
            {
                ime = "",
                prezime = "Test",
                email = "validan@test.com",
                lozinka = "Test123!"
            });
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        [Fact]
        public async Task Create_PraznoPrezime_Vraca400()
        {
            // US-02: Prezime je obavezno polje
            var client = await BibliotekarClientAsync();
            var resp = await client.PostAsJsonAsync("/api/korisnik", new
            {
                ime = "Test",
                prezime = "",
                email = "validan2@test.com",
                lozinka = "Test123!"
            });
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        [Fact]
        public async Task Create_NevalidanEmailFormat_Vraca400()
        {
            // US-02: Kada email adresa nije u ispravnom formatu, prikazuje se greška
            var client = await BibliotekarClientAsync();
            var resp = await client.PostAsJsonAsync("/api/korisnik", new
            {
                ime = "Test",
                prezime = "Test",
                email = "nijevalidan-email",
                lozinka = "Test123!"
            });
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        [Fact]
        public async Task Create_LozinkaPrekratka_Vraca400()
        {
            // US-02: Kada lozinka ima manje od 8 znakova, prikazuje se greška
            var client = await BibliotekarClientAsync();
            var resp = await client.PostAsJsonAsync("/api/korisnik", new
            {
                ime = "Test",
                prezime = "Test",
                email = "kratkalozinka@test.com",
                lozinka = "Abc1!"
            });
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        // ─── PUT /api/korisnik/{id}/deaktiviraj — US-09 ──────────────

        [Fact]
        public async Task Deactivate_BezAuth_Vraca401()
        {
            // US-08: Neprijavljeni korisnik ne može deaktivirati naloge
            var resp = await _factory.CreateClient()
                .PutAsync("/api/korisnik/1/deaktiviraj", null);
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task Deactivate_KaoClan_Vraca403()
        {
            // Član nema ovlast za deaktivaciju naloga
            var client = await ClanClientAsync();
            var resp = await client.PutAsync("/api/korisnik/1/deaktiviraj", null);
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }

        [Fact]
        public async Task Deactivate_PostojeciKorisnik_Vraca204()
        {
            // US-09: Bibliotekar može deaktivirati aktivni nalog
            var client = await BibliotekarClientAsync();
            var id = await KreirajKorisnikaAsync(client, "deaktivacija1@test.com");

            var resp = await client.PutAsync($"/api/korisnik/{id}/deaktiviraj", null);
            Assert.Equal(HttpStatusCode.NoContent, resp.StatusCode);
        }

        [Fact]
        public async Task Deactivate_StatusSeMijenjaNaDeaktiviran()
        {
            // US-09: Nakon deaktivacije status mora biti "deaktiviran"
            var client = await BibliotekarClientAsync();
            var id = await KreirajKorisnikaAsync(client, "deaktivacija2@test.com");

            await client.PutAsync($"/api/korisnik/{id}/deaktiviraj", null);

            var resp = await client.GetAsync($"/api/korisnik/{id}");
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.Equal("deaktiviran", doc.RootElement.GetProperty("status").GetString());
        }

        [Fact]
        public async Task Deactivate_DeaktiviraniKorisnik_NijeVidljivUGetAll()
        {
            // GetAllAsync filtrira samo Status == "aktivan" —
            // deaktivirani korisnik mora nestati iz liste
            var client = await BibliotekarClientAsync();
            var id = await KreirajKorisnikaAsync(client, "deaktivacija3@test.com");

            await client.PutAsync($"/api/korisnik/{id}/deaktiviraj", null);

            var resp = await client.GetAsync("/api/korisnik");
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            var idevi = doc.RootElement.EnumerateArray()
                .Select(k => k.GetProperty("id").GetInt32());

            Assert.DoesNotContain(id, idevi);
        }

        [Fact]
        public async Task Deactivate_DeaktiviraniKorisnik_NeMozeSePrijaviti()
        {
            // US-09: Deaktivirani korisnik ne može pristupiti sistemu —
            // AuthController odbija login za Status != "aktivan"
            var client = await BibliotekarClientAsync();
            var email = "deaktivacija4@test.com";
            var id = await KreirajKorisnikaAsync(client, email);

            await client.PutAsync($"/api/korisnik/{id}/deaktiviraj", null);

            var loginResp = await _factory.CreateClient()
                .PostAsJsonAsync("/api/auth/login", new { email, lozinka = "Test123!" });
            Assert.Equal(HttpStatusCode.Unauthorized, loginResp.StatusCode);
        }

        [Fact]
        public async Task Deactivate_NepostojeciId_Vraca404()
        {
            // Korisnik koji se deaktivira mora postojati u sistemu
            var client = await BibliotekarClientAsync();
            var resp = await client.PutAsync("/api/korisnik/99999/deaktiviraj", null);
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }
    }

    // ─── Seeder ───────────────────────────────────────────────────────
    // Seedovani korisnici:
    //   id=1 → bibliotekar@smartlib.ba (Bibliotekar, aktivan) — za autentifikaciju
    //   id=2 → clan@smartlib.ba        (Član, aktivan)        — za provjeru 403
    //   id=3 → deaktiviran@smartlib.ba (Član, deaktiviran)    — za GetById deaktivirani test

    internal static class KorisnikDbSeeder
    {
        public static void Seed(ApplicationDbContext db)
        {
            if (!db.Uloge.Any())
            {
                db.Uloge.AddRange(
                    new SmartLib.Core.Models.Uloga { Id = 1, Naziv = "Član" },
                    new SmartLib.Core.Models.Uloga { Id = 2, Naziv = "Bibliotekar" }
                );
                db.SaveChanges();
            }

            var testAccounts = new[]
            {
                ("bibliotekar@smartlib.ba", "Test123!", 2, "aktivan"),
                ("clan@smartlib.ba",        "Test123!", 1, "aktivan"),
                ("deaktiviran@smartlib.ba", "Test123!", 1, "deaktiviran")
            };

            foreach (var (email, lozinka, ulogaId, status) in testAccounts)
            {
                var korisnik = db.Korisnici.FirstOrDefault(k => k.Email == email);
                if (korisnik != null)
                {
                    korisnik.LozinkaHash = PasswordHasher.HashPassword(lozinka);
                    korisnik.Status = status;
                }
                else
                {
                    db.Korisnici.Add(new SmartLib.Core.Models.Korisnik
                    {
                        Ime = "Test",
                        Prezime = email.Split('@')[0],
                        Email = email,
                        LozinkaHash = PasswordHasher.HashPassword(lozinka),
                        UlogaId = ulogaId,
                        Status = status,
                        DatumKreiranja = DateTime.UtcNow
                    });
                }
            }
            db.SaveChanges();
        }
    }
}