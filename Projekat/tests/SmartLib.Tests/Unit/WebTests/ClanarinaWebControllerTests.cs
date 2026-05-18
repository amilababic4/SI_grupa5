using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Web.Controllers;
using Xunit;

namespace SmartLib.Tests.Unit.WebTests
{
    public class ClanarinaWebControllerTests
    {
        private readonly Mock<IClanarinaRepository> _clanarinaMock;
        private readonly Mock<IKorisnikRepository> _korisnikMock;
        private readonly ClanarinaController _controller;

        public ClanarinaWebControllerTests()
        {
            _clanarinaMock = new Mock<IClanarinaRepository>();
            _korisnikMock = new Mock<IKorisnikRepository>();

            _controller = new ClanarinaController(
                _clanarinaMock.Object,
                _korisnikMock.Object);

            var httpContext = new DefaultHttpContext();

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _controller.TempData = new TempDataDictionary(
                httpContext,
                Mock.Of<ITempDataProvider>());
        }

        private static Korisnik TestKorisnik(int id = 1, string status = "aktivan")
        {
            return new Korisnik
            {
                Id = id,
                Ime = "Test",
                Prezime = "Korisnik",
                Email = "test@smartlib.ba",
                Status = status,
                Uloga = new Uloga
                {
                    Id = 1,
                    Naziv = "Član"
                }
            };
        }

        private static Clanarina TestClanarina(int korisnikId = 1)
        {
            return new Clanarina
            {
                Id = 1,
                KorisnikId = korisnikId,
                DatumPocetka = DateTime.Today.AddMonths(-1),
                DatumIsteka = DateTime.Today.AddMonths(11),
                Status = "aktivna"
            };
        }

        private static ClanarinaUpsertDto ValidanDto(int korisnikId = 1)
        {
            return new ClanarinaUpsertDto
            {
                KorisnikId = korisnikId,
                DatumPocetka = DateTime.Today,
                DatumIsteka = DateTime.Today.AddYears(1)
            };
        }

        [Fact]
        public async Task Upsert_Get_NemaPostojeceClanarine_VracaViewSaNovimDto()
        {
            _clanarinaMock.Setup(r => r.GetByKorisnikAsync(1))
                .ReturnsAsync((Clanarina?)null);

            var result = await _controller.Upsert(1);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ClanarinaUpsertDto>(view.Model);

            Assert.Equal(1, model.KorisnikId);
            Assert.Equal(DateTime.Today, model.DatumPocetka.Date);
            Assert.Equal(DateTime.Today.AddYears(1), model.DatumIsteka.Date);
            Assert.False((bool)_controller.ViewBag.Postoji);
        }

        [Fact]
        public async Task Upsert_Get_PostojecaClanarina_VracaViewSaPostojecimDatumima()
        {
            var clanarina = TestClanarina(1);

            _clanarinaMock.Setup(r => r.GetByKorisnikAsync(1))
                .ReturnsAsync(clanarina);

            var result = await _controller.Upsert(1);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ClanarinaUpsertDto>(view.Model);

            Assert.Equal(1, model.KorisnikId);
            Assert.Equal(clanarina.DatumPocetka, model.DatumPocetka);
            Assert.Equal(clanarina.DatumIsteka, model.DatumIsteka);
            Assert.True((bool)_controller.ViewBag.Postoji);
        }

        [Fact]
        public async Task Upsert_Post_NeispravanDatum_VracaViewSaGreskom()
        {
            var dto = new ClanarinaUpsertDto
            {
                KorisnikId = 1,
                DatumPocetka = DateTime.Today,
                DatumIsteka = DateTime.Today
            };

            _clanarinaMock.Setup(r => r.GetByKorisnikAsync(1))
                .ReturnsAsync((Clanarina?)null);

            var result = await _controller.Upsert(dto);

            var view = Assert.IsType<ViewResult>(result);

            Assert.Equal(dto, view.Model);
            Assert.False(_controller.ModelState.IsValid);
            Assert.True(_controller.ModelState.ContainsKey(nameof(ClanarinaUpsertDto.DatumIsteka)));
        }

        [Fact]
        public async Task Upsert_Post_ModelStateInvalid_NePozivaSprema()
        {
            var dto = ValidanDto();
            _controller.ModelState.AddModelError("KorisnikId", "Greška");

            var result = await _controller.Upsert(dto);

            Assert.IsType<ViewResult>(result);

            _clanarinaMock.Verify(r => r.CreateAsync(It.IsAny<Clanarina>()), Times.Never);
            _clanarinaMock.Verify(r => r.UpdateAsync(It.IsAny<Clanarina>()), Times.Never);
        }

