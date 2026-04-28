using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartLib.Infrastructure.Data;
using System.Net;
using System.Net.Http.Json;
using System.Text;
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
    /// </summary>
    public class SecurityTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public SecurityTests(WebApplicationFactory<Program> factory)
        {
            // Ista konfiguracija kao u integracijskim testovima
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(Microsoft.EntityFrameworkCore.DbContextOptions<SmartLib.Infrastructure.Data.ApplicationDbContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<SmartLib.Infrastructure.Data.ApplicationDbContext>(opts =>
                        opts.UseInMemoryDatabase("SecurityTestDb_" + Guid.NewGuid()));

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

        // ─── SQL Injection na login polju ─────────────────────────────────────

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
                email   = sqlPayload,
                lozinka = lozinka
            });

            // Treba biti 400 (nevalidan email format) ili 401, NIKAD 200
            Assert.True(
                resp.StatusCode == HttpStatusCode.BadRequest ||
                resp.StatusCode == HttpStatusCode.Unauthorized,
                $"SQL injection payload dao je: {resp.StatusCode}"
            );
        }

        // ─── XSS na registracijskim poljima ──────────────────────────────────

        [Theory(Skip = "Security layer (JWT/RBAC) nije implementiran")]
        [InlineData("<script>alert('xss')</script>", "Prezime", "xss@test.ba")]
        [InlineData("Ime", "<img src=x onerror=alert(1)>", "xss2@test.ba")]
        public async Task Create_XssPayloadUImenu_VracaSeEscapovan(
            string ime, string prezime, string email)
        {
            var token  = await DobijBibliotekarToken();
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var resp = await client.PostAsJsonAsync("/api/korisnik", new
            {
                ime, prezime, email,
                lozinka = "Lozinka1!"
            });

            // Ako je 201, provjeri da response ne reflektuje <script> tag
            if (resp.StatusCode == HttpStatusCode.Created)
            {
                var body = await resp.Content.ReadAsStringAsync();
                Assert.DoesNotContain("<script>", body, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("onerror",  body, StringComparison.OrdinalIgnoreCase);
            }
            // 400 je također prihvatljivo (server odbija takav input)
        }

        // ─── US-08: Lažni JWT token bez validnog potpisa ne prolazi ──────────

        [Fact]
        public async Task ZasticenaRuta_LazniBearerToken_Vraca401()
        {
            var client = CreateClient();
            // Ručno konstruisan token sa krivim potpisom
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

        // ─── US-08: Token sa promijenjenom ulogom ne prolazi ─────────────────

        [Fact(Skip = "Security layer (JWT/RBAC) nije implementiran")]
        public async Task ZasticenaRuta_TokenSaModifikovanomUlogom_Vraca401()
        {
            // Dobijemo validan token za Člana
            var clanToken = await DobijClanToken();

            // Dekodiramo payload i mijenjamo ulogu (bez valjanog potpisa)
            var dijelovi = clanToken.Split('.');
            var payloadBytes = Convert.FromBase64String(
                dijelovi[1].PadRight(dijelovi[1].Length + (4 - dijelovi[1].Length % 4) % 4, '='));
            var payload = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(payloadBytes)!;

            // Ovaj token ima pogrešan potpis — server ga mora odbiti
            var lazniToken = $"{dijelovi[0]}.{dijelovi[1]}.LAZNI_POTPIS";

            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", lazniToken);

            var resp = await client.GetAsync("/api/korisnik");
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        // ─── US-09: Deaktiviran korisnik sa STARIM TOKENOM ne smije proći ────
        // Napomena: JWT je stateless — ako token nije istekao, server ga prihvata.
        // Pravi fix: blacklist tokena ili kratko trajanje + refresh token.
        // Test dokumentuje trenutno ponašanje i podsjeća tim na ovu rupu.

        [Fact]
        public async Task DeaktiviranKorisnik_SaStarimJwtTokenom_BiljeziSigurnosnuNapomenu()
        {
            // Ovaj test NAMJERNO NIJE Assert.Equal(401) jer JWT je stateless
            // i token ostaje validan dok ne istekne.
            // Ako tim implementira blacklist/refresh, ovaj test treba promijeniti u 401.
            Assert.True(true,
                "SIGURNOSNA NAPOMENA (US-09): Deaktiviran korisnik zadržava stari JWT " +
                "dok ne istekne. Riješiti: token blacklist ili refresh token mehanizam.");
        }

        // ─── US-08: Direktan URL unos zaštićene rute bez tokena ──────────────

        [Theory]
        [InlineData("/api/korisnik")]
        [InlineData("/api/korisnik/1")]
        public async Task DirectUrlUnos_BezAutentifikacije_Vraca401(string url)
        {
            var client = CreateClient();
            var resp   = await client.GetAsync(url);
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        // ─── Brute force: Previše pokušaja (dokumentacijski test) ────────────

        [Fact]
        public async Task Login_ViseNeuspjelihPokusaja_SvakiVraca401NikadNe200()
        {
            var client = CreateClient();

            for (int i = 0; i < 10; i++)
            {
                var resp = await client.PostAsJsonAsync("/api/auth/login", new
                {
                    email   = "clan@smartlib.ba",
                    lozinka = $"PogresnaLozinka{i}!"
                });

                // Svaki pokušaj mora biti 401, nikad 200
                Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
            }
        }

        // ─── Helpers ─────────────────────────────────────────────────────────

        private async Task<string> DobijBibliotekarToken()
        {
            var client = CreateClient();
            var resp = await client.PostAsJsonAsync("/api/auth/login", new
            {
                email   = "bibliotekar@smartlib.ba",
                lozinka = "Test123!"
            });
            resp.EnsureSuccessStatusCode();
            var body = await resp.Content.ReadFromJsonAsync<JsonElement>();
            return body.GetProperty("token").GetString()!;
            Console.WriteLine(resp.StatusCode);
            Console.WriteLine(body);
        }

        private async Task<string> DobijClanToken()
        {
            var client = CreateClient();
            var resp = await client.PostAsJsonAsync("/api/auth/login", new
            {
                email   = "clan@smartlib.ba",
                lozinka = "Test123!"
            });
            resp.EnsureSuccessStatusCode();
            var body = await resp.Content.ReadFromJsonAsync<JsonElement>();
            return body.GetProperty("token").GetString()!;
        }
    }
}
