using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Web.Controllers;
using SmartLib.Infrastructure.Security;
using Xunit;

namespace SmartLib.Tests.Unit.WebTests
{
    /// <summary>
    /// US-04: Prijava putem web-a
    /// US-05: Generička poruka pri neuspjehu, redirect po ulozi
    /// US-06: Odjava korisnika
    /// </summary>
    public class AuthWebControllerTests
    {
        private readonly Mock<IKorisnikRepository> _repoMock;
        private readonly AuthController _controller;

        public AuthWebControllerTests()
        {
            _repoMock = new Mock<IKorisnikRepository>();

            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(s => s.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(),
                    It.IsAny<System.Security.Claims.ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            authServiceMock
                .Setup(s => s.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(),
                    It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);

            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProviderMock.Object
            };

            _controller = new AuthController(_repoMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                },
                TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
            };

            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(u => u.IsLocalUrl(It.IsAny<string>())).Returns(true);

            _controller.Url = urlHelperMock.Object;
        }

        private static Korisnik KorisnikSaUlogom(string uloga) => new()
        {
            Id = 1,
            Ime = "Test",
            Prezime = "User",
            Email = "test@smartlib.ba",
            LozinkaHash = SmartLib.Infrastructure.Security.PasswordHasher.HashPassword("Lozinka1!"),
            Status = "aktivan",
            Uloga = new Uloga { Id = 1, Naziv = uloga }
        };

        // US-04: Clan se preusmjerava na Home 

        [Fact]
        public async Task Login_UspjesanClan_RedirectNaHomeIndex()
        {
            _repoMock.Setup(r => r.GetByEmailAsync("test@smartlib.ba"))
                     .ReturnsAsync(KorisnikSaUlogom("Član"));

            var result = await _controller.Login(new LoginRequest
            {
                Email = "test@smartlib.ba",
                Lozinka = "Lozinka1!"
            });

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
        }

        //  US-04: Bibliotekar se preusmjerava na Korisnik/Index 

        [Fact]
        public async Task Login_UspjesanBibliotekar_RedirectNaKorisnikIndex()
        {
            _repoMock.Setup(r => r.GetByEmailAsync("test@smartlib.ba"))
                     .ReturnsAsync(KorisnikSaUlogom("Bibliotekar"));

            var result = await _controller.Login(new LoginRequest
            {
                Email = "test@smartlib.ba",
                Lozinka = "Lozinka1!"
            });

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Korisnik", redirect.ControllerName);
        }

        // US-04: Administrator se preusmjerava na Korisnik/Index

        [Fact]
        public async Task Login_UspjesanAdministrator_RedirectNaKorisnikIndex()
        {
            _repoMock.Setup(r => r.GetByEmailAsync("test@smartlib.ba"))
                     .ReturnsAsync(KorisnikSaUlogom("Administrator"));

            var result = await _controller.Login(new LoginRequest
            {
                Email = "test@smartlib.ba",
                Lozinka = "Lozinka1!"
            });

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Korisnik", redirect.ControllerName);
        }

        // US-05: Pogrešna lozinka vraća View (ne otkriva razlog) 

        [Fact]
        public async Task Login_PogresnaLozinka_VracaViewSaGenerickomPorukom()
        {
            _repoMock.Setup(r => r.GetByEmailAsync("test@smartlib.ba"))
                     .ReturnsAsync(KorisnikSaUlogom("Član"));

            var result = await _controller.Login(new LoginRequest
            {
                Email = "test@smartlib.ba",
                Lozinka = "PogresnaLozinka!"
            });

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);

            // Greška je na string.Empty — ne kaže koji je razlog
            var errors = _controller.ModelState[string.Empty]?.Errors;
            Assert.NotNull(errors);
            Assert.Contains(errors!, e => e.ErrorMessage == "Prijava nije uspjela.");
        }

        // US-05: Deaktiviran korisnik — ista generička poruka 

        [Fact]
        public async Task Login_DeaktiviranKorisnik_VracaGenericku()
        {
            var k = KorisnikSaUlogom("Član");
            k.Status = "deaktiviran";
            _repoMock.Setup(r => r.GetByEmailAsync("test@smartlib.ba")).ReturnsAsync(k);

            var result = await _controller.Login(new LoginRequest
            {
                Email = "test@smartlib.ba",
                Lozinka = "Lozinka1!"
            });

            Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
        }

        // US-06: Odjava briše sesiju i redirekuje na Login 

        [Fact]
        public async Task Logout_UvijekRedirectNaLoginStranu()
        {
            var result = await _controller.Logout();

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirect.ActionName);
            Assert.Equal("Auth", redirect.ControllerName);
        }

        // US-04: returnUrl redirect (lokalni URL)

        [Fact]
        public async Task Login_ValjanReturnUrl_RedirectNaTajUrl()
        {
            _repoMock.Setup(r => r.GetByEmailAsync("test@smartlib.ba"))
                     .ReturnsAsync(KorisnikSaUlogom("Član"));

            // Podešavamo Url helper koji vraća IsLocalUrl=true
            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(u => u.IsLocalUrl("/knjige")).Returns(true);
            _controller.Url = urlHelperMock.Object;

            var result = await _controller.Login(new LoginRequest
            {
                Email = "test@smartlib.ba",
                Lozinka = "Lozinka1!"
            }, returnUrl: "/knjige");

            var redirect = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/knjige", redirect.Url);
        }

        // US-05: Nepostojeći korisnik

        [Fact]
        public async Task Login_NepostojeciKorisnik_VracaViewSaGreskom()
        {
            _repoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((Korisnik?)null);

            var result = await _controller.Login(new LoginRequest { Email = "nema@me.com", Lozinka = "bilo-sta" });

            Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
            Assert.Equal("Prijava nije uspjela.", _controller.ModelState[string.Empty]?.Errors[0].ErrorMessage);
        }

        [Fact]
        public void Login_Get_VracaView()
        {
            var result = _controller.Login();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Login_Get_SaReturnUrl_PostavljaViewData()
        {
            var result = _controller.Login("/knjige") as ViewResult;

            Assert.Equal("/knjige", _controller.ViewData["ReturnUrl"]);
        }

        [Fact]
        public async Task Login_Post_ModelStateInvalid_VracaView()
        {
            _controller.ModelState.AddModelError("Email", "Obavezno");

            var result = await _controller.Login(new LoginRequest());

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Login_NelokalniReturnUrl_RedirectNaDefaultUlogu()
        {
            _repoMock.Setup(r => r.GetByEmailAsync("test@smartlib.ba"))
                     .ReturnsAsync(KorisnikSaUlogom("Član"));

            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(u => u.IsLocalUrl("https://evil.com")).Returns(false);
            _controller.Url = urlHelperMock.Object;

            var result = await _controller.Login(new LoginRequest
            {
                Email = "test@smartlib.ba",
                Lozinka = "Lozinka1!"
            }, returnUrl: "https://evil.com");

            // Ignorira zlonamjerni URL, redirekuje na Home
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
        }

        [Fact]
        public async Task Login_KorisnikBezUloge_UspjesnoSeLoguje()
        {
            var korisnik = KorisnikSaUlogom("Član");
            korisnik.Uloga = null; // ← null uloga

            _repoMock.Setup(r => r.GetByEmailAsync("test@smartlib.ba"))
                     .ReturnsAsync(korisnik);

            var result = await _controller.Login(new LoginRequest
            {
                Email = "test@smartlib.ba",
                Lozinka = "Lozinka1!"
            });

            // Ne baca exception, redirekuje na Home (nije Bibliotekar ni Administrator)
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", redirect.ControllerName);
        }
    }
}
