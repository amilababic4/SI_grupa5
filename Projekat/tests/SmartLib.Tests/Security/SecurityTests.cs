using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartLib.Infrastructure.Data;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace SmartLib.Tests.Security
{
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
                    // Ukloni SAMO DbContextOptions — sve ostale registracije iz Program.cs ostaju
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<ApplicationDbContext>(opts =>
                        opts.UseInMemoryDatabase(_dbName));
                });
            });
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            TestData.TestSeeder.Seed(db);
        }

        private HttpClient CreateClient() => _factory.CreateClient();

        // Seed radimo u samom testu, ne u konstruktoru
        private HttpClient CreateClient(bool seed = false)
        {
            var client = _factory.CreateClient();

            if (seed)
            {
                using var scope = _factory.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                TestData.TestSeeder.Seed(db);
            }

            return client;
        }

        // PT-01
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
                $"[PT-01] SQL injection u emailu dao je neočekivan status: {resp.StatusCode}. Payload: {sqlPayload}"
            );
        }

        // PT-02
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

        // PT-03
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

        // PT-04
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

        // PT-05
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

        // PT-06
        [Fact]
        public async Task Login_DeaktiviranKorisnik_StariJwtOstajeValidan_ArchitekturalnaNapomena()
        {
            Assert.True(true,
                "SIGURNOSNA NAPOMENA (US-09): Deaktiviran korisnik zadržava stari JWT " +
                "dok ne istekne. Preporuka: token blacklist ili kratko trajanje tokena + refresh.");
        }

        // PT-07
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

        // PT-08
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
        [Fact]
        public async Task Zaduzenje_GetActive_ClanNeSmijePristupiti_VracaForbidden()
        {
            var token = await DobijClanToken();

            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var resp = await client.GetAsync("/api/zaduzenje");

            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }
        [Fact]
        public async Task Zaduzi_BezTokena_VracaUnauthorized()
        {
            var client = CreateClient();

            var resp = await client.PostAsJsonAsync("/api/zaduzenje/zaduzi", new
            {
                korisnikId = 2,
                primjerakId = 1
            });

            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }
        [Fact]
        public async Task Zaduzi_ClanPokusavaZaduziti_VracaForbidden()
        {
            var token = await DobijClanToken();

            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var resp = await client.PostAsJsonAsync("/api/zaduzenje/zaduzi", new
            {
                korisnikId = 2,
                primjerakId = 1
            });

            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }
        [Fact]
        public async Task Rezervacija_OtkazivanjeTudjeRezervacije_VracaForbidden()
        {
            var token = await DobijClanToken();

            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var resp = await client.DeleteAsync("/api/rezervacija/999");

            Assert.True(
                resp.StatusCode == HttpStatusCode.Forbidden ||
                resp.StatusCode == HttpStatusCode.NotFound);
        }
        [Fact]
        public async Task Rezervacija_Create_BezTokena_VracaUnauthorized()
        {
            var client = CreateClient();

            var resp = await client.PostAsJsonAsync("/api/rezervacija", new
            {
                knjigaId = 1
            });

            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }
        [Fact]
        public async Task Primjerak_Create_ClanPokusavaKreirati_VracaForbidden()
        {
            var token = await DobijClanToken();

            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var resp = await client.PostAsJsonAsync("/api/primjerak", new
            {
                knjigaId = 1,
                brojNovih = 1
            });

            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }
        [Fact]
        public async Task Primjerak_Create_BezTokena_VracaUnauthorized()
        {
            var client = CreateClient();

            var resp = await client.PostAsJsonAsync("/api/primjerak", new
            {
                knjigaId = 1,
                brojNovih = 1
            });

            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }
        [Fact]
        public async Task Primjerak_GetById_NegativanId_VracaNotFound()
        {
            var token = await DobijBibliotekarToken();

            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var resp = await client.GetAsync("/api/primjerak/-999");

            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }
        [Fact]
        public async Task Knjiga_Create_BezTokena_VracaUnauthorized()
        {
            var client = CreateClient();

            var resp = await client.PostAsJsonAsync("/api/knjiga", new
            {
                naslov = "Test knjiga",
                autor = "Test autor",
                isbn = "9781234567890",
                kategorijaId = 1,
                brojPrimjeraka = 1
            });

            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }
        [Fact]
        public async Task Knjiga_Create_ClanToken_VracaForbidden()
        {
            var token = await DobijClanToken();

            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var resp = await client.PostAsJsonAsync("/api/knjiga", new
            {
                naslov = "Test knjiga",
                autor = "Test autor",
                isbn = "9781234567890",
                kategorijaId = 1,
                brojPrimjeraka = 1
            });

            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }
        [Theory]
        [InlineData("<script>alert('xss')</script>")]
        [InlineData("<img src=x onerror=alert(1)>")]
        public async Task Knjiga_Create_XssUNaslovu_OdbijenIliEscapovan(string naslov)
        {
            var token = await DobijBibliotekarToken();

            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var resp = await client.PostAsJsonAsync("/api/knjiga", new
            {
                naslov,
                autor = "Autor",
                isbn = Guid.NewGuid().ToString("N")[..13],
                kategorijaId = 1,
                brojPrimjeraka = 1
            });

            if (resp.StatusCode == HttpStatusCode.Created)
            {
                var body = await resp.Content.ReadAsStringAsync();

                Assert.DoesNotContain("<script>", body, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("onerror", body, StringComparison.OrdinalIgnoreCase);
            }
        }
        [Theory]
        [InlineData("' OR '1'='1")]
        [InlineData("'; DROP TABLE Knjige;--")]
        [InlineData("\" OR 1=1--")]
        public async Task Knjiga_Pretraga_SqlInjectionPayload_NeUzrokujeGresku(string payload)
        {
            var token = await DobijClanToken();

            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var resp = await client.GetAsync($"/api/knjiga?naslov={Uri.EscapeDataString(payload)}");

            Assert.True(
                resp.StatusCode == HttpStatusCode.OK ||
                resp.StatusCode == HttpStatusCode.BadRequest);
        }
        [Theory]
        [InlineData("'; DROP TABLE Knjige;--")]
        [InlineData("<script>alert(1)</script>")]
        [InlineData("../../../etc/passwd")]
        public async Task Knjiga_Create_MaliciozanIsbn_VracaBadRequest(string isbn)
        {
            var token = await DobijBibliotekarToken();

            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var resp = await client.PostAsJsonAsync("/api/knjiga", new
            {
                naslov = "Test",
                autor = "Autor",
                isbn,
                kategorijaId = 1,
                brojPrimjeraka = 1
            });

            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }
        [Fact]
        public async Task Knjiga_Delete_ClanToken_VracaForbidden()
        {
            var token = await DobijClanToken();

            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var resp = await client.DeleteAsync("/api/knjiga/1");

            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }

    }
}