        [Fact]
        public async Task Upsert_Post_KorisnikNePostoji_VracaNotFound()
        {
            var dto = ValidanDto();

            _korisnikMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((Korisnik?)null);

            var result = await _controller.Upsert(dto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Upsert_Post_NovaClanarina_KreiraIRedirektuje()
        {
            var dto = ValidanDto();

            _korisnikMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(TestKorisnik(1));

            _clanarinaMock.Setup(r => r.GetByKorisnikAsync(1))
                .ReturnsAsync((Clanarina?)null);

            _clanarinaMock.Setup(r => r.CreateAsync(It.IsAny<Clanarina>()))
                .ReturnsAsync((Clanarina c) => c);

            var result = await _controller.Upsert(dto);

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("ProfilClana", redirect.ActionName);
            Assert.Equal("Korisnik", redirect.ControllerName);
            Assert.Equal(1, redirect.RouteValues!["id"]);

            _clanarinaMock.Verify(r => r.CreateAsync(It.Is<Clanarina>(
                c => c.KorisnikId == 1 &&
                     c.DatumPocetka == dto.DatumPocetka &&
                     c.DatumIsteka == dto.DatumIsteka)), Times.Once);
        }

        [Fact]
        public async Task Upsert_Post_PostojecaClanarina_AzuriraIRedirektuje()
        {
            var dto = ValidanDto();
            var postojeca = TestClanarina(1);

            _korisnikMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(TestKorisnik(1));

            _clanarinaMock.Setup(r => r.GetByKorisnikAsync(1))
                .ReturnsAsync(postojeca);

            _clanarinaMock.Setup(r => r.UpdateAsync(It.IsAny<Clanarina>()))
                .Returns(Task.CompletedTask);

            var result = await _controller.Upsert(dto);

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("ProfilClana", redirect.ActionName);
            Assert.Equal("Korisnik", redirect.ControllerName);
            Assert.Equal(1, redirect.RouteValues!["id"]);

            Assert.Equal(dto.DatumPocetka, postojeca.DatumPocetka);
            Assert.Equal(dto.DatumIsteka, postojeca.DatumIsteka);

            _clanarinaMock.Verify(r => r.UpdateAsync(postojeca), Times.Once);
            _clanarinaMock.Verify(r => r.CreateAsync(It.IsAny<Clanarina>()), Times.Never);
        }

        [Fact]
        public async Task Upsert_Post_DeaktiviranKorisnik_AktiviraKorisnika()
        {
            var dto = ValidanDto();

            var korisnik = TestKorisnik(1, "deaktiviran");
            korisnik.DatumDeaktivacije = DateTime.UtcNow.AddDays(-5);

            _korisnikMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(korisnik);

            _clanarinaMock.Setup(r => r.GetByKorisnikAsync(1))
                .ReturnsAsync((Clanarina?)null);

            _clanarinaMock.Setup(r => r.CreateAsync(It.IsAny<Clanarina>()))
                .ReturnsAsync((Clanarina c) => c);

            await _controller.Upsert(dto);

            Assert.Equal("aktivan", korisnik.Status);
            Assert.Null(korisnik.DatumDeaktivacije);

            _korisnikMock.Verify(r => r.UpdateAsync(korisnik), Times.Once);
        }

        [Fact]
        public async Task Upsert_Post_AktivanKorisnik_NePozivaUpdateKorisnika()
        {
            var dto = ValidanDto();
            var korisnik = TestKorisnik(1, "aktivan");

            _korisnikMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(korisnik);

            _clanarinaMock.Setup(r => r.GetByKorisnikAsync(1))
                .ReturnsAsync((Clanarina?)null);

            _clanarinaMock.Setup(r => r.CreateAsync(It.IsAny<Clanarina>()))
                .ReturnsAsync((Clanarina c) => c);

            await _controller.Upsert(dto);

            _korisnikMock.Verify(r => r.UpdateAsync(It.IsAny<Korisnik>()), Times.Never);
        }

        [Fact]
        public async Task Upsert_Post_Uspjesno_PostavljaTempDataPoruku()
        {
            var dto = ValidanDto();

            _korisnikMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(TestKorisnik(1));

            _clanarinaMock.Setup(r => r.GetByKorisnikAsync(1))
                .ReturnsAsync((Clanarina?)null);

            _clanarinaMock.Setup(r => r.CreateAsync(It.IsAny<Clanarina>()))
                .ReturnsAsync((Clanarina c) => c);

            await _controller.Upsert(dto);

            Assert.Equal("Članarina je uspješno spremljena.", _controller.TempData["Uspjeh"]);
        }
    }
}