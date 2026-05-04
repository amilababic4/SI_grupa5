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
    public class KnjigaTestFixture : WebApplicationFactory<Program>
    {
        private readonly string _dbName = "KnjigaTestDb_" + Guid.NewGuid();

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

            using (var scope = host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                KnjigaDbSeeder.Seed(db);
            }

            return host;
        }
    }

    // ─── Tests ────────────────────────────────────────────────────────

    public class KnjigaIntegrationTests : IClassFixture<KnjigaTestFixture>
    {
        private readonly KnjigaTestFixture _factory;

        public KnjigaIntegrationTests(KnjigaTestFixture factory)
        {
            _factory = factory;
        }

        // ─── Auth helpers ─────────────────────────────────────────────

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

        // ─── GET /api/knjiga ──────────────────────────────────────────

        [Fact]
        public async Task GetAll_BezAuth_Vraca401()
        {
            var resp = await _factory.CreateClient().GetAsync("/api/knjiga");
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task GetAll_SaAuth_VracaObjektSaPoljimaKataloga()
        {
            var client = await BibliotekarClientAsync();
            var resp = await client.GetAsync("/api/knjiga");

            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.True(doc.RootElement.TryGetProperty("podaci", out _));
            Assert.True(doc.RootElement.TryGetProperty("ukupnoStavki", out _));
            Assert.True(doc.RootElement.TryGetProperty("ukupnoStranica", out _));
        }

        [Fact]
        public async Task GetAll_SeededKnjige_PrikazaneUListiKataloga()
        {
            var client = await BibliotekarClientAsync();
            var resp = await client.GetAsync("/api/knjiga");
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());

            Assert.True(doc.RootElement.GetProperty("podaci").GetArrayLength() >= 2);
        }

        [Fact]
        public async Task GetAll_PageSize1_VracaSamoJedanRezultat()
        {
            var client = await BibliotekarClientAsync();
            var resp = await client.GetAsync("/api/knjiga?page=1&pageSize=1");
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());

            Assert.Equal(1, doc.RootElement.GetProperty("podaci").GetArrayLength());
            Assert.True(doc.RootElement.GetProperty("ukupnoStranica").GetInt32() >= 2);
        }

        [Fact]
        public async Task GetAll_FilterNaslov_VracaSamoOdgovarajuceKnjige()
        {
            var client = await BibliotekarClientAsync();
            var resp = await client.GetAsync("/api/knjiga?naslov=Seeded");
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            var podaci = doc.RootElement.GetProperty("podaci");

            Assert.All(
                Enumerable.Range(0, podaci.GetArrayLength()).Select(i => podaci[i]),
                k => Assert.Contains(
                    "Seeded",
                    k.GetProperty("naslov").GetString(),
                    StringComparison.OrdinalIgnoreCase)
            );
        }

        // ─── GET /api/knjiga/{id} ─────────────────────────────────────

        [Fact]
        public async Task GetById_PostojeciId_VracaKnjigu()
        {
            var client = await BibliotekarClientAsync();
            var resp = await client.GetAsync("/api/knjiga/1");

            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.Equal("Seeded Knjiga Jedna", doc.RootElement.GetProperty("naslov").GetString());
        }

        [Fact]
        public async Task GetById_NepostojeciId_Vraca404()
        {
            var client = await BibliotekarClientAsync();
            var resp = await client.GetAsync("/api/knjiga/99999");
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }

        // ─── POST /api/knjiga ─────────────────────────────────────────

        [Fact]
        public async Task Create_BezAuth_Vraca401()
        {
            var resp = await _factory.CreateClient()
                .PostAsJsonAsync("/api/knjiga", NovaKnjiga("9780306406157"));
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task Create_KaoClan_Vraca403()
        {
            var client = await ClanClientAsync();
            var resp = await client.PostAsJsonAsync("/api/knjiga", NovaKnjiga("9780306406157"));
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }

        [Fact]
        public async Task Create_ValidanPayload_Vraca201()
        {
            var client = await BibliotekarClientAsync();
            var resp = await client.PostAsJsonAsync("/api/knjiga", NovaKnjiga("9780306406101"));

            Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.Equal("Nova Test Knjiga", doc.RootElement.GetProperty("naslov").GetString());
        }

        [Fact]
        public async Task Create_TriPrimjerka_VracaBrojPrimjerakaIBrojDostupnih()
        {
            var client = await BibliotekarClientAsync();
            var resp = await client.PostAsJsonAsync(
                "/api/knjiga", NovaKnjiga("9780306406102", brojPrimjeraka: 3));

            Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.Equal(3, doc.RootElement.GetProperty("brojPrimjeraka").GetInt32());
            Assert.Equal(3, doc.RootElement.GetProperty("brojDostupnih").GetInt32());
        }

        [Fact]
        public async Task Create_NulaPrimjeraka_KnjigaUKataloguAliBrojDostupnihJeNula()
        {
            var client = await BibliotekarClientAsync();
            var resp = await client.PostAsJsonAsync(
                "/api/knjiga", NovaKnjiga("9780306406103", brojPrimjeraka: 0));

            Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.Equal(0, doc.RootElement.GetProperty("brojPrimjeraka").GetInt32());
            Assert.Equal(0, doc.RootElement.GetProperty("brojDostupnih").GetInt32());
        }

        [Fact]
        public async Task Create_IsbnVecPostoji_Vraca409()
        {
            var client = await BibliotekarClientAsync();
            var resp = await client.PostAsJsonAsync("/api/knjiga", NovaKnjiga("9781111111111"));
            Assert.Equal(HttpStatusCode.Conflict, resp.StatusCode);
        }

        [Fact]
        public async Task Create_IsbnPrekratak_Vraca400()
        {
            var client = await BibliotekarClientAsync();
            var resp = await client.PostAsJsonAsync("/api/knjiga", NovaKnjiga("12345"));
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        [Fact]
        public async Task Create_Isbn13SaSlovom_Vraca400()
        {
            var client = await BibliotekarClientAsync();
            var resp = await client.PostAsJsonAsync("/api/knjiga", NovaKnjiga("978030640615X"));
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        [Fact]
        public async Task Create_Isbn10SaXNaKraju_Vraca201()
        {
            var client = await BibliotekarClientAsync();
            var resp = await client.PostAsJsonAsync("/api/knjiga", NovaKnjiga("080442957X"));
            Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
        }

        [Fact]
        public async Task Create_IsbnSaCrtama_NormalizujeISpremaSe()
        {
            var client = await BibliotekarClientAsync();
            var resp = await client.PostAsJsonAsync("/api/knjiga", NovaKnjiga("978-0-306-40615-7"));
            Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
        }

        [Fact]
        public async Task Create_NevalidnaKategorija_Vraca400()
        {
            var client = await BibliotekarClientAsync();
            var resp = await client.PostAsJsonAsync("/api/knjiga", new
            {
                naslov = "Nova Test Knjiga",
                autor = "Test Autor",
                isbn = "9780306400001",
                kategorijaId = 99999,
                brojPrimjeraka = 1
            });
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        // ─── PUT /api/knjiga/{id} ─────────────────────────────────────

        [Fact]
        public async Task Update_BezAuth_Vraca401()
        {
            var resp = await _factory.CreateClient()
                .PutAsJsonAsync("/api/knjiga/1", EditKnjiga(1));
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task Update_KaoClan_Vraca403()
        {
            var client = await ClanClientAsync();
            var resp = await client.PutAsJsonAsync("/api/knjiga/1", EditKnjiga(1));
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }

        [Fact]
        public async Task Update_ValidanPayload_Vraca204()
        {
            var client = await BibliotekarClientAsync();
            var resp = await client.PutAsJsonAsync("/api/knjiga/1", EditKnjiga(1));
            Assert.Equal(HttpStatusCode.NoContent, resp.StatusCode);
        }

        [Fact]
        public async Task Update_IdMismatch_Vraca400()
        {
            var client = await BibliotekarClientAsync();
            var resp = await client.PutAsJsonAsync("/api/knjiga/1", EditKnjiga(99));
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        [Fact]
        public async Task Update_NepostojeciId_Vraca404()
        {
            var client = await BibliotekarClientAsync();
            var resp = await client.PutAsJsonAsync("/api/knjiga/99999", EditKnjiga(99999));
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }

        [Fact]
        public async Task Update_IzmjenaVidljivaUKatalogu()
        {
            // Creates its own book so this test is independent of seeded id=1
            var client = await BibliotekarClientAsync();

            var createResp = await client.PostAsJsonAsync(
                "/api/knjiga", NovaKnjiga("9780306406200", naslov: "Originalni Naslov"));
            var created = JsonDocument.Parse(await createResp.Content.ReadAsStringAsync());
            var id = created.RootElement.GetProperty("id").GetInt32();

            await client.PutAsJsonAsync($"/api/knjiga/{id}", EditKnjiga(id, naslov: "Azurirani Naslov"));

            var getResp = await client.GetAsync($"/api/knjiga/{id}");
            var doc = JsonDocument.Parse(await getResp.Content.ReadAsStringAsync());
            Assert.Equal("Azurirani Naslov", doc.RootElement.GetProperty("naslov").GetString());
        }

        // ─── DELETE /api/knjiga/{id} ──────────────────────────────────

        [Fact]
        public async Task Delete_BezAuth_Vraca401()
        {
            var resp = await _factory.CreateClient().DeleteAsync("/api/knjiga/1");
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task Delete_KaoClan_Vraca403()
        {
            var client = await ClanClientAsync();
            var resp = await client.DeleteAsync("/api/knjiga/1");
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }

        [Fact]
        public async Task Delete_PostojeciIdBezZaduzenja_Vraca204()
        {
            // Always creates its own book — never deletes seeded data
            var client = await BibliotekarClientAsync();
            var createResp = await client.PostAsJsonAsync(
                "/api/knjiga", NovaKnjiga("9780306406201"));
            var created = JsonDocument.Parse(await createResp.Content.ReadAsStringAsync());
            var id = created.RootElement.GetProperty("id").GetInt32();

            var resp = await client.DeleteAsync($"/api/knjiga/{id}");
            Assert.Equal(HttpStatusCode.NoContent, resp.StatusCode);
        }

        [Fact]
        public async Task Delete_ObrisanaKnjiga_ViseNijeVidljivaUKatalogu()
        {
            var client = await BibliotekarClientAsync();
            var createResp = await client.PostAsJsonAsync(
                "/api/knjiga", NovaKnjiga("9780306406202"));
            var created = JsonDocument.Parse(await createResp.Content.ReadAsStringAsync());
            var id = created.RootElement.GetProperty("id").GetInt32();

            await client.DeleteAsync($"/api/knjiga/{id}");

            var getResp = await client.GetAsync($"/api/knjiga/{id}");
            Assert.Equal(HttpStatusCode.NotFound, getResp.StatusCode);
        }

        [Fact]
        public async Task Delete_NepostojeciId_Vraca404()
        {
            var client = await BibliotekarClientAsync();
            var resp = await client.DeleteAsync("/api/knjiga/99999");
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }

        // ─── GET filter po autoru ─────────────────────────────────────────

        [Fact]
        public async Task GetAll_FilterAutor_VracaSamoOdgovarajuceKnjige()
        {
            var client = await BibliotekarClientAsync();
            var resp = await client.GetAsync("/api/knjiga?autor=Seeded");
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            var podaci = doc.RootElement.GetProperty("podaci");

            Assert.True(podaci.GetArrayLength() >= 1);
            Assert.All(
                Enumerable.Range(0, podaci.GetArrayLength()).Select(i => podaci[i]),
                k => Assert.Contains(
                    "Seeded",
                    k.GetProperty("autor").GetString(),
                    StringComparison.OrdinalIgnoreCase)
            );
        }

        [Fact]
        public async Task GetAll_FilterNaslovNePostoji_VracaPrazanRezultat()
        {
            var client = await BibliotekarClientAsync();
            var resp = await client.GetAsync("/api/knjiga?naslov=XYZNePosotjiNigdje999");
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());

            Assert.Equal(0, doc.RootElement.GetProperty("podaci").GetArrayLength());
            Assert.Equal(0, doc.RootElement.GetProperty("ukupnoStavki").GetInt32());
        }

        // ─── DELETE sa aktivnim zaduženjima (US-28) ───────────────────────

        [Fact]
        public async Task Delete_KnjigaSaAktivnimZaduzenjem_Vraca400()
        {
            // 1. Kreiraj knjigu
            var client = await BibliotekarClientAsync();
            var createResp = await client.PostAsJsonAsync(
                "/api/knjiga", NovaKnjiga("9780306406300", brojPrimjeraka: 1));
            createResp.EnsureSuccessStatusCode();
            var created = JsonDocument.Parse(await createResp.Content.ReadAsStringAsync());
            var knjigaId = created.RootElement.GetProperty("id").GetInt32();

            // 2. Direktno u bazu dodaj aktivno zaduženje za primjerak te knjige
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                LoanDbSeeder.SeedActiveLoan(db, knjigaId);
            }

            // 3. Pokušaj brisanja — mora vratiti 400
            var resp = await client.DeleteAsync($"/api/knjiga/{knjigaId}");
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        [Fact]
        public async Task Delete_KnjigaSaZavrsanimZaduzenjem_Vraca204()
        {
            // Završeno zaduženje ne smije blokirati brisanje
            var client = await BibliotekarClientAsync();
            var createResp = await client.PostAsJsonAsync(
                "/api/knjiga", NovaKnjiga("9780306406301", brojPrimjeraka: 1));
            createResp.EnsureSuccessStatusCode();
            var created = JsonDocument.Parse(await createResp.Content.ReadAsStringAsync());
            var knjigaId = created.RootElement.GetProperty("id").GetInt32();

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                LoanDbSeeder.SeedCompletedLoan(db, knjigaId);
            }

            var resp = await client.DeleteAsync($"/api/knjiga/{knjigaId}");
            Assert.Equal(HttpStatusCode.NoContent, resp.StatusCode);
        }

        // ─── CREATE duplikat ISBN — test kreira vlastiti duplikat ─────────

        [Fact]
        public async Task Create_DuplikatIsbn_SelfContained_Vraca409()
        {
            var client = await BibliotekarClientAsync();
            const string isbn = "9780306406400";

            var prvi = await client.PostAsJsonAsync("/api/knjiga", NovaKnjiga(isbn));
            Assert.Equal(HttpStatusCode.Created, prvi.StatusCode);

            var drugi = await client.PostAsJsonAsync("/api/knjiga", NovaKnjiga(isbn));
            Assert.Equal(HttpStatusCode.Conflict, drugi.StatusCode);
        }

        // ─── UPDATE provjera kategorije ───────────────────────────────────

        [Fact]
        public async Task Update_NevalidnaKategorija_Vraca400()
        {
            var client = await BibliotekarClientAsync();
            var resp = await client.PutAsJsonAsync("/api/knjiga/1", new
            {
                id = 1,
                naslov = "Test",
                autor = "Test",
                kategorijaId = 99999,
                izdavac = "Test",
                godinaIzdanja = 2024
            });
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }



        // ─── Payload helpers ──────────────────────────────────────────

        private static object NovaKnjiga(
            string isbn,
            int brojPrimjeraka = 2,
            string naslov = "Nova Test Knjiga") => new
            {
                naslov,
                autor = "Test Autor",
                isbn,
                kategorijaId = 1,
                izdavac = "Test Izdavac",
                godinaIzdanja = 2023,
                brojPrimjeraka
            };

        private static object EditKnjiga(int id, string naslov = "Izmijenjena Knjiga") => new
        {
            id,
            naslov,
            autor = "Novi Autor",
            kategorijaId = 1,
            izdavac = "Novi Izdavac",
            godinaIzdanja = 2024
        };
    }

    // ─── Seeder ───────────────────────────────────────────────────────
    internal static class KnjigaDbSeeder
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
                    var uloga = db.Uloge.FirstOrDefault(u =>
                        u.Naziv == (email.StartsWith("bibliotekar") ? "Bibliotekar" : "Član"));

                    db.Korisnici.Add(new SmartLib.Core.Models.Korisnik
                    {
                        Ime = "Test",
                        Prezime = email.StartsWith("bibliotekar") ? "Bibliotekar" : "Clan",
                        Email = email,
                        LozinkaHash = PasswordHasher.HashPassword(lozinka),
                        UlogaId = uloga?.Id ?? 1,
                        Status = "aktivan",
                        DatumKreiranja = DateTime.UtcNow
                    });
                }
            }
            db.SaveChanges();

            if (!db.Kategorije.Any())
            {
                db.Kategorije.AddRange(
                    new SmartLib.Core.Models.Kategorija { Id = 1, Naziv = "Beletristika" },
                    new SmartLib.Core.Models.Kategorija { Id = 2, Naziv = "Nauka" }
                );
                db.SaveChanges();
            }

            if (!db.Knjige.Any())
            {
                db.Knjige.AddRange(
                    new SmartLib.Core.Models.Knjiga
                    {
                        Id = 1,
                        Naslov = "Seeded Knjiga Jedna",
                        Autor = "Seeded Autor",
                        Isbn = "9781111111111",
                        KategorijaId = 1,
                        Izdavac = "Seeded Izdavac",
                        GodinaIzdanja = 2020
                    },
                    new SmartLib.Core.Models.Knjiga
                    {
                        Id = 2,
                        Naslov = "Seeded Knjiga Dva",
                        Autor = "Seeded Autor",
                        Isbn = "9782222222222",
                        KategorijaId = 2,
                        GodinaIzdanja = 2021
                    }
                );
                db.SaveChanges();
            }
        }
    }

    // ─── Loan Seeder helper ───────────────────────────────────────────
    internal static class LoanDbSeeder
    {
        public static void SeedActiveLoan(ApplicationDbContext db, int knjigaId)
        {
            var primjerak = db.Primjerci.FirstOrDefault(p => p.KnjigaId == knjigaId);
            if (primjerak == null) return;

            db.Zaduzenja.Add(new SmartLib.Core.Models.Zaduzenje
            {
                PrimjerakId = primjerak.Id,
                KorisnikId = db.Korisnici.First().Id,
                DatumZaduzivanja = DateTime.UtcNow.AddDays(-5),
                DatumPlaniranogVracanja = DateTime.UtcNow.AddDays(9),
                Status = "aktivno"
            });
            db.SaveChanges();
        }

        public static void SeedCompletedLoan(ApplicationDbContext db, int knjigaId)
        {
            var primjerak = db.Primjerci.FirstOrDefault(p => p.KnjigaId == knjigaId);
            if (primjerak == null) return;

            db.Zaduzenja.Add(new SmartLib.Core.Models.Zaduzenje
            {
                PrimjerakId = primjerak.Id,
                KorisnikId = db.Korisnici.First().Id,
                DatumZaduzivanja = DateTime.UtcNow.AddDays(-14),
                DatumPlaniranogVracanja = DateTime.UtcNow.AddDays(-7),
                DatumStvarnogVracanja = DateTime.UtcNow.AddDays(-7),
                Status = "završeno"
            });
            db.SaveChanges();
        }
    }
}