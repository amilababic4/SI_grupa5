using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using SmartLib.Web.Controllers;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using Xunit;

namespace SmartLib.Tests.Unit.WebTests
{
    public class KorisnikWebControllerTests
    {
        private readonly Mock<IKorisnikRepository> _repoMock;
        private readonly KorisnikController _controller;

        public KorisnikWebControllerTests()
        {
            _repoMock = new Mock<IKorisnikRepository>();

            var tempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new Microsoft.AspNetCore.Http.DefaultHttpContext(), tempDataProvider.Object);

            _controller = new KorisnikController(_repoMock.Object)
            {
                TempData = tempData
            };
        }

        [Fact]
        public async Task Index_VracaViewSaListomClanova()
        {
            var korisnici = new List<Korisnik>
            {
                new Korisnik { Ime = "Mujo", Prezime = "Mujic", Uloga = new Uloga { Naziv = "Član" } },
                new Korisnik { Ime = "Admin", Prezime = "Adminovic", Uloga = new Uloga { Naziv = "Administrator" } }
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(korisnici);

            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<KorisnikDto>>(viewResult.Model);

            Assert.Single(model);
        }

        [Fact]
        public async Task Create_Post_ValidanModel_RedirectsToIndex()
        {
            var dto = new KorisnikCreateDto { Email = "novi@test.com", Ime = "Test", Prezime = "User", Lozinka = "12345678", PotvrdaLozinke = "12345678" };
            _repoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync((Korisnik?)null);

            var result = await _controller.Create(dto);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Nalog člana je uspješno kreiran.", _controller.TempData["SuccessMessage"]);
        }

        [Fact]
        public async Task Create_Post_EmailVecPostoji_VracaViewSaGreskom()
        {
            var dto = new KorisnikCreateDto { Email = "postojeci@test.com" };
            _repoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(new Korisnik());

            var result = await _controller.Create(dto);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
            Assert.True(_controller.ModelState.ContainsKey("Email"));
        }

        [Fact]
        public async Task Deaktiviraj_PostojeciKorisnik_RedirectsSaPorukom()
        {
            var korisnik = new Korisnik { Id = 1, Status = "aktivan" };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(korisnik);

            var result = await _controller.Deaktiviraj(1);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("deaktiviran", korisnik.Status);
            Assert.Equal("Nalog člana je deaktiviran.", _controller.TempData["SuccessMessage"]);
        }

        //test napravljen samo da se pokrije pregled profila iako to jos uvijek nije implementirano, bit ce u kasnijim sprintovima
        [Fact]
        public void Create_Get_VracaViewSaPraznimModelom()
        {
            var result = _controller.Create();

            var view = Assert.IsType<ViewResult>(result);
            Assert.IsType<KorisnikCreateDto>(view.Model);
        }

        [Fact]
        public async Task Create_Post_NeispravanModel_VracaView()
        {
            _controller.ModelState.AddModelError("Ime", "Obavezno");

            var result = await _controller.Create(new KorisnikCreateDto());

            Assert.IsType<ViewResult>(result);
            _repoMock.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Deaktiviraj_NepostojeciKorisnik_VracaNotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Korisnik?)null);

            var result = await _controller.Deaktiviraj(99);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Profil_VracaView()
        {
            var result = await _controller.Profil();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Index_KorisnikBezUloge_NijeUkljucenUListu()
        {
            var korisnici = new List<Korisnik>
    {
        new() { Ime = "Test", Prezime = "Test", Uloga = null } // null uloga
    };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(korisnici);

            var result = await _controller.Index();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<KorisnikDto>>(view.Model);
            Assert.Empty(model); // null uloga != "Član", ne uključuje se
        }

        [Fact]
        public async Task Index_SortiranjePoPrezimenu_IspravanRedoslijed()
        {
            var korisnici = new List<Korisnik>
    {
        new() { Ime = "Zana",  Prezime = "Zoric",
                Uloga = new Uloga { Naziv = "Član" } },
        new() { Ime = "Ana",   Prezime = "Alic",
                Uloga = new Uloga { Naziv = "Član" } },
        new() { Ime = "Mujo",  Prezime = "Alic",
                Uloga = new Uloga { Naziv = "Član" } }
    };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(korisnici);

            var result = await _controller.Index();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<KorisnikDto>>(view.Model).ToList();
            Assert.Equal(3, model.Count);
            Assert.Equal("Alic", model[0].Prezime);
            Assert.Equal("Ana", model[0].Ime);   // ThenBy Ime
            Assert.Equal("Alic", model[1].Prezime);
            Assert.Equal("Mujo", model[1].Ime);
        }

        [Fact]
        public async Task Index_NemaKorisnika_VracaPrazanModel()
        {
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Korisnik>());

            var result = await _controller.Index();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<KorisnikDto>>(view.Model);
            Assert.Empty(model);
        }
    }
}