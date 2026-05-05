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

    public class KategorijaTestFixture : WebApplicationFactory<Program>
    {
        private readonly string _dbName = "KategorijaTestDb_" + Guid.NewGuid();

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
            KategorijaDbSeeder.Seed(db);

            return host;
        }
    }

    // ─── Testovi ──────────────────────────────────────────────────────

    public class KategorijaIntegrationTests : IClassFixture<KategorijaTestFixture>
    {
        private readonly KategorijaTestFixture _factory;

        public KategorijaIntegrationTests(KategorijaTestFixture factory)
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

        // Kreira kategoriju i vraća njen ID
        // Koristi se u destruktivnim testovima (edit, delete) kako ti testovi
        // ne bi dirali seedovane podatke na kojima se oslanjaju drugi testovi.
        private async Task<int> KreirajKategorijuAsync(HttpClient client, string naziv, string? opis = null)
        {
            var resp = await client.PostAsJsonAsync("/api/kategorija", new { naziv, opis });
            resp.EnsureSuccessStatusCode();
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            return doc.RootElement.GetProperty("id").GetInt32();
        }

        // ─── GET /api/kategorija — US-31: Pregled svih kategorija ─────

        [Fact]
        public async Task GetAll_BezAuth_Vraca401()
        {
            // US-08: Neprijavljeni korisnik ne smije pristupiti zaštićenim rutama
            var resp = await _factory.CreateClient().GetAsync("/api/kategorija");
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task GetAll_KaoClan_Vraca403()
        {
            // Kontroler zahtijeva ulogu Bibliotekar ili Administrator —
            // Član nema pristup upravljanju kategorijama
            var client = await ClanClientAsync();
            var resp = await client.GetAsync("/api/kategorija");
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }

        [Fact]
        public async Task GetAll_KaoBibliotekar_VracaListuKategorija()
        {
            // US-31: Sekcija "Kategorije" prikazuje listu svih kategorija
            var client = await BibliotekarClientAsync();
            var resp = await client.GetAsync("/api/kategorija");

            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.True(doc.RootElement.GetArrayLength() >= 2);
        }

        [Fact]
        public async Task GetAll_SvakiElementImaOcekivanaPolja()
        {
            // US-31: Lista prikazuje naziv svake kategorije i broj povezanih knjiga
            var client = await BibliotekarClientAsync();
            var resp = await client.GetAsync("/api/kategorija");
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            var prvi = doc.RootElement[0];

            Assert.True(prvi.TryGetProperty("id", out _));
            Assert.True(prvi.TryGetProperty("naziv", out _));
            Assert.True(prvi.TryGetProperty("brojKnjiga", out _));
        }

        [Fact]
        public async Task GetAll_SeededKategorijaSaKnjigama_ImaIspravanBrojKnjiga()
        {
            // US-31: BrojKnjiga mora biti tačan — "Beletristika" ima 1 seedovanu knjigu
            var client = await BibliotekarClientAsync();
            var resp = await client.GetAsync("/api/kategorija");
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());

            var beletristika = doc.RootElement
                .EnumerateArray()
                .FirstOrDefault(k => k.GetProperty("naziv").GetString() == "Beletristika");

            Assert.Equal(1, beletristika.GetProperty("brojKnjiga").GetInt32());
        }

        // ─── GET /api/kategorija/{id} — US-31: Pregled jedne kategorije ─

        [Fact]
        public async Task GetById_BezAuth_Vraca401()
        {
            // US-08: Direktan URL ne zaobilazi autentifikaciju
            var resp = await _factory.CreateClient().GetAsync("/api/kategorija/1");
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task GetById_PostojeciId_VracaKategoriju()
        {
            // US-31: Kategorija postoji i vraća se sa ispravnim nazivom
            var client = await BibliotekarClientAsync();
            var resp = await client.GetAsync("/api/kategorija/1");

            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.Equal("Beletristika", doc.RootElement.GetProperty("naziv").GetString());
        }

        [Fact]
        public async Task GetById_NepostojeciId_Vraca404()
        {
            // US-31: Direktan pokušaj otvaranja nepostojeće kategorije vraća 404
            var client = await BibliotekarClientAsync();
            var resp = await client.GetAsync("/api/kategorija/99999");
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }

        // ─── POST /api/kategorija — US-30: Dodavanje nove kategorije ──

        [Fact]
        public async Task Create_BezAuth_Vraca401()
        {
            // US-08: Neprijavljeni korisnik ne može dodavati kategorije
            var resp = await _factory.CreateClient()
                .PostAsJsonAsync("/api/kategorija", new { naziv = "Nova" });
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task Create_KaoClan_Vraca403()
        {
            // Član nema ovlast za kreiranje kategorija
            var client = await ClanClientAsync();
            var resp = await client.PostAsJsonAsync("/api/kategorija", new { naziv = "Nova" });
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }

        [Fact]
        public async Task Create_ValidanNaziv_Vraca201SaPodrucjem()
        {
            // US-30: Kada korisnik unese naziv i klikne "Sačuvaj",
            // kategorija se sprema i prikazuje u listi
            var client = await BibliotekarClientAsync();
            var resp = await client.PostAsJsonAsync("/api/kategorija",
                new { naziv = "Historija", opis = "Historijske knjige" });

            Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.Equal("Historija", doc.RootElement.GetProperty("naziv").GetString());
            Assert.True(doc.RootElement.TryGetProperty("id", out _));
        }

        [Fact]
        public async Task Create_NakonKreiranja_KategorijaVidljivaUGetAll()
        {
            // US-30: Kada je kategorija uspješno dodana, prikazuje se u listi
            var client = await BibliotekarClientAsync();
            await client.PostAsJsonAsync("/api/kategorija", new { naziv = "Filozofija" });

            var resp = await client.GetAsync("/api/kategorija");
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            var nazivi = doc.RootElement.EnumerateArray()
                .Select(k => k.GetProperty("naziv").GetString());

            Assert.Contains("Filozofija", nazivi);
        }

        [Fact]
        public async Task Create_DuplikatNaziv_Vraca409()
        {
            // US-30: Kada kategorija već postoji, sistem prikazuje poruku o grešci
            var client = await BibliotekarClientAsync();
            var resp = await client.PostAsJsonAsync("/api/kategorija",
                new { naziv = "Beletristika" });
            Assert.Equal(HttpStatusCode.Conflict, resp.StatusCode);
        }

        [Fact]
        public async Task Create_DuplikatNazivRazlicitoKucanje_Vraca409()
        {
            // US-30: Provjera duplikata je case-insensitive (OrdinalIgnoreCase u kontroleru)
            var client = await BibliotekarClientAsync();
            var resp = await client.PostAsJsonAsync("/api/kategorija",
                new { naziv = "BELETRISTIKA" });
            Assert.Equal(HttpStatusCode.Conflict, resp.StatusCode);
        }

        [Fact]
        public async Task Create_PrazanNaziv_Vraca400()
        {
            // US-30: Sistem ne dozvoljava kreiranje kategorije bez naziva
            var client = await BibliotekarClientAsync();
            var resp = await client.PostAsJsonAsync("/api/kategorija",
                new { naziv = "" });
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        [Fact]
        public async Task Create_NazivSamoRazmaci_Vraca400()
        {
            // US-30: Naziv koji sadrži samo razmake tretira se kao prazan
            var client = await BibliotekarClientAsync();
            var resp = await client.PostAsJsonAsync("/api/kategorija",
                new { naziv = "   " });
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        [Fact]
        public async Task Create_BezOpisa_KategorijaSeSpremaBezOpisa()
        {
            // US-30: Opis nije obavezno polje — kategorija se sprema i bez njega
            var client = await BibliotekarClientAsync();
            var resp = await client.PostAsJsonAsync("/api/kategorija",
                new { naziv = "Putopisi" });

            Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            if (doc.RootElement.TryGetProperty("opis", out var opis))
                Assert.True(opis.ValueKind == JsonValueKind.Null);
        }

        // ─── PUT /api/kategorija/{id} — US-33: Uređivanje kategorije ──

        [Fact]
        public async Task Update_BezAuth_Vraca401()
        {
            // US-08: Neprijavljeni korisnik ne može mijenjati kategorije
            var resp = await _factory.CreateClient()
                .PutAsJsonAsync("/api/kategorija/1", new { naziv = "Novo" });
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task Update_KaoClan_Vraca403()
        {
            // Član nema ovlast za uređivanje kategorija
            var client = await ClanClientAsync();
            var resp = await client.PutAsJsonAsync("/api/kategorija/1",
                new { naziv = "Novo" });
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }

        [Fact]
        public async Task Update_ValidanPayload_Vraca200()
        {
            // US-33: Kada korisnik izmijeni naziv i klikne "Sačuvaj", izmjena se sprema
            var client = await BibliotekarClientAsync();
            var id = await KreirajKategorijuAsync(client, "Za Edit Test");

            var resp = await client.PutAsJsonAsync($"/api/kategorija/{id}",
                new { naziv = "Editovana Kategorija" });
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        }

        [Fact]
        public async Task Update_IzmjenaVidljivaUGetById()
        {
            // US-33: Nakon spremanja, ažurirani podaci se odmah prikazuju
            var client = await BibliotekarClientAsync();
            var id = await KreirajKategorijuAsync(client, "Stari Naziv");

            await client.PutAsJsonAsync($"/api/kategorija/{id}",
                new { naziv = "Novi Naziv", opis = "Novi opis" });

            var resp = await client.GetAsync($"/api/kategorija/{id}");
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.Equal("Novi Naziv", doc.RootElement.GetProperty("naziv").GetString());
        }

        [Fact]
        public async Task Update_DuplikatNazivDrugeKategorije_Vraca409()
        {
            // US-33: Kada naziv već postoji kod druge kategorije, sistem odbija izmjenu
            var client = await BibliotekarClientAsync();
            var id = await KreirajKategorijuAsync(client, "Kategorija Za Konflikt");

            var resp = await client.PutAsJsonAsync($"/api/kategorija/{id}",
                new { naziv = "Nauka" });
            Assert.Equal(HttpStatusCode.Conflict, resp.StatusCode);
        }

        [Fact]
        public async Task Update_IstaNazivIsteKategorije_Vraca200()
        {
            // US-33: Rubni slučaj — kontroler koristi k.Id != id u provjeri duplikata,
            // što znači da se može sačuvati kategorija s istim imenom (samo opis izmijenjen)
            var client = await BibliotekarClientAsync();
            var id = await KreirajKategorijuAsync(client, "Isti Naziv Test");

            var resp = await client.PutAsJsonAsync($"/api/kategorija/{id}",
                new { naziv = "Isti Naziv Test", opis = "Samo opis izmijenjen" });
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        }

        [Fact]
        public async Task Update_PrazanNaziv_Vraca400()
        {
            // US-33: Kada naziv je prazan, sistem prikazuje grešku i ne sprema promjene
            var client = await BibliotekarClientAsync();
            var id = await KreirajKategorijuAsync(client, "Za Prazan Naziv Test");

            var resp = await client.PutAsJsonAsync($"/api/kategorija/{id}",
                new { naziv = "" });
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        [Fact]
        public async Task Update_NepostojeciId_Vraca404()
        {
            // US-33: Kategorija koju uređujemo mora postojati u sistemu
            var client = await BibliotekarClientAsync();
            var resp = await client.PutAsJsonAsync("/api/kategorija/99999",
                new { naziv = "Bilo Sta" });
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }

        // ─── DELETE /api/kategorija/{id} — US-34 + US-32 ─────────────

        [Fact]
        public async Task Delete_BezAuth_Vraca401()
        {
            // US-08: Neprijavljeni korisnik ne može brisati kategorije
            var resp = await _factory.CreateClient().DeleteAsync("/api/kategorija/1");
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task Delete_KaoClan_Vraca403()
        {
            // Član nema ovlast za brisanje kategorija
            var client = await ClanClientAsync();
            var resp = await client.DeleteAsync("/api/kategorija/1");
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }

        [Fact]
        public async Task Delete_KategorijaBezKnjiga_Vraca200()
        {
            // US-34: Kada kategorija nema povezane knjige, brisanje je moguće
            var client = await BibliotekarClientAsync();
            var id = await KreirajKategorijuAsync(client, "Za Brisanje");

            var resp = await client.DeleteAsync($"/api/kategorija/{id}");
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        }

        [Fact]
        public async Task Delete_ObrisanaKategorija_ViseNijeVidljiva()
        {
            // US-34: Nakon brisanja kategorija se više ne prikazuje u listi
            var client = await BibliotekarClientAsync();
            var id = await KreirajKategorijuAsync(client, "Privremena Kategorija");

            await client.DeleteAsync($"/api/kategorija/{id}");

            var resp = await client.GetAsync($"/api/kategorija/{id}");
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }

        [Fact]
        public async Task Delete_KategorijaSaKnjigama_Vraca409()
        {
            // US-32: Sistem sprječava brisanje kategorije koja ima povezane knjige —
            // "Beletristika" (id=1) ima 1 seedovanu knjigu pa mora biti blokirana
            var client = await BibliotekarClientAsync();
            var resp = await client.DeleteAsync("/api/kategorija/1");
            Assert.Equal(HttpStatusCode.Conflict, resp.StatusCode);
        }

        [Fact]
        public async Task Delete_KategorijaSaKnjigama_PorukaSadrziRazlog()
        {
            // US-32: Prikazuje se jasna poruka zašto brisanje nije dozvoljeno
            var client = await BibliotekarClientAsync();
            var resp = await client.DeleteAsync("/api/kategorija/1");
            var body = await resp.Content.ReadAsStringAsync();

            Assert.Contains("knjig", body, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Delete_NepostojeciId_Vraca404()
        {
            // US-34: Pokušaj brisanja nepostojeće kategorije vraća 404
            var client = await BibliotekarClientAsync();
            var resp = await client.DeleteAsync("/api/kategorija/99999");
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }
    }

    // ─── Seeder ───────────────────────────────────────────────────────

    internal static class KategorijaDbSeeder
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
                ("bibliotekar@smartlib.ba", "Test123!"),
                ("clan@smartlib.ba",        "Test123!")
            };

            foreach (var (email, lozinka) in testAccounts)
            {
                var korisnik = db.Korisnici.FirstOrDefault(k => k.Email == email);
                if (korisnik != null)
                {
                    korisnik.LozinkaHash = PasswordHasher.HashPassword(lozinka);
                }
                else
                {
                    var ulogaId = email.StartsWith("bibliotekar") ? 2 : 1;
                    db.Korisnici.Add(new SmartLib.Core.Models.Korisnik
                    {
                        Ime = "Test",
                        Prezime = email.StartsWith("bibliotekar") ? "Bibliotekar" : "Clan",
                        Email = email,
                        LozinkaHash = PasswordHasher.HashPassword(lozinka),
                        UlogaId = ulogaId,
                        Status = "aktivan",
                        DatumKreiranja = DateTime.UtcNow
                    });
                }
            }
            db.SaveChanges();

            if (!db.Kategorije.Any())
            {
                db.Kategorije.AddRange(
                    new SmartLib.Core.Models.Kategorija { Id = 1, Naziv = "Beletristika", Opis = "Književnost" },
                    new SmartLib.Core.Models.Kategorija { Id = 2, Naziv = "Nauka", Opis = "Naučne knjige" }
                );
                db.SaveChanges();
            }

            // Jedna knjiga vezana za "Beletristika" (id=1) potrebna za:
            // - Delete_KategorijaSaKnjigama_Vraca409 (US-32)
            // - GetAll_SeededKategorijaSaKnjigama_ImaIspravanBrojKnjiga (US-31)
            if (!db.Knjige.Any())
            {
                db.Knjige.Add(new SmartLib.Core.Models.Knjiga
                {
                    Id = 1,
                    Naslov = "Seeded Knjiga",
                    Autor = "Seeded Autor",
                    Isbn = "9781111111111",
                    KategorijaId = 1,
                    GodinaIzdanja = 2020
                });
                db.SaveChanges();
            }
        }
    }
}