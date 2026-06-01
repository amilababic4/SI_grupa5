using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Services;
using SmartLib.Web.Controllers;
using Xunit;

namespace SmartLib.Tests.Unit.WebTests
{
    public class RezervacijaWebControllerTests
    {
        private readonly Mock<IRezervacijaRepository> _rezervacijaRepoMock;
        private readonly Mock<IKnjigaRepository> _knjigaRepoMock;
        private readonly Mock<IZaduzenjeRepository> _zaduzenjeRepoMock;
        private readonly Mock<IKorisnikRepository> _korisnikRepoMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<ILogger<BibliotekariNotifikacijaService>> _loggerMock;
        private readonly BibliotekariNotifikacijaService _notifikacijaService;
        private readonly RezervacijaController _controller;

        public RezervacijaWebControllerTests()
        {
            _rezervacijaRepoMock = new Mock<IRezervacijaRepository>();
            _knjigaRepoMock = new Mock<IKnjigaRepository>();
            _zaduzenjeRepoMock = new Mock<IZaduzenjeRepository>();
            _korisnikRepoMock = new Mock<IKorisnikRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _loggerMock = new Mock<ILogger<BibliotekariNotifikacijaService>>();

            // Stvarna instanca servisa sa mock dependencijama umjesto Mock<BibliotekariNotifikacijaService>
            _notifikacijaService = new BibliotekariNotifikacijaService(
                _korisnikRepoMock.Object,
                _emailServiceMock.Object,
                _loggerMock.Object);

            // Prazna lista korisnika � nema primaoca, email se ne �alje
            _korisnikRepoMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Korisnik>());

            _controller = new RezervacijaController(
                _rezervacijaRepoMock.Object,
                _knjigaRepoMock.Object,
                _zaduzenjeRepoMock.Object,
                _notifikacijaService);

            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            _controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        }

