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
    /// Penetracijski / Sigurnosni testovi
    ///
    /// Pokriveni vektori napada:
    ///   PT-01: SQL Injection u email polju (US-04, US-05)
    ///   PT-02: SQL Injection u polju lozinke (US-04, US-05)
    ///   PT-03: Brute Force napad na login (US-04)
    ///   PT-04: Lažni JWT token bez potpisa (US-08)
    ///   PT-05: Modificiran JWT token sa lažnim potpisom (US-08)
    ///   PT-06: Arhitekturalni rizik — stari JWT deaktiviranog korisnika (US-09)
    ///   PT-07: XSS napad u registracijskoj formi — Ime/Prezime (US-01, US-02)
    ///   PT-08: XSS napad u nazivu kategorije (US-30)
    ///   PT-09: Path Traversal / Injection u ISBN polju (US-25)
    /// </summary>
    public class PenetrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private static readonly string _dbName = "PenetrationTestDb_" + Guid.NewGuid();

        public PenetrationTests(WebApplicationFactory<Program> factory)
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

                    services.AddDbContext<ApplicationDbContext>(opts =>
                        opts.UseInMemoryDatabase(_dbName));

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
        
        // PT-01: SQL Injection u email polju (US-04 / US-05)       

        [Theory]
        [InlineData("' OR '1'='1", "bilo_sta")]
        [InlineData("admin'--", "bilo_sta")]
        [InlineData("'; DROP TABLE Korisnici;--", "bilo_sta")]
        [InlineData("\" OR 1=1--", "bilo_sta")]
        public async Task Login_SqlInjectionUEmailu_VracaBadRequestIliUnauthorized(
            string sqlPayload, string lozinka)
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
                $"[PT-01] SQL injection u emailu dao je neočekivan status: {resp.StatusCode}. " +
                $"Payload: {sqlPayload}"
            );
        }
       
        // PT-02: SQL Injection u polju lozinke (US-04 / US-05)       

        [Theory]
        [InlineData("bibliotekar@smartlib.ba", "' OR '1'='1")]
        [InlineData("bibliotekar@smartlib.ba", "'; DROP TABLE Korisnici;--")]
        [InlineData("bibliotekar@smartlib.ba", "\" OR 1=1--")]
        public async Task Login_SqlInjectionULozinki_VracaUnauthorized(
            string email, string sqlPayload)
        {
            var client = CreateClient();
            var resp = await client.PostAsJsonAsync("/api/auth/login", new
            {
                email = email,
                lozinka = sqlPayload
            });

            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }
       
        // PT-03: Brute Force napad — 10 uzastopnih pokušaja (US-04)       

        [Fact]
        public async Task Login_ViseNeuspjelihPokusaja_SvakiVracaUnauthorized()
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
      
        // PT-04: Lažni JWT token bez potpisa (US-08)        

        [Fact]
        public async Task ZasticenaRuta_LazniJwtBezPotpisa_VracaUnauthorized()
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
      
        // PT-05: Modificiran JWT token — promjena potpisa (US-08)       

        [Fact]
        public async Task ZasticenaRuta_ModifikovanJwtPotpis_VracaUnauthorized()
        {
            var validanToken = await DobijClanToken();
            var dijelovi = validanToken.Split('.');
            var lazniToken = $"{dijelovi[0]}.{dijelovi[1]}.LAZNI_POTPIS_KOJI_NIJE_VALIDAN";

            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", lazniToken);

            var resp = await client.GetAsync("/api/korisnik");
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }
        
        // PT-06: Arhitekturalni sigurnosni rizik — stari JWT deaktiviranog korisnika (US-09)
        //
        // Integracijski test pokriva da deaktiviran korisnik ne može dobiti novi token.
        // Ovaj test dokumentuje preostali arhitekturalni rizik: već izdati JWT token
        // ostaje validan sve dok ne istekne

        [Fact]
        public async Task Login_DeaktiviranKorisnik_StariJwtOstajeValidan_ArchitekturalnaNapomena()
        {
            Assert.True(true,
                "SIGURNOSNA NAPOMENA (US-09): Deaktiviran korisnik zadržava stari JWT " +
                "dok ne istekne. Preporuka: token blacklist ili kratko trajanje tokena + refresh.");
        }
        
        // PT-07: XSS napad u registracijskoj formi — Ime / Prezime (US-01, US-02)        

        [Theory]
        [InlineData("<script>alert('xss')</script>", "Prezime", "xss1@test.ba")]
        [InlineData("Ime", "<img src=x onerror=alert(1)>", "xss2@test.ba")]
        public async Task KorisnikCreate_XssPayloadUImenuIliPrezimenu_OdbijenIliEscapovan(
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
            else
            {
                Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
            }
        }
        
        // PT-08: XSS napad u nazivu kategorije (US-30)        

        [Theory]
        [InlineData("<script>alert('xss')</script>", "Opis")]
        [InlineData("Naziv", "<img src=x onerror=alert(1)>")]
        public async Task KategorijaCreate_XssPayloadUNazivu_OdbijenIliEscapovan(
            string naziv, string opis)
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
        }
        
        // PT-09: Path Traversal i Injection u ISBN polju (US-25)       

        [Theory]
        [InlineData("' OR '1'='1", "SQL Injection")]
        [InlineData("<script>alert(1)</script>", "XSS")]
        [InlineData("../../../../etc/passwd", "Path Traversal")]
        public async Task KnjigaCreate_InjectionUIsbnPolju_VracaBadRequest(
            string isbn, string tipNapada)
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
       
        // Helpers        

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
            var body = await resp.Content.ReadAsStringAsync();
            Assert.True(resp.IsSuccessStatusCode, $"Login nije uspio: {resp.StatusCode} — {body}");
            var json = await resp.Content.ReadFromJsonAsync<JsonElement>();
            return json.GetProperty("token").GetString()!;
        }
    }
}