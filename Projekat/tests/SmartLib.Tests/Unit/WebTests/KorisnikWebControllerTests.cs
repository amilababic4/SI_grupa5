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
            var dto = new KorisnikCreateDto { Email = "novi@test.com", Ime = "Test", Prezime = "User", Lozinka = "12345678" };
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
    }
}