        private void SetAuthenticatedUser(int userId = 1)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Name, "Test Korisnik")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);
        }

        [Fact]
        public async Task Index_VracaView()
        {
            _rezervacijaRepoMock.Setup(r => r.GetActiveAsync())
                .ReturnsAsync(new List<Rezervacija>());

            var result = await _controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Moje_VracaView()
        {
            SetAuthenticatedUser(1);
            _rezervacijaRepoMock.Setup(r => r.GetByKorisnikAsync(1))
                .ReturnsAsync(new List<Rezervacija>());

            var result = await _controller.Moje();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_RedirektujeNaDetaljeKnjige()
        {
            SetAuthenticatedUser(1);

            var knjiga = new Knjiga
            {
                Id = 5,
                Naslov = "Test Knjiga",
                Primjerci = new List<Primjerak>
                {
                    new Primjerak { Status = "zaduzen" }
                }
            };

            _knjigaRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(knjiga);
            _rezervacijaRepoMock.Setup(r => r.HasActiveAsync(1, 5)).ReturnsAsync(false);
            _zaduzenjeRepoMock.Setup(r => r.ImaKasnelaZaduzenjaAsync(1)).ReturnsAsync(false);
            _rezervacijaRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Rezervacija?)null);

            var result = await _controller.Create(5);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirect.ActionName);
            Assert.Equal("Knjiga", redirect.ControllerName);
            Assert.Equal(5, redirect.RouteValues!["id"]);
        }

        [Fact]
        public async Task Otkazi_RedirektujeNaMoje()
        {
            SetAuthenticatedUser(1);

            var rezervacija = new Rezervacija
            {
                Id = 10,
                KorisnikId = 1,
                Status = "aktivna"
            };

            _rezervacijaRepoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(rezervacija);

            var result = await _controller.Otkazi(10);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Moje", redirect.ActionName);
        }

        [Fact]
        public async Task Create_KnjigaNijePronadjena_VracaErrorIRedirect()
        {
            SetAuthenticatedUser(1);
            _knjigaRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Knjiga?)null);

            var result = await _controller.Create(99);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Knjiga", redirect.ControllerName);
            Assert.Equal("Knjiga nije pronađena.", _controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task Create_ImaDostupnihPrimjeraka_BlokiraPravljenje()
        {
            SetAuthenticatedUser(1);
            var knjiga = new Knjiga
            {
                Id = 5,
                Primjerci = new List<Primjerak> { new() { Status = "dostupan" } }
            };
            _knjigaRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(knjiga);

            var result = await _controller.Create(5);

            Assert.Equal("Rezervacija nije moguća — knjiga ima dostupnih primjeraka.",
                _controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task Create_DuplikatRezervacije_VracaError()
        {
            SetAuthenticatedUser(1);
            var knjiga = new Knjiga
            {
                Id = 5,
                Primjerci = new List<Primjerak> { new() { Status = "zaduzen" } }
            };
            _knjigaRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(knjiga);
            _rezervacijaRepoMock.Setup(r => r.HasActiveAsync(1, 5)).ReturnsAsync(true);

            var result = await _controller.Create(5);

            Assert.Equal("Već imate aktivnu rezervaciju za ovu knjigu.",
                _controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task Create_ImaKasnelaZaduzenja_VracaError()
        {
            SetAuthenticatedUser(1);
            var knjiga = new Knjiga
            {
                Id = 5,
                Primjerci = new List<Primjerak> { new() { Status = "zaduzen" } }
            };
            _knjigaRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(knjiga);
            _rezervacijaRepoMock.Setup(r => r.HasActiveAsync(1, 5)).ReturnsAsync(false);
            _zaduzenjeRepoMock.Setup(r => r.ImaKasnelaZaduzenjaAsync(1)).ReturnsAsync(true);

            var result = await _controller.Create(5);

            Assert.Contains("zakasnjelih zaduženja",
                _controller.TempData["ErrorMessage"]?.ToString());
        }

        [Fact]
        public async Task Otkazi_RezervacijaNijePronadjena_VracaError()
        {
            SetAuthenticatedUser(1);
            _rezervacijaRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Rezervacija?)null);

            var result = await _controller.Otkazi(99);

            Assert.Equal("Rezervacija nije pronađena.", _controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task Otkazi_DrugiBibliotekarmPokusava_VracaError()
        {
            SetAuthenticatedUser(2);
            var rezervacija = new Rezervacija { Id = 10, KorisnikId = 1, Status = "aktivna" };
            _rezervacijaRepoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(rezervacija);

            var result = await _controller.Otkazi(10);

            Assert.Equal("Nemate pravo otkazati ovu rezervaciju.", _controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task Otkazi_RezervacijaNijeAktivna_VracaError()
        {
            SetAuthenticatedUser(1);
            var rezervacija = new Rezervacija { Id = 10, KorisnikId = 1, Status = "otkazana" };
            _rezervacijaRepoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(rezervacija);

            var result = await _controller.Otkazi(10);

            Assert.Equal("Rezervacija nije aktivna.", _controller.TempData["ErrorMessage"]);
        }
        [Fact]
        public async Task Create_UspjesnaRezervacija_SaljeEmailBibliotekaru()
        {
            // Arrange
            SetAuthenticatedUser(1);

            var knjiga = new Knjiga
            {
                Id = 5,
                Naslov = "Test Knjiga",
                Primjerci = new List<Primjerak> { new() { Status = "zaduzen" } }
            };

            var sacuvanaRezervacija = new Rezervacija
            {
                Id = 1,
                KorisnikId = 1,
                KnjigaId = 5,
                Status = "aktivna",
                Knjiga = knjiga,
                Korisnik = new Korisnik { Id = 1, Ime = "Test", Prezime = "Korisnik", Email = "test@test.com" }
            };

            _knjigaRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(knjiga);
            _rezervacijaRepoMock.Setup(r => r.HasActiveAsync(1, 5)).ReturnsAsync(false);
            _zaduzenjeRepoMock.Setup(r => r.ImaKasnelaZaduzenjaAsync(1)).ReturnsAsync(false);
            _rezervacijaRepoMock.Setup(r => r.CreateAsync(It.IsAny<Rezervacija>())).ReturnsAsync((Rezervacija r) => r);
            _rezervacijaRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(sacuvanaRezervacija);

            // Bibliotekar koji �e primiti email
            _korisnikRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Korisnik>
                {
        new() { Id = 99, Ime = "Bibliotekar", Prezime = "Test", Email = "bib@test.com", UlogaId = 2, Uloga = new Uloga { Id = 2, Naziv = RoleNames.Bibliotekar }, Status = "aktivan" }
                });

            // Act
            var result = await _controller.Create(5);

            // Assert � email servis je pozvan bar jednom
            _emailServiceMock.Verify(
                e => e.SendEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task Create_EmailGreskaNeBlokiraRezervaciju()
        {
            // Arrange
            SetAuthenticatedUser(1);

            var knjiga = new Knjiga
            {
                Id = 5,
                Naslov = "Test Knjiga",
                Primjerci = new List<Primjerak> { new() { Status = "zaduzen" } }
            };

            var sacuvanaRezervacija = new Rezervacija
            {
                Id = 1,
                KorisnikId = 1,
                KnjigaId = 5,
                Status = "aktivna",
                Knjiga = knjiga,
                Korisnik = new Korisnik { Id = 1, Ime = "Test", Prezime = "Korisnik", Email = "test@test.com" }
            };

            _knjigaRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(knjiga);
            _rezervacijaRepoMock.Setup(r => r.HasActiveAsync(1, 5)).ReturnsAsync(false);
            _zaduzenjeRepoMock.Setup(r => r.ImaKasnelaZaduzenjaAsync(1)).ReturnsAsync(false);
            _rezervacijaRepoMock.Setup(r => r.CreateAsync(It.IsAny<Rezervacija>())).ReturnsAsync((Rezervacija r) => r);
            _rezervacijaRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(sacuvanaRezervacija);

            // Email servis baca izuzetak
            _korisnikRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Korisnik>
    {
        new() { Id = 99, UlogaId = 2, Uloga = new Uloga { Id = 2, Naziv = RoleNames.Bibliotekar }, Email = "bib@test.com", Status = "aktivan" }
    });
            _emailServiceMock
                .Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("SMTP gre�ka"));

            // Act
            var result = await _controller.Create(5);

            // Assert � akcija je i dalje uspje�na, redirect je obavljen
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirect.ActionName);
            Assert.Equal("Knjiga", redirect.ControllerName);
            Assert.Equal(5, redirect.RouteValues!["id"]);
        }
    }
}