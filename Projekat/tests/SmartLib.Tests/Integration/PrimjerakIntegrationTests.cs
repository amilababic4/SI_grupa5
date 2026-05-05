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
    public class PrimjerakTestFixture : WebApplicationFactory<Program>
    {
        private readonly string _dbName = "PrimjerakTestDb_" + Guid.NewGuid();

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
            PrimjerakDbSeeder.Seed(db);

            return host;
        }
    }

    // ─── Testovi ──────────────────────────────────────────────────────

    public class PrimjerakIntegrationTests : IClassFixture<PrimjerakTestFixture>
    {
        private readonly PrimjerakTestFixture _factory;

        public PrimjerakIntegrationTests(PrimjerakTestFixture factory)
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

        // Kreira novu knjigu kroz API i vraća njen ID.
        // Koristi se kako testovi primjeraka ne bi ovisili o seedovanim knjigama.
        private async Task<int> KreirajKnjiguAsync(HttpClient client, string isbn)
        {
            var resp = await client.PostAsJsonAsync("/api/knjiga", new
            {
                naslov = "Test Knjiga Za Primjerke",
                autor = "Test Autor",
                isbn,
                kategorijaId = 1,
                godinaIzdanja = 2023,
                brojPrimjeraka = 0   // kreiramo knjgu bez primjeraka, primjerke dodajemo zasebno
            });
            resp.EnsureSuccessStatusCode();
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            return doc.RootElement.GetProperty("id").GetInt32();
        }

        // Dodaje aktivno zaduženje direktno u bazu za prvi primjerak date knjige.
        // Koristi se u testovima deaktivacije koji zahtijevaju primjerak s aktivnim zaduženjem.
        private void SeedAktivnoZaduzenje(int knjigaId)
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var primjerak = db.Primjerci.FirstOrDefault(p => p.KnjigaId == knjigaId);
            if (primjerak == null) return;

            db.Zaduzenja.Add(new SmartLib.Core.Models.Zaduzenje
            {
                PrimjerakId = primjerak.Id,
                KorisnikId = db.Korisnici.First().Id,
                DatumZaduzivanja = DateTime.UtcNow.AddDays(-3),
                DatumPlaniranogVracanja = DateTime.UtcNow.AddDays(11),
                Status = "aktivno"
            });
            db.SaveChanges();
        }

        // ─── GET /api/primjerak/knjiga/{knjigaId} — US-22, US-23 ──────

        [Fact]
        public async Task GetByKnjiga_BezAuth_Vraca401()
        {
            // US-08: Neprijavljeni korisnik ne smije pristupiti listi primjeraka
            var resp = await _factory.CreateClient()
                .GetAsync("/api/primjerak/knjiga/1");
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task GetByKnjiga_KaoClan_Vraca403()
        {
            // PrimjerakController zahtijeva ulogu Bibliotekar ili Administrator
            var client = await ClanClientAsync();
            var resp = await client.GetAsync("/api/primjerak/knjiga/1");
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }

        [Fact]
        public async Task GetByKnjiga_PostojecaKnjiga_VracaListuPrimjeraka()
        {
            // US-22: Kada korisnik otvori detalje knjige, sistem prikazuje listu svih primjeraka
            var client = await BibliotekarClientAsync();
            var resp = await client.GetAsync("/api/primjerak/knjiga/1");

            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            // Seedovana knjiga id=1 ima 2 primjerka
            Assert.Equal(2, doc.RootElement.GetArrayLength());
        }

        [Fact]
        public async Task GetByKnjiga_SvakiPrimjerakImaOcekivanaPolja()
        {
            // US-22: Svaki primjerak mora biti jasno prikazan s jedinstvenim identifikatorom
            // US-23: Mora biti prikazan trenutni status svakog primjerka
            var client = await BibliotekarClientAsync();
            var resp = await client.GetAsync("/api/primjerak/knjiga/1");
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            var prvi = doc.RootElement[0];

            Assert.True(prvi.TryGetProperty("id", out _));
            Assert.True(prvi.TryGetProperty("inventarniBroj", out _));
            Assert.True(prvi.TryGetProperty("status", out _));
            Assert.True(prvi.TryGetProperty("knjigaId", out _));
        }

        [Fact]
        public async Task GetByKnjiga_SeededPrimjerci_ImajuStatusDostupan()
        {
            // US-23: Kada primjerak knjige postoji u sistemu, mora imati definisan status
            var client = await BibliotekarClientAsync();
            var resp = await client.GetAsync("/api/primjerak/knjiga/1");
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());

            Assert.All(
                doc.RootElement.EnumerateArray(),
                p => Assert.Equal("dostupan", p.GetProperty("status").GetString())
            );
        }

        [Fact]
        public async Task GetByKnjiga_NePostojecaKnjiga_Vraca404()
        {
            // Knjiga koja se traži mora postojati — controller provjerava GetByIdAsync
            var client = await BibliotekarClientAsync();
            var resp = await client.GetAsync("/api/primjerak/knjiga/99999");
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }

        [Fact]
        public async Task GetByKnjiga_KnjigaBezPrimjeraka_VracaPrazanNiz()
        {
            // US-22: Knjiga može postojati u katalogu bez primjeraka (brojPrimjeraka=0)
            var client = await BibliotekarClientAsync();

            // Knjiga id=2 seedovana bez primjeraka
            var resp = await client.GetAsync("/api/primjerak/knjiga/2");
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.Equal(0, doc.RootElement.GetArrayLength());
        }

        [Fact]
        public async Task GetByKnjiga_PrimjerciPripadajuSamoTojKnjizi()
        {
            // US-22: Sistem ne smije prikazivati primjerke koji ne pripadaju odabranoj knjizi
            var client = await BibliotekarClientAsync();
            var resp = await client.GetAsync("/api/primjerak/knjiga/1");
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());

            Assert.All(
                doc.RootElement.EnumerateArray(),
                p => Assert.Equal(1, p.GetProperty("knjigaId").GetInt32())
            );
        }

        // ─── GET /api/primjerak/{id} — US-22, US-23 ──────────────────

        [Fact]
        public async Task GetById_BezAuth_Vraca401()
        {
            // US-08: Direktan URL ne zaobilazi autentifikaciju
            var resp = await _factory.CreateClient().GetAsync("/api/primjerak/1");
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task GetById_PostojeciPrimjerak_VracaPrimjerak()
        {
            // US-22: Pregled pojedinačnog primjerka s inventarnim brojem i statusom
            var client = await BibliotekarClientAsync();
            var resp = await client.GetAsync("/api/primjerak/1");

            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.True(doc.RootElement.TryGetProperty("inventarniBroj", out _));
            Assert.True(doc.RootElement.TryGetProperty("status", out _));
        }

        [Fact]
        public async Task GetById_VracaNazivKnjige()
        {
            // US-22: Response uključuje naziv knjige kojoj primjerak pripada
            var client = await BibliotekarClientAsync();
            var resp = await client.GetAsync("/api/primjerak/1");
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());

            Assert.True(doc.RootElement.TryGetProperty("knjiga", out var knjiga));
            Assert.False(string.IsNullOrEmpty(knjiga.GetString()));
        }

        [Fact]
        public async Task GetById_NepostojeciId_Vraca404()
        {
            // Primjerak koji se traži mora postojati u sistemu
            var client = await BibliotekarClientAsync();
            var resp = await client.GetAsync("/api/primjerak/99999");
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }

        // ─── POST /api/primjerak — US-21: Dodavanje primjeraka ────────

        [Fact]
        public async Task Create_BezAuth_Vraca401()
        {
            // US-08: Neprijavljeni korisnik ne može dodavati primjerke
            var resp = await _factory.CreateClient()
                .PostAsJsonAsync("/api/primjerak", new { knjigaId = 1, brojNovih = 1 });
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task Create_KaoClan_Vraca403()
        {
            // Član nema ovlast za kreiranje primjeraka
            var client = await ClanClientAsync();
            var resp = await client.PostAsJsonAsync("/api/primjerak",
                new { knjigaId = 1, brojNovih = 1 });
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }

        [Fact]
        public async Task Create_ValidanPayload_Vraca201()
        {
            // US-21: Kada bibliotekar doda primjerke, kreira se odgovarajući broj zapisa
            var client = await BibliotekarClientAsync();
            var knjigaId = await KreirajKnjiguAsync(client, "9780306406501");

            var resp = await client.PostAsJsonAsync("/api/primjerak",
                new { knjigaId, brojNovih = 2 });

            Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.Equal(2, doc.RootElement.GetProperty("kreirani").GetArrayLength());
        }

        [Fact]
        public async Task Create_NovoKreiraniPrimjerci_ImajuStatusDostupan()
        {
            // US-21 + US-23: Svaki novokreirani primjerak mora imati status "dostupan"
            var client = await BibliotekarClientAsync();
            var knjigaId = await KreirajKnjiguAsync(client, "9780306406502");

            var resp = await client.PostAsJsonAsync("/api/primjerak",
                new { knjigaId, brojNovih = 3 });

            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.All(
                doc.RootElement.GetProperty("kreirani").EnumerateArray(),
                p => Assert.Equal("dostupan", p.GetProperty("status").GetString())
            );
        }

        [Fact]
        public async Task Create_NovoKreiraniPrimjerci_PrikazaniUGetByKnjiga()
        {
            // US-21: Nakon kreiranja primjerci moraju biti vidljivi u pregledu knjige
            var client = await BibliotekarClientAsync();
            var knjigaId = await KreirajKnjiguAsync(client, "9780306406503");

            await client.PostAsJsonAsync("/api/primjerak",
                new { knjigaId, brojNovih = 2 });

            var resp = await client.GetAsync($"/api/primjerak/knjiga/{knjigaId}");
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.Equal(2, doc.RootElement.GetArrayLength());
        }

        [Fact]
        public async Task Create_NePostojecaKnjiga_Vraca400()
        {
            // US-21: Sistem ne smije dozvoliti kreiranje primjeraka bez povezane knjige
            var client = await BibliotekarClientAsync();
            var resp = await client.PostAsJsonAsync("/api/primjerak",
                new { knjigaId = 99999, brojNovih = 1 });
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        [Fact]
        public async Task Create_NulaPrimjeraka_Vraca400()
        {
            // Broj novih primjeraka mora biti između 1 i 50 — 0 nije dozvoljeno
            var client = await BibliotekarClientAsync();
            var knjigaId = await KreirajKnjiguAsync(client, "9780306406504");

            var resp = await client.PostAsJsonAsync("/api/primjerak",
                new { knjigaId, brojNovih = 0 });
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        [Fact]
        public async Task Create_PrekoMaxPrimjeraka_Vraca400()
        {
            // Controller dozvoljava maksimalno 50 primjeraka u jednom zahtjevu
            var client = await BibliotekarClientAsync();
            var knjigaId = await KreirajKnjiguAsync(client, "9780306406505");

            var resp = await client.PostAsJsonAsync("/api/primjerak",
                new { knjigaId, brojNovih = 51 });
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        [Fact]
        public async Task Create_InventarniBrojJeJedinstven()
        {
            // US-21: Svaki primjerak mora biti zaseban zapis s jedinstvenim inventarnim brojem
            var client = await BibliotekarClientAsync();
            var knjigaId = await KreirajKnjiguAsync(client, "9780306406506");

            var resp = await client.PostAsJsonAsync("/api/primjerak",
                new { knjigaId, brojNovih = 3 });

            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            var invBrojevi = doc.RootElement
                .GetProperty("kreirani")
                .EnumerateArray()
                .Select(p => p.GetProperty("inventarniBroj").GetString())
                .ToList();

            // Svi inventarni brojevi moraju biti različiti
            Assert.Equal(invBrojevi.Count, invBrojevi.Distinct().Count());
        }

        // ─── POST /api/primjerak/{id}/deaktiviraj — US-24 ────────────

        [Fact]
        public async Task Deaktiviraj_BezAuth_Vraca401()
        {
            // US-08: Neprijavljeni korisnik ne može deaktivirati primjerke
            var resp = await _factory.CreateClient()
                .PostAsync("/api/primjerak/1/deaktiviraj", null);
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task Deaktiviraj_KaoClan_Vraca403()
        {
            // Član nema ovlast za deaktivaciju primjeraka
            var client = await ClanClientAsync();
            var resp = await client.PostAsync("/api/primjerak/1/deaktiviraj", null);
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }

        [Fact]
        public async Task Deaktiviraj_DostupanPrimjerak_Vraca200()
        {
            // US-24: Bibliotekar može deaktivirati primjerak koji je dostupan
            var client = await BibliotekarClientAsync();
            var knjigaId = await KreirajKnjiguAsync(client, "9780306406601");
            await client.PostAsJsonAsync("/api/primjerak",
                new { knjigaId, brojNovih = 1 });

            var primjerci = await client.GetAsync($"/api/primjerak/knjiga/{knjigaId}");
            var doc = JsonDocument.Parse(await primjerci.Content.ReadAsStringAsync());
            var primjerakId = doc.RootElement[0].GetProperty("id").GetInt32();

            var resp = await client.PostAsync($"/api/primjerak/{primjerakId}/deaktiviraj", null);
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        }

        [Fact]
        public async Task Deaktiviraj_StatusSeMijenjaNaDeaktiviran()
        {
            // US-24: Nakon deaktivacije status primjerka mora biti "deaktiviran"
            var client = await BibliotekarClientAsync();
            var knjigaId = await KreirajKnjiguAsync(client, "9780306406602");
            await client.PostAsJsonAsync("/api/primjerak",
                new { knjigaId, brojNovih = 1 });

            var primjerci = await client.GetAsync($"/api/primjerak/knjiga/{knjigaId}");
            var docPrimjerci = JsonDocument.Parse(await primjerci.Content.ReadAsStringAsync());
            var primjerakId = docPrimjerci.RootElement[0].GetProperty("id").GetInt32();

            await client.PostAsync($"/api/primjerak/{primjerakId}/deaktiviraj", null);

            var resp = await client.GetAsync($"/api/primjerak/{primjerakId}");
            var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.Equal("deaktiviran", doc.RootElement.GetProperty("status").GetString());
        }

        [Fact]
        public async Task Deaktiviraj_DeaktiviraniPrimjerak_NijeViseUBrojuDostupnih()
        {
            // US-24: Deaktivirani primjerak se više ne računa kao dostupan za zaduživanje
            var client = await BibliotekarClientAsync();
            var knjigaId = await KreirajKnjiguAsync(client, "9780306406603");
            await client.PostAsJsonAsync("/api/primjerak",
                new { knjigaId, brojNovih = 2 });

            // Uzmi prvi primjerak i deaktiviraj ga
            var primjerci = await client.GetAsync($"/api/primjerak/knjiga/{knjigaId}");
            var docP = JsonDocument.Parse(await primjerci.Content.ReadAsStringAsync());
            var primjerakId = docP.RootElement[0].GetProperty("id").GetInt32();
            await client.PostAsync($"/api/primjerak/{primjerakId}/deaktiviraj", null);

            // Provjeri u katalogu da se brojDostupnih smanjio
            var knjigaResp = await client.GetAsync($"/api/knjiga/{knjigaId}");
            var docKnjiga = JsonDocument.Parse(await knjigaResp.Content.ReadAsStringAsync());
            Assert.Equal(1, docKnjiga.RootElement.GetProperty("brojDostupnih").GetInt32());
        }

        [Fact]
        public async Task Deaktiviraj_VecDeaktiviranPrimjerak_Vraca409()
        {
            // US-24: Sistem ne dozvoljava dvostruku deaktivaciju istog primjerka
            var client = await BibliotekarClientAsync();
            var knjigaId = await KreirajKnjiguAsync(client, "9780306406604");
            await client.PostAsJsonAsync("/api/primjerak",
                new { knjigaId, brojNovih = 1 });

            var primjerci = await client.GetAsync($"/api/primjerak/knjiga/{knjigaId}");
            var docP = JsonDocument.Parse(await primjerci.Content.ReadAsStringAsync());
            var primjerakId = docP.RootElement[0].GetProperty("id").GetInt32();

            // Prva deaktivacija
            await client.PostAsync($"/api/primjerak/{primjerakId}/deaktiviraj", null);
            // Druga deaktivacija — mora biti odbijena
            var resp = await client.PostAsync($"/api/primjerak/{primjerakId}/deaktiviraj", null);
            Assert.Equal(HttpStatusCode.Conflict, resp.StatusCode);
        }

        [Fact]
        public async Task Deaktiviraj_PrimjerakSAktivnimZaduzenjem_Vraca409()
        {
            // US-24: Sistem ne dozvoljava deaktivaciju primjerka koji je trenutno zadužen
            var client = await BibliotekarClientAsync();
            var knjigaId = await KreirajKnjiguAsync(client, "9780306406605");
            await client.PostAsJsonAsync("/api/primjerak",
                new { knjigaId, brojNovih = 1 });

            var primjerci = await client.GetAsync($"/api/primjerak/knjiga/{knjigaId}");
            var docP = JsonDocument.Parse(await primjerci.Content.ReadAsStringAsync());
            var primjerakId = docP.RootElement[0].GetProperty("id").GetInt32();

            // Dodaj aktivno zaduženje direktno u bazu
            SeedAktivnoZaduzenje(knjigaId);

            var resp = await client.PostAsync($"/api/primjerak/{primjerakId}/deaktiviraj", null);
            Assert.Equal(HttpStatusCode.Conflict, resp.StatusCode);
        }

        [Fact]
        public async Task Deaktiviraj_NepostojeciId_Vraca404()
        {
            // Primjerak koji se deaktivira mora postojati u sistemu
            var client = await BibliotekarClientAsync();
            var resp = await client.PostAsync("/api/primjerak/99999/deaktiviraj", null);
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }
    }

    // ─── Seeder ───────────────────────────────────────────────────────
    // Knjiga id=1 ima 2 dostupna primjerka — koristi se u GET testovima.
    // Knjiga id=2 nema primjeraka — testira GetByKnjiga s praznim nizom.
    // Sve ostale knjige kreiraju testovi kroz API kako bi bili nezavisni.

    internal static class PrimjerakDbSeeder
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
                db.Kategorije.Add(
                    new SmartLib.Core.Models.Kategorija { Id = 1, Naziv = "Beletristika" }
                );
                db.SaveChanges();
            }

            if (!db.Knjige.Any())
            {
                db.Knjige.AddRange(
                    // Knjiga s primjercima — za GET testove
                    new SmartLib.Core.Models.Knjiga
                    {
                        Id = 1,
                        Naslov = "Knjiga Sa Primjercima",
                        Autor = "Test Autor",
                        Isbn = "9781111111111",
                        KategorijaId = 1,
                        GodinaIzdanja = 2020
                    },
                    // Knjiga bez primjeraka — za test praznog niza
                    new SmartLib.Core.Models.Knjiga
                    {
                        Id = 2,
                        Naslov = "Knjiga Bez Primjeraka",
                        Autor = "Test Autor",
                        Isbn = "9782222222222",
                        KategorijaId = 1,
                        GodinaIzdanja = 2021
                    }
                );
                db.SaveChanges();
            }

            if (!db.Primjerci.Any())
            {
                db.Primjerci.AddRange(
                    new SmartLib.Core.Models.Primjerak
                    {
                        Id = 1,
                        KnjigaId = 1,
                        InventarniBroj = "INV-1-001",
                        Status = "dostupan",
                        DatumNabave = DateTime.UtcNow
                    },
                    new SmartLib.Core.Models.Primjerak
                    {
                        Id = 2,
                        KnjigaId = 1,
                        InventarniBroj = "INV-1-002",
                        Status = "dostupan",
                        DatumNabave = DateTime.UtcNow
                    }
                );
                db.SaveChanges();
            }
        }
    }
}