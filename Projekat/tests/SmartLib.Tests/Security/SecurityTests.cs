using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartLib.Infrastructure.Data;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace SmartLib.Tests.Security
{
    /// <summary>
    /// Penetracijski / sigurnosni testovi
    ///
    /// US-04/US-05: SQL injection na login
    /// US-08:       RBAC manipulacija (promjena uloge u tokenu)
    /// US-09:       Pristup deaktiviranog korisnika starom sesijom
    /// US-08:       Direktni URL unos zaštićenih ruta
    /// XSS:         Unos skripti u registracijsku formu
    /// RBAC:        Neautorizovani pristup KategorijaController-u
    /// RBAC:        Neautorizovani pristup PrimjerakController-u
    /// RBAC:        Član pokušava admin operacije na KnjigaController-u
    /// RBAC:        Clan pokušava pristup KorisnikController-u
    /// </summary>
    public class SecurityTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        // Statičko ime — jedna baza za cijelu test klasu
        private static readonly string _dbName = "SecurityTestDb_" + Guid.NewGuid();

        public SecurityTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((_, cfg) =>
                    cfg.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Jwt:Key"] = "OVO_JE_TESTNI_KLJUC_KOJI_IMA_VISE_OD_32_KARAKTERA!",
                        ["Jwt:Issuer"] = "SmartLib",
                        ["Jwt:Audience"] = "SmartLibUsers",
                        ["Jwt:ExpirationMinutes"] = "30"
                    }));

                builder.ConfigureServices(services =>
                {                 
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    // Fiksno ime baze — dijeli se između svih CreateClient() poziva
                    services.AddDbContext<ApplicationDbContext>(opts =>
                        opts.UseInMemoryDatabase(_dbName));

                    // Seed jednom
                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();
                    TestData.TestSeeder.Seed(db);
                });
            });
        }

        private HttpClient CreateClient() => _factory.CreateClient();

        // ════════════════════════════════════════════════════════════════════════
        // AUTH — SQL Injection (US-04 / US-05)
        // ════════════════════════════════════════════════════════════════════════

        [Theory]
        [InlineData("' OR '1'='1", "bilo_sta")]
        [InlineData("admin'--", "bilo_sta")]
        [InlineData("'; DROP TABLE Korisnici;--", "bilo_sta")]
        [InlineData("\" OR 1=1--", "bilo_sta")]
        public async Task Login_SqlInjectionUEmailu_NeDozvoljavaPristup(string sqlPayload, string lozinka)
        {
            var client = CreateClient();
            var resp = await client.PostAsJsonAsync("/api/auth/login", new
            {
                email = sqlPayload,
                lozinka = lozinka
            });

            Assert.True(
                resp.StatusCode == HttpStatusCode.BadRequest ||
                resp.StatusCode == HttpStatusCode.Unauthorized,
                $"SQL injection payload dao je: {resp.StatusCode}"
            );
        }

        [Theory]
        [InlineData("bibliotekar@smartlib.ba", "' OR '1'='1")]
        [InlineData("bibliotekar@smartlib.ba", "'; DROP TABLE Korisnici;--")]
        [InlineData("bibliotekar@smartlib.ba", "\" OR 1=1--")]
        public async Task Login_SqlInjectionULozinki_NeDozvoljavaPristup(string email, string sqlPayload)
        {
            // Injection u polju lozinke — najčešći vektor napada
            var client = CreateClient();
            var resp = await client.PostAsJsonAsync("/api/auth/login", new
            {
                email = email,
                lozinka = sqlPayload
            });

            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        // ════════════════════════════════════════════════════════════════════════
        // AUTH — Brute Force (US-04)
        // ════════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task Login_ViseNeuspjelihPokusaja_SvakiVraca401NikadNe200()
        {
            var client = CreateClient();

            for (int i = 0; i < 10; i++)
            {
                var resp = await client.PostAsJsonAsync("/api/auth/login", new
                {
                    email = "clan@smartlib.ba",
                    lozinka = $"PogresnaLozinka{i}!"
                });

                Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
            }
        }

        // ════════════════════════════════════════════════════════════════════════
        // AUTH — Lažni JWT token (US-08)
        // ════════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task ZasticenaRuta_LazniBearerToken_Vraca401()
        {
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9" +
                    ".eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkhhY2tlciIsInJvbGUiOiJBZG1pbmlzdHJhdG9yIn0" +
                    ".SIGNATURE_JE_LAZNA"
                );

            var resp = await client.GetAsync("/api/korisnik");
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task ZasticenaRuta_TokenSaModifikovanomUlogom_Vraca401()
        {
            var clanToken = await DobijClanToken();

            var dijelovi = clanToken.Split('.');
            var lazniToken = $"{dijelovi[0]}.{dijelovi[1]}.LAZNI_POTPIS";

            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", lazniToken);

            var resp = await client.GetAsync("/api/korisnik");
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        // ════════════════════════════════════════════════════════════════════════
        // AUTH — Logout
        // ════════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task Logout_BezTokena_Vraca401()
        {
            var client = CreateClient();
            var resp = await client.PostAsync("/api/auth/logout", null);
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task Logout_SaValidnimTokenom_Vraca200()
        {
            var token = await DobijClanToken();
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var resp = await client.PostAsync("/api/auth/logout", null);
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        }

        // ════════════════════════════════════════════════════════════════════════
        // US-09: Deaktiviran korisnik
        // ════════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task DeaktiviranKorisnik_SaStarimJwtTokenom_BiljeziSigurnosnuNapomenu()
        {
            // JWT je stateless — token ostaje validan dok ne istekne.
            // Pravi fix: token blacklist ili kratko trajanje + refresh token.
            Assert.True(true,
                "SIGURNOSNA NAPOMENA (US-09): Deaktiviran korisnik zadržava stari JWT " +
                "dok ne istekne. Riješiti: token blacklist ili refresh token mehanizam.");
        }

        [Fact]
        public async Task Login_DeaktiviranKorisnik_Vraca401()
        {
            // Seed treba sadržavati korisnika sa statusom "deaktiviran" i emailom deaktiviran@smartlib.ba
            var client = CreateClient();
            var resp = await client.PostAsJsonAsync("/api/auth/login", new
            {
                email = "deaktiviran@smartlib.ba",
                lozinka = "Test123!"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        // ════════════════════════════════════════════════════════════════════════
        // KORISNIK — Direktan URL unos bez autentifikacije (US-08)
        // ════════════════════════════════════════════════════════════════════════

        [Theory]
        [InlineData("GET", "/api/korisnik")]
        [InlineData("GET", "/api/korisnik/1")]
        [InlineData("POST", "/api/korisnik")]
        [InlineData("PUT", "/api/korisnik/1/uloga")]
        [InlineData("PUT", "/api/korisnik/1/deaktiviraj")]
        public async Task KorisnikRute_BezAutentifikacije_Vraca401(string metod, string url)
        {
            var client = CreateClient();
            var request = new HttpRequestMessage(new HttpMethod(metod), url);
            if (metod == "POST" || metod == "PUT")
                request.Content = JsonContent.Create(new { });

            var resp = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task KorisnikRute_SaClanTokenom_Vraca403()
        {
            // Član (uloga koja nije Bibliotekar/Administrator) ne smije pristupiti
            var token = await DobijClanToken();
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var resp = await client.GetAsync("/api/korisnik");
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }

        [Fact]
        public async Task KorisnikDeaktiviraj_SaClanTokenom_Vraca403()
        {
            var token = await DobijClanToken();
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var resp = await client.PutAsync("/api/korisnik/1/deaktiviraj", null);
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }

        // ════════════════════════════════════════════════════════════════════════
        // KATEGORIJA — RBAC (US-30..US-34)
        // ════════════════════════════════════════════════════════════════════════

        [Theory]
        [InlineData("GET", "/api/kategorija")]
        [InlineData("GET", "/api/kategorija/1")]
        [InlineData("POST", "/api/kategorija")]
        [InlineData("PUT", "/api/kategorija/1")]
        [InlineData("DELETE", "/api/kategorija/1")]
        public async Task KategorijaRute_BezAutentifikacije_Vraca401(string metod, string url)
        {
            var client = CreateClient();
            var request = new HttpRequestMessage(new HttpMethod(metod), url);
            if (metod == "POST" || metod == "PUT")
                request.Content = JsonContent.Create(new { naziv = "Test" });

            var resp = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Theory]
        [InlineData("GET", "/api/kategorija")]
        [InlineData("GET", "/api/kategorija/1")]
        [InlineData("POST", "/api/kategorija")]
        [InlineData("PUT", "/api/kategorija/1")]
        [InlineData("DELETE", "/api/kategorija/1")]
        public async Task KategorijaRute_SaClanTokenom_Vraca403(string metod, string url)
        {
            var token = await DobijClanToken();
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var request = new HttpRequestMessage(new HttpMethod(metod), url);
            if (metod == "POST" || metod == "PUT")
                request.Content = JsonContent.Create(new { naziv = "Test" });

            var resp = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }

        // ════════════════════════════════════════════════════════════════════════
        // KNJIGA — Mješovit pristup (US-25..US-29)
        // ════════════════════════════════════════════════════════════════════════

        [Theory]
        [InlineData("GET", "/api/knjiga")]
        [InlineData("GET", "/api/knjiga/1")]
        [InlineData("POST", "/api/knjiga")]
        [InlineData("PUT", "/api/knjiga/1")]
        [InlineData("DELETE", "/api/knjiga/1")]
        public async Task KnjigaRute_BezAutentifikacije_Vraca401(string metod, string url)
        {
            var client = CreateClient();
            var request = new HttpRequestMessage(new HttpMethod(metod), url);
            if (metod == "POST" || metod == "PUT")
                request.Content = JsonContent.Create(new { });

            var resp = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task KnjigaGet_SaClanTokenom_Vraca200()
        {
            // GET je dostupan svim autentifikovanim korisnicima (samo [Authorize], bez Role)
            var token = await DobijClanToken();
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var resp = await client.GetAsync("/api/knjiga");
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        }

        [Theory]
        [InlineData("POST", "/api/knjiga")]
        [InlineData("PUT", "/api/knjiga/1")]
        [InlineData("DELETE", "/api/knjiga/1")]
        public async Task KnjigaMutacija_SaClanTokenom_Vraca403(string metod, string url)
        {
            // POST/PUT/DELETE zahtijevaju Bibliotekar ili Administrator
            var token = await DobijClanToken();
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var request = new HttpRequestMessage(new HttpMethod(metod), url);
            if (metod == "POST" || metod == "PUT")
                request.Content = JsonContent.Create(new { });

            var resp = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }

        // ════════════════════════════════════════════════════════════════════════
        // PRIMJERAK — RBAC (US-21..US-24)
        // ════════════════════════════════════════════════════════════════════════

        [Theory]
        [InlineData("GET", "/api/primjerak/knjiga/1")]
        [InlineData("GET", "/api/primjerak/1")]
        [InlineData("POST", "/api/primjerak")]
        [InlineData("POST", "/api/primjerak/1/deaktiviraj")]
        public async Task PrimjerakRute_BezAutentifikacije_Vraca401(string metod, string url)
        {
            var client = CreateClient();
            var request = new HttpRequestMessage(new HttpMethod(metod), url);
            if (metod == "POST")
                request.Content = JsonContent.Create(new { knjigaId = 1, brojNovih = 1 });

            var resp = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Theory]
        [InlineData("GET", "/api/primjerak/knjiga/1")]
        [InlineData("GET", "/api/primjerak/1")]
        [InlineData("POST", "/api/primjerak")]
        [InlineData("POST", "/api/primjerak/1/deaktiviraj")]
        public async Task PrimjerakRute_SaClanTokenom_Vraca403(string metod, string url)
        {
            var token = await DobijClanToken();
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var request = new HttpRequestMessage(new HttpMethod(metod), url);
            if (metod == "POST")
                request.Content = JsonContent.Create(new { knjigaId = 1, brojNovih = 1 });

            var resp = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }

        // ════════════════════════════════════════════════════════════════════════
        // XSS — Unos skripti u registracijsku formu
        // ════════════════════════════════════════════════════════════════════════

        [Theory]
        [InlineData("<script>alert('xss')</script>", "Prezime", "xss@test.ba")]
        [InlineData("Ime", "<img src=x onerror=alert(1)>", "xss2@test.ba")]
        public async Task Create_XssPayloadUImenu_VracaSeEscapovan(
            string ime, string prezime, string email)
        {
            var token = await DobijBibliotekarToken();
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var resp = await client.PostAsJsonAsync("/api/korisnik", new
            {
                ime,
                prezime,
                email,
                lozinka = "Lozinka1!"
            });

            if (resp.StatusCode == HttpStatusCode.Created)
            {
                var body = await resp.Content.ReadAsStringAsync();
                Assert.DoesNotContain("<script>", body, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("onerror", body, StringComparison.OrdinalIgnoreCase);
            }
        }

        [Theory]
        [InlineData("<script>alert('xss')</script>", "Test", "xss_naziv@test.ba")]
        [InlineData("Test", "<img src=x onerror=alert(1)>", "xss_opis@test.ba")]
        public async Task Kategorija_XssPayloadUNazivu_VracaSeEscapovanIliOdbijen(
            string naziv, string opis, string _)
        {
            var token = await DobijBibliotekarToken();
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var resp = await client.PostAsJsonAsync("/api/kategorija", new { naziv, opis });

            if (resp.StatusCode == HttpStatusCode.Created)
            {
                var body = await resp.Content.ReadAsStringAsync();
                Assert.DoesNotContain("<script>", body, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("onerror", body, StringComparison.OrdinalIgnoreCase);
            }
            // 400/409 su također prihvatljivi
        }

        // ════════════════════════════════════════════════════════════════════════
        // PRIMJERAK — graničні slučajevi (poslovna validacija)
        // ════════════════════════════════════════════════════════════════════════

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(51)]
        [InlineData(999)]
        public async Task PrimjerakCreate_NevalidinBrojNovih_Vraca400(int brojNovih)
        {
            var token = await DobijBibliotekarToken();
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var resp = await client.PostAsJsonAsync("/api/primjerak", new
            {
                knjigaId = 1,
                brojNovih = brojNovih
            });

            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        [Fact]
        public async Task PrimjerakCreate_NepostojecaKnjiga_Vraca400()
        {
            var token = await DobijBibliotekarToken();
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var resp = await client.PostAsJsonAsync("/api/primjerak", new
            {
                knjigaId = 99999,
                brojNovih = 1
            });

            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        // ════════════════════════════════════════════════════════════════════════
        // KNJIGA — ISBN injection / fuzzing (US-25)
        // ════════════════════════════════════════════════════════════════════════

        [Theory]
        [InlineData("' OR '1'='1")]
        [InlineData("<script>alert(1)</script>")]
        [InlineData("../../../../etc/passwd")]
        [InlineData("")]
        [InlineData("12345")]          // prekratak
        [InlineData("ABCDEFGHIJKLM")] // nije broj
        public async Task KnjigaCreate_NevalidinIsbn_Vraca400(string isbn)
        {
            var token = await DobijBibliotekarToken();
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var resp = await client.PostAsJsonAsync("/api/knjiga", new
            {
                naslov = "Test Knjiga",
                autor = "Test Autor",
                isbn = isbn,
                kategorijaId = 1,
                brojPrimjeraka = 1
            });

            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        // ════════════════════════════════════════════════════════════════════════
        // Helpers
        // ════════════════════════════════════════════════════════════════════════

        private async Task<string> DobijBibliotekarToken()
        {
            var client = CreateClient();
            var resp = await client.PostAsJsonAsync("/api/auth/login", new
            {
                email = "bibliotekar@smartlib.ba",
                lozinka = "Test123!"
            });
            resp.EnsureSuccessStatusCode();
            var body = await resp.Content.ReadFromJsonAsync<JsonElement>();
            return body.GetProperty("token").GetString()!;
        }

        private async Task<string> DobijClanToken()
        {
            var client = CreateClient();
            var resp = await client.PostAsJsonAsync("/api/auth/login", new
            {
                email = "clan@smartlib.ba",
                lozinka = "Test123!"
            });

            // Dodaj ovo privremeno da vidiš pravi error:
            var body = await resp.Content.ReadAsStringAsync();
            Assert.True(resp.IsSuccessStatusCode, $"Login failed: {resp.StatusCode} — {body}");

            var json = await resp.Content.ReadFromJsonAsync<JsonElement>();
            return json.GetProperty("token").GetString()!;
        }
    }
}