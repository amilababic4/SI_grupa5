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
        private readonly Mock<IClanarinaRepository> _clanarinaRepoMock;
        private readonly Mock<IZaduzenjeRepository> _zaduzenjeRepoMock;
        private readonly KorisnikController _controller;

        public KorisnikWebControllerTests()
        {
            _repoMock = new Mock<IKorisnikRepository>();
            _clanarinaRepoMock = new Mock<IClanarinaRepository>();
            _zaduzenjeRepoMock = new Mock<IZaduzenjeRepository>();

            var tempDataProvider = new Mock<ITempDataProvider>();
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, tempDataProvider.Object);

            _controller = new KorisnikController(
                _repoMock.Object,
                _clanarinaRepoMock.Object,
                _zaduzenjeRepoMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                },
                TempData = tempData
            };
        }

        [Fact]
        public async Task Index_VracaViewSaListomClanova()
        {
            var korisnici = new List<Korisnik>
            {
                new Korisnik
                {
                    Ime = "Mujo",
                    Prezime = "Mujic",
                    Status = "aktivan",
                    Uloga = new Uloga { Naziv = "Član" }
                },
                new Korisnik
                {
                    Ime = "Admin",
                    Prezime = "Adminovic",
                    Status = "aktivan",
                    Uloga = new Uloga { Naziv = "Administrator" }
                }
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
            var dto = new KorisnikCreateDto
            {
                Email = "novi@test.com",
                Ime = "Test",
                Prezime = "User",
                Lozinka = "12345678",
                PotvrdaLozinke = "12345678"
            };

            _repoMock.Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync((Korisnik?)null);

            var result = await _controller.Create(dto);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Nalog člana je uspješno kreiran.", _controller.TempData["SuccessMessage"]);
        }

        [Fact]
        public async Task Create_Post_EmailVecPostoji_VracaViewSaGreskom()
        {
            var dto = new KorisnikCreateDto
            {
                Email = "postojeci@test.com",
                Ime = "Test",
                Prezime = "User",
                Lozinka = "12345678",
                PotvrdaLozinke = "12345678"
            };

            _repoMock.Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync(new Korisnik());

            var result = await _controller.Create(dto);

            Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
            Assert.True(_controller.ModelState.ContainsKey("Email"));
        }

        [Fact]
        public async Task Deaktiviraj_PostojeciKorisnik_RedirectsSaPorukom()
        {
            var korisnik = new Korisnik
            {
                Id = 1,
                Status = "aktivan",
                Uloga = new Uloga { Naziv = "Član" }
            };

            _repoMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(korisnik);

            _zaduzenjeRepoMock.Setup(r => r.GetByKorisnikAsync(1))
                .ReturnsAsync(new List<Zaduzenje>());

            var result = await _controller.Deaktiviraj(1);

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("deaktiviran", korisnik.Status);
            Assert.Equal("Nalog člana je deaktiviran.", _controller.TempData["SuccessMessage"]);
        }

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
            _repoMock.Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Korisnik?)null);

            var result = await _controller.Deaktiviraj(99);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Index_KorisnikBezUloge_NijeUkljucenUListu()
        {
            var korisnici = new List<Korisnik>
            {
                new Korisnik
                {
                    Ime = "Test",
                    Prezime = "Test",
                    Status = "aktivan",
                    Uloga = null
                }
            };

            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(korisnici);

            var result = await _controller.Index();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<KorisnikDto>>(view.Model);

            Assert.Empty(model);
        }

        [Fact]
        public async Task Index_SortiranjePoPrezimenu_IspravanRedoslijed()
        {
            var korisnici = new List<Korisnik>
            {
                new Korisnik
                {
                    Ime = "Zana",
                    Prezime = "Zoric",
                    Status = "aktivan",
                    Uloga = new Uloga { Naziv = "Član" }
                },
                new Korisnik
                {
                    Ime = "Ana",
                    Prezime = "Alic",
                    Status = "aktivan",
                    Uloga = new Uloga { Naziv = "Član" }
                },
                new Korisnik
                {
                    Ime = "Mujo",
                    Prezime = "Alic",
                    Status = "aktivan",
                    Uloga = new Uloga { Naziv = "Član" }
                }
            };

            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(korisnici);

            var result = await _controller.Index();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<KorisnikDto>>(view.Model).ToList();

            Assert.Equal(3, model.Count);
            Assert.Equal("Alic", model[0].Prezime);
            Assert.Equal("Ana", model[0].Ime);
            Assert.Equal("Alic", model[1].Prezime);
            Assert.Equal("Mujo", model[1].Ime);
        }

        [Fact]
        public async Task Index_NemaKorisnika_VracaPrazanModel()
        {
            _repoMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Korisnik>());

            var result = await _controller.Index();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<KorisnikDto>>(view.Model);

            Assert.Empty(model);
        }

        [Fact]
        public async Task Create_Post_LozinkeSeNePodudaraju_VracaViewSaGreskom()
        {
            var dto = new KorisnikCreateDto
            {
                Ime = "Test",
                Prezime = "User",
                Email = "test@test.com",
                Lozinka = "password123",
                PotvrdaLozinke = "drugiPassword"
            };

            var result = await _controller.Create(dto);

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.False(_controller.ModelState.IsValid);
            Assert.True(_controller.ModelState.ContainsKey("PotvrdaLozinke"));

            var errors = _controller.ModelState["PotvrdaLozinke"]!.Errors;

            Assert.Single(errors);
            Assert.Equal("Lozinka i potvrda lozinke se ne poklapaju.", errors[0].ErrorMessage);
            Assert.Equal(dto, viewResult.Model);
        }

        [Fact]
        public async Task Index_ClanSaSvimPoljima_MapiraSvaPoljaUDto()
        {
            var datum = new DateTime(2025, 1, 15, 10, 30, 0);

            var korisnici = new List<Korisnik>
            {
                new Korisnik
                {
                    Id = 42,
                    Ime = "Ajna",
                    Prezime = "Bajric",
                    Email = "ajna@test.com",
                    Status = "aktivan",
                    DatumKreiranja = datum,
                    Uloga = new Uloga { Naziv = "Član" }
                }
            };

            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(korisnici);

            var result = await _controller.Index();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<KorisnikDto>>(view.Model).ToList();

            Assert.Single(model);

            var dto = model[0];

            Assert.Equal(42, dto.Id);
            Assert.Equal("Ajna", dto.Ime);
            Assert.Equal("Bajric", dto.Prezime);
            Assert.Equal("ajna@test.com", dto.Email);
            Assert.Equal("Član", dto.Uloga);
            Assert.Equal("aktivan", dto.Status);
            Assert.Equal(datum, dto.DatumKreiranja);
        }
    }
}