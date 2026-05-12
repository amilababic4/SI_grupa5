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
using SmartLib.Core.DTOs;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;
using SmartLib.Infrastructure.Security;
using Xunit;

namespace SmartLib.Tests.Integration
{
    public sealed class ZaduzenjeTestFixture : WebApplicationFactory<Program>
    {
        private readonly string _dbName = "ZaduzenjeTestDb_" + Guid.NewGuid();

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

                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase(_dbName));
            });
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = base.CreateHost(builder);

            using var scope = host.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();
            ZaduzenjeDbSeeder.Seed(db);

            return host;
        }
    }

    public class ZaduzenjeIntegrationTests
    {
        private static async Task<string> GetTokenAsync(ZaduzenjeTestFixture factory, string email, string lozinka)
        {
            var client = factory.CreateClient();
            var response = await client.PostAsJsonAsync("/api/auth/login", new { email, lozinka });

            response.EnsureSuccessStatusCode();

            using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return document.RootElement.GetProperty("token").GetString()!;
        }

        private static async Task<HttpClient> CreateAuthedClientAsync(
            ZaduzenjeTestFixture factory,
            string email,
            string lozinka)
        {
            var token = await GetTokenAsync(factory, email, lozinka);
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
        {
            var payload = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(payload);
        }

        [Fact]
        public async Task GetActive_BezAuth_Vraca401()
        {
            using var factory = new ZaduzenjeTestFixture();

            var response = await factory.CreateClient().GetAsync("/api/zaduzenje");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetActive_SaBibliotekarToken_VracaOkSaListom()
        {
            using var factory = new ZaduzenjeTestFixture();
            var client = await CreateAuthedClientAsync(factory, "bibliotekar@smartlib.ba", "Test123!");

            var response = await client.GetAsync("/api/zaduzenje");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            using var document = await ReadJsonAsync(response);

            Assert.Equal(2, document.RootElement.GetArrayLength());
        }

        [Fact]
        public async Task GetActive_SaFilteromPoImenu_VracaJedanRezultat()
        {
            using var factory = new ZaduzenjeTestFixture();
            var client = await CreateAuthedClientAsync(factory, "bibliotekar@smartlib.ba", "Test123!");

            var response = await client.GetAsync("/api/zaduzenje?clan=bibliotekar");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            using var document = await ReadJsonAsync(response);

            Assert.Equal(1, document.RootElement.GetArrayLength());
            Assert.Equal(
                "bibliotekar@smartlib.ba",
                document.RootElement[0].GetProperty("korisnikEmail").GetString());
        }

        [Fact]
        public async Task GetMine_BezAuth_Vraca401()
        {
            using var factory = new ZaduzenjeTestFixture();

            var response = await factory.CreateClient().GetAsync("/api/zaduzenje/moja");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetMine_SaClanToken_VracaSamoVlastitaZaduzenja()
        {
            using var factory = new ZaduzenjeTestFixture();
            var client = await CreateAuthedClientAsync(factory, "clan@smartlib.ba", "Test123!");

            var response = await client.GetAsync("/api/zaduzenje/moja");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            using var document = await ReadJsonAsync(response);

            Assert.Single(document.RootElement.EnumerateArray());
            Assert.Equal(
                "clan@smartlib.ba",
                document.RootElement[0].GetProperty("korisnikEmail").GetString());
        }

        [Fact]
        public async Task GetById_PostojeciId_Vraca200IJson()
        {
            using var factory = new ZaduzenjeTestFixture();
            var client = await CreateAuthedClientAsync(factory, "bibliotekar@smartlib.ba", "Test123!");

            var response = await client.GetAsync("/api/zaduzenje/1");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            using var document = await ReadJsonAsync(response);

            Assert.Equal(1, document.RootElement.GetProperty("id").GetInt32());
            Assert.Equal("aktivno", document.RootElement.GetProperty("status").GetString());
            Assert.Equal(
                "Test Clan",
                document.RootElement.GetProperty("korisnikIme").GetString());
        }

        [Fact]
        public async Task GetById_NepostojeciId_Vraca404()
        {
            using var factory = new ZaduzenjeTestFixture();
            var client = await CreateAuthedClientAsync(factory, "bibliotekar@smartlib.ba", "Test123!");

            var response = await client.GetAsync("/api/zaduzenje/99999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetHistory_PostojeciKorisnik_Vraca200()
        {
            using var factory = new ZaduzenjeTestFixture();
            var client = await CreateAuthedClientAsync(factory, "bibliotekar@smartlib.ba", "Test123!");

            var response = await client.GetAsync("/api/zaduzenje/historija/3");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            using var document = await ReadJsonAsync(response);

            Assert.NotEmpty(document.RootElement.EnumerateArray());
            Assert.All(
                document.RootElement.EnumerateArray(),
                element => Assert.Equal("deaktiviran@smartlib.ba", element.GetProperty("korisnikEmail").GetString()));
        }

        [Fact]
        public async Task GetHistory_NepostojeciKorisnik_Vraca404()
        {
            using var factory = new ZaduzenjeTestFixture();
            var client = await CreateAuthedClientAsync(factory, "bibliotekar@smartlib.ba", "Test123!");

            var response = await client.GetAsync("/api/zaduzenje/historija/99999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Zaduzi_BezAuth_Vraca401()
        {
            using var factory = new ZaduzenjeTestFixture();

            var response = await factory.CreateClient().PostAsJsonAsync(
                "/api/zaduzenje/zaduzi",
                new ZaduzenjeCreateDto
                {
                    KorisnikId = 3,
                    KnjigaId = 2,
                    PrimjerakId = 3
                });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Zaduzi_KaoClan_Vraca403()
        {
            using var factory = new ZaduzenjeTestFixture();
            var client = await CreateAuthedClientAsync(factory, "clan@smartlib.ba", "Test123!");

            var response = await client.PostAsJsonAsync(
                "/api/zaduzenje/zaduzi",
                new ZaduzenjeCreateDto
                {
                    KorisnikId = 3,
                    KnjigaId = 2,
                    PrimjerakId = 3
                });

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task Zaduzi_ValidanPayload_Vraca201IPraviZaduzenje()
        {
            using var factory = new ZaduzenjeTestFixture();
            var client = await CreateAuthedClientAsync(factory, "bibliotekar@smartlib.ba", "Test123!");

            var response = await client.PostAsJsonAsync(
                "/api/zaduzenje/zaduzi",
                new ZaduzenjeCreateDto
                {
                    KorisnikId = 3,
                    KnjigaId = 2,
                    PrimjerakId = 3,
                    DatumPovratka = DateTime.UtcNow.AddDays(21)
                });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            using var document = await ReadJsonAsync(response);

            Assert.Equal("aktivno", document.RootElement.GetProperty("status").GetString());
            Assert.True(document.RootElement.GetProperty("id").GetInt32() > 0);

            using var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var primjerak = db.Primjerci.Single(p => p.Id == 3);
            var zaduzenje = db.Zaduzenja.Single(z => z.KorisnikId == 3 && z.PrimjerakId == 3 && z.Status == "aktivno");

            Assert.Equal("zadužen", primjerak.Status);
            Assert.NotEqual(default, zaduzenje.DatumZaduzivanja);
        }

        [Fact]
        public async Task Zaduzi_PrimjerakNedostupan_Vraca400()
        {
            using var factory = new ZaduzenjeTestFixture();
            var client = await CreateAuthedClientAsync(factory, "bibliotekar@smartlib.ba", "Test123!");

            var response = await client.PostAsJsonAsync(
                "/api/zaduzenje/zaduzi",
                new ZaduzenjeCreateDto
                {
                    KorisnikId = 3,
                    KnjigaId = 1,
                    PrimjerakId = 1
                });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Zaduzi_DatumUProslosti_Vraca400()
        {
            using var factory = new ZaduzenjeTestFixture();
            var client = await CreateAuthedClientAsync(factory, "bibliotekar@smartlib.ba", "Test123!");

            var response = await client.PostAsJsonAsync(
                "/api/zaduzenje/zaduzi",
                new ZaduzenjeCreateDto
                {
                    KorisnikId = 3,
                    KnjigaId = 2,
                    PrimjerakId = 3,
                    DatumPovratka = DateTime.UtcNow.AddDays(-1)
                });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Vrati_AktivnoZaduzenje_Vraca200IAzuriraStatus()
        {
            using var factory = new ZaduzenjeTestFixture();
            var client = await CreateAuthedClientAsync(factory, "bibliotekar@smartlib.ba", "Test123!");

            var response = await client.PostAsync("/api/zaduzenje/vrati/1", null);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            using var document = await ReadJsonAsync(response);

            Assert.Equal("zatvoreno", document.RootElement.GetProperty("status").GetString());
            Assert.NotNull(document.RootElement.GetProperty("datumStvarnogVracanja").GetString());

            using var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var zaduzenje = db.Zaduzenja.Single(z => z.Id == 1);
            var primjerak = db.Primjerci.Single(p => p.Id == 1);

            Assert.Equal("zatvoreno", zaduzenje.Status);
            Assert.Equal("dostupan", primjerak.Status);
            Assert.NotNull(zaduzenje.DatumStvarnogVracanja);
        }

        [Fact]
        public async Task Vrati_NeaktivnoZaduzenje_Vraca400()
        {
            using var factory = new ZaduzenjeTestFixture();
            var client = await CreateAuthedClientAsync(factory, "bibliotekar@smartlib.ba", "Test123!");

            var response = await client.PostAsync("/api/zaduzenje/vrati/3", null);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Vrati_NepostojeciId_Vraca404()
        {
            using var factory = new ZaduzenjeTestFixture();
            var client = await CreateAuthedClientAsync(factory, "bibliotekar@smartlib.ba", "Test123!");

            var response = await client.PostAsync("/api/zaduzenje/vrati/99999", null);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    internal static class ZaduzenjeDbSeeder
    {
        public static void Seed(ApplicationDbContext db)
        {
            db.Zaduzenja.RemoveRange(db.Zaduzenja);
            db.Primjerci.RemoveRange(db.Primjerci);
            db.Knjige.RemoveRange(db.Knjige);
            db.Kategorije.RemoveRange(db.Kategorije);
            db.Korisnici.RemoveRange(db.Korisnici);
            db.Uloge.RemoveRange(db.Uloge);
            db.SaveChanges();

            var ulogaClan = new Uloga
            {
                Id = 1,
                Naziv = "Član",
                Opis = "Član biblioteke"
            };

            var ulogaBibliotekar = new Uloga
            {
                Id = 2,
                Naziv = "Bibliotekar",
                Opis = "Bibliotečko osoblje"
            };

            db.Uloge.AddRange(ulogaClan, ulogaBibliotekar);

            var hash = PasswordHasher.HashPassword("Test123!");

            db.Korisnici.AddRange(
                new Korisnik
                {
                    Id = 1,
                    Ime = "Test",
                    Prezime = "Bibliotekar",
                    Email = "bibliotekar@smartlib.ba",
                    LozinkaHash = hash,
                    UlogaId = 2,
                    Status = "aktivan",
                    DatumKreiranja = DateTime.UtcNow
                },
                new Korisnik
                {
                    Id = 2,
                    Ime = "Test",
                    Prezime = "Clan",
                    Email = "clan@smartlib.ba",
                    LozinkaHash = hash,
                    UlogaId = 1,
                    Status = "aktivan",
                    DatumKreiranja = DateTime.UtcNow
                },
                new Korisnik
                {
                    Id = 3,
                    Ime = "Test",
                    Prezime = "Deaktiviran",
                    Email = "deaktiviran@smartlib.ba",
                    LozinkaHash = hash,
                    UlogaId = 1,
                    Status = "deaktiviran",
                    DatumKreiranja = DateTime.UtcNow
                });

            db.SaveChanges();

            db.Kategorije.AddRange(
                new Kategorija
                {
                    Id = 1,
                    Naziv = "Roman",
                    Opis = "Test kategorija za integracijske testove"
                },
                new Kategorija
                {
                    Id = 2,
                    Naziv = "Strucna literatura",
                    Opis = "Druga test kategorija"
                });

            db.SaveChanges();

            db.Knjige.AddRange(
                new Knjiga
                {
                    Id = 1,
                    Naslov = "Test Knjiga 1",
                    Autor = "Autor 1",
                    Isbn = "9780000000001",
                    KategorijaId = 1,
                    GodinaIzdanja = 2024
                },
                new Knjiga
                {
                    Id = 2,
                    Naslov = "Test Knjiga 2",
                    Autor = "Autor 2",
                    Isbn = "9780000000002",
                    KategorijaId = 1,
                    GodinaIzdanja = 2023
                });

            db.SaveChanges();

            db.Primjerci.AddRange(
                new Primjerak
                {
                    Id = 1,
                    KnjigaId = 1,
                    InventarniBroj = "INV-1-001",
                    Status = "zadužen"
                },
                new Primjerak
                {
                    Id = 2,
                    KnjigaId = 1,
                    InventarniBroj = "INV-1-002",
                    Status = "zadužen"
                },
                new Primjerak
                {
                    Id = 3,
                    KnjigaId = 2,
                    InventarniBroj = "INV-2-001",
                    Status = "dostupan"
                });

            db.Zaduzenja.AddRange(
                new Zaduzenje
                {
                    Id = 1,
                    KorisnikId = 2,
                    PrimjerakId = 1,
                    DatumZaduzivanja = DateTime.UtcNow.AddDays(-5),
                    DatumPlaniranogVracanja = DateTime.UtcNow.AddDays(25),
                    Status = "aktivno"
                },
                new Zaduzenje
                {
                    Id = 2,
                    KorisnikId = 1,
                    PrimjerakId = 2,
                    DatumZaduzivanja = DateTime.UtcNow.AddDays(-3),
                    DatumPlaniranogVracanja = DateTime.UtcNow.AddDays(10),
                    Status = "aktivno"
                },
                new Zaduzenje
                {
                    Id = 3,
                    KorisnikId = 3,
                    PrimjerakId = 3,
                    DatumZaduzivanja = DateTime.UtcNow.AddDays(-30),
                    DatumPlaniranogVracanja = DateTime.UtcNow.AddDays(-1),
                    DatumStvarnogVracanja = DateTime.UtcNow.AddDays(-1),
                    Status = "zatvoreno"
                });

            db.SaveChanges();
        }
    }
}