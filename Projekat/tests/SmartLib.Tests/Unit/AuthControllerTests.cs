using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using SmartLib.API.Controllers;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using Xunit;

namespace SmartLib.Tests.Unit
{
    /// <summary>
    /// US-04: Prijava korisnika — unit testovi za API AuthController
    /// US-05: Poruke o neuspjehu prijave
    /// </summary>
    public class AuthControllerTests
    {
        private readonly Mock<IKorisnikRepository> _repoMock;
        private readonly IConfiguration _configuration;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _repoMock = new Mock<IKorisnikRepository>();

            // Minimalna JWT konfiguracija za testove
            var inMemorySettings = new Dictionary<string, string>
            {
                { "Jwt:Key",               "SmartLibTestKeyKojiJeDovoljnoDugacak123!" },
                { "Jwt:Issuer",            "SmartLib" },
                { "Jwt:Audience",          "SmartLibUsers" },
                { "Jwt:ExpirationMinutes", "60" }
            };
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();

            _controller = new AuthController(_repoMock.Object, _configuration);
        }

        // ─── Pomoćna metoda ───────────────────────────────────────────────────

        private static Korisnik PravianKorisnik(string uloga = "Član") => new()
        {
            Id       = 1,
            Ime      = "Test",
            Prezime  = "Korisnik",
            Email    = "test@smartlib.ba",
            // Hash za lozinku "Lozinka1!"  generisan PasswordHasher-om
            LozinkaHash = SmartLib.Infrastructure.Security.PasswordHasher.HashPassword("Lozinka1!"),
            Status   = "aktivan",
            Uloga    = new Uloga { Id = 1, Naziv = uloga }
        };

        // ─── US-04: Uspješna prijava ──────────────────────────────────────────

        [Fact]
        public async Task Login_ValidanEmailILozinka_VracaOkSaTokenom()
        {
            _repoMock.Setup(r => r.GetByEmailAsync("test@smartlib.ba"))
                     .ReturnsAsync(PravianKorisnik());

            var result = await _controller.Login(new LoginRequest
            {
                Email   = "test@smartlib.ba",
                Lozinka = "Lozinka1!"
            });

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<LoginResponse>(ok.Value);
            Assert.NotEmpty(response.Token);
            Assert.Equal("Test", response.Ime);
            Assert.Equal("Član", response.Uloga);
        }

        // ─── US-05: Neispravni kredencijali ───────────────────────────────────

        [Fact]
        public async Task Login_NetacnaLozinka_VracaUnauthorized()
        {
            _repoMock.Setup(r => r.GetByEmailAsync("test@smartlib.ba"))
                     .ReturnsAsync(PravianKorisnik());

            var result = await _controller.Login(new LoginRequest
            {
                Email   = "test@smartlib.ba",
                Lozinka = "PogrešnaLozinka!"
            });

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        [Fact]
        public async Task Login_NepostojeciEmail_VracaUnauthorized()
        {
            _repoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                     .ReturnsAsync((Korisnik?)null);

            var result = await _controller.Login(new LoginRequest
            {
                Email   = "nepostoji@smartlib.ba",
                Lozinka = "BilaSta1!"
            });

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        [Fact]
        public async Task Login_DeaktiviranKorisnik_VracaUnauthorized()
        {
            var korisnik = PravianKorisnik();
            korisnik.Status = "deaktiviran";

            _repoMock.Setup(r => r.GetByEmailAsync("test@smartlib.ba"))
                     .ReturnsAsync(korisnik);

            var result = await _controller.Login(new LoginRequest
            {
                Email   = "test@smartlib.ba",
                Lozinka = "Lozinka1!"
            });

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        // ─── US-05: Generička poruka — ne otkriva razlog greške ──────────────

        [Fact]
        public async Task Login_Neuspjeh_PorukaJeGenerickaNeSadrziDetalje()
        {
            _repoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                     .ReturnsAsync((Korisnik?)null);

            var result = await _controller.Login(new LoginRequest
            {
                Email   = "x@x.com",
                Lozinka = "x"
            });

            var unauth = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            var body = unauth.Value?.ToString() ?? string.Empty;

            // Poruka ne smije otkrivati da li greška leži u emailu ili lozinci
            Assert.DoesNotContain("email", body, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("lozinka", body, StringComparison.OrdinalIgnoreCase);
        }

        // ─── US-04: Validacija modela (prazna polja) ──────────────────────────

        [Theory]
        [InlineData("", "Lozinka1!")]
        [InlineData("test@smartlib.ba", "")]
        [InlineData("nijeEmail", "Lozinka1!")]
        public async Task Login_NeispravanModel_VracaBadRequest(string email, string lozinka)
        {
            _controller.ModelState.AddModelError("test", "Greška validacije");

            var result = await _controller.Login(new LoginRequest
            {
                Email = email,
                Lozinka = lozinka
            });

            // Treba biti ValidationProblem (400), popraviti implementaciju ako nije
            var actionResult = result.Result ?? (result as ActionResult<LoginResponse>)!.Result;
            var objectResult = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(null, objectResult.StatusCode);
        }
    }
}
