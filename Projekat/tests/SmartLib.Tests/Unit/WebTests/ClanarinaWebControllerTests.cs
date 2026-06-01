using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Web.Controllers;
using System.Security.Claims;
using Xunit;

namespace SmartLib.Tests.Unit.WebTests
{
    public class ClanarinaWebControllerTests
    {
        private readonly Mock<IClanarinaRepository> _clanarinaMock;
        private readonly Mock<IKorisnikRepository> _korisnikMock;
        private readonly Mock<IZahtjevProduzenjaRepository> _zahtjevMock; // DODATO
        private readonly ClanarinaController _controller;

        public ClanarinaWebControllerTests()
        {
            _clanarinaMock = new Mock<IClanarinaRepository>();
            _korisnikMock = new Mock<IKorisnikRepository>();
            _zahtjevMock = new Mock<IZahtjevProduzenjaRepository>(); // DODATO

            _controller = new ClanarinaController(
                _clanarinaMock.Object,
                _korisnikMock.Object,
                _zahtjevMock.Object); // DODATO

            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            _controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        }

        private void SetAuthenticatedUser(int userId = 1)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new("korisnikId", userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);
        }

        private static Korisnik TestKorisnik(int id = 1, string status = "aktivan") => new()
        {
            Id = id,
            Ime = "Test",
            Prezime = "Korisnik",
            Email = "test@smartlib.ba",
            Status = status,
            Uloga = new Uloga { Id = 1, Naziv = "Član" }
        };

        private static Clanarina TestClanarina(int korisnikId = 1) => new()
        {
            Id = 1,
            KorisnikId = korisnikId,
            DatumPocetka = DateTime.Today.AddMonths(-1),
            DatumIsteka = DateTime.Today.AddMonths(11),
            Status = "aktivna"
        };

        private static ClanarinaUpsertDto ValidanDto(int korisnikId = 1) => new()
        {
            KorisnikId = korisnikId,
            DatumPocetka = DateTime.Today,
            DatumIsteka = DateTime.Today.AddYears(1)
        };

        // ── Upsert GET ────────────────────────────────────────────────────

        [Fact]
        public async Task Upsert_Get_NemaPostojeceClanarine_VracaViewSaNovimDto()
        {
            _clanarinaMock.Setup(r => r.GetByKorisnikAsync(1)).ReturnsAsync((Clanarina?)null);

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
            _clanarinaMock.Setup(r => r.GetByKorisnikAsync(1)).ReturnsAsync(clanarina);

            var result = await _controller.Upsert(1);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ClanarinaUpsertDto>(view.Model);
            Assert.Equal(clanarina.DatumPocetka, model.DatumPocetka);
            Assert.Equal(clanarina.DatumIsteka, model.DatumIsteka);
            Assert.True((bool)_controller.ViewBag.Postoji);
        }

        // ── Upsert POST ───────────────────────────────────────────────────

        [Fact]
        public async Task Upsert_Post_NeispravanDatum_VracaViewSaGreskom()
        {
            var dto = new ClanarinaUpsertDto
            {
                KorisnikId = 1,
                DatumPocetka = DateTime.Today,
                DatumIsteka = DateTime.Today
            };
            _clanarinaMock.Setup(r => r.GetByKorisnikAsync(1)).ReturnsAsync((Clanarina?)null);

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
            _korisnikMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Korisnik?)null);

            var result = await _controller.Upsert(dto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Upsert_Post_NovaClanarina_KreiraIRedirektuje()
        {
            var dto = ValidanDto();
            _korisnikMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKorisnik(1));
            _clanarinaMock.Setup(r => r.GetByKorisnikAsync(1)).ReturnsAsync((Clanarina?)null);
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
            _korisnikMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKorisnik(1));
            _clanarinaMock.Setup(r => r.GetByKorisnikAsync(1)).ReturnsAsync(postojeca);
            _clanarinaMock.Setup(r => r.UpdateAsync(It.IsAny<Clanarina>())).Returns(Task.CompletedTask);

            var result = await _controller.Upsert(dto);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ProfilClana", redirect.ActionName);
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
            _korisnikMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(korisnik);
            _clanarinaMock.Setup(r => r.GetByKorisnikAsync(1)).ReturnsAsync((Clanarina?)null);
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
            _korisnikMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKorisnik(1, "aktivan"));
            _clanarinaMock.Setup(r => r.GetByKorisnikAsync(1)).ReturnsAsync((Clanarina?)null);
            _clanarinaMock.Setup(r => r.CreateAsync(It.IsAny<Clanarina>()))
                .ReturnsAsync((Clanarina c) => c);

            await _controller.Upsert(dto);

            _korisnikMock.Verify(r => r.UpdateAsync(It.IsAny<Korisnik>()), Times.Never);
        }

        [Fact]
        public async Task Upsert_Post_Uspjesno_PostavljaTempDataPoruku()
        {
            var dto = ValidanDto();
            _korisnikMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKorisnik(1));
            _clanarinaMock.Setup(r => r.GetByKorisnikAsync(1)).ReturnsAsync((Clanarina?)null);
            _clanarinaMock.Setup(r => r.CreateAsync(It.IsAny<Clanarina>()))
                .ReturnsAsync((Clanarina c) => c);

            await _controller.Upsert(dto);

            Assert.Equal("Članarina je uspješno spremljena.", _controller.TempData["Uspjeh"]);
        }

        // ── ZahtjeviProduzenja ────────────────────────────────────────────

        [Fact]
        public async Task ZahtjeviProduzenja_VracaView()
        {
            _zahtjevMock.Setup(r => r.GetNaCekanjuAsync())
                .ReturnsAsync(new List<ZahtjevProduzenja>());

            var result = await _controller.ZahtjeviProduzenja();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task ZahtjeviProduzenja_PrikazujeZahtjeveNaCekanju()
        {
            var zahtjevi = new List<ZahtjevProduzenja>
            {
                new() {
                    Id = 1, KorisnikId = 1, TrajanjeMjeseci = 6,
                    Status = "na_cekanju", DatumPodnosenja = DateTime.UtcNow,
                    Korisnik = new Korisnik { Ime = "Ana", Prezime = "Anić", Email = "ana@test.ba" }
                }
            };
            _zahtjevMock.Setup(r => r.GetNaCekanjuAsync()).ReturnsAsync(zahtjevi);
            _clanarinaMock.Setup(r => r.GetByKorisnikAsync(1)).ReturnsAsync(TestClanarina(1));

            var result = await _controller.ZahtjeviProduzenja();

            var view = Assert.IsType<ViewResult>(result);
            var vm = Assert.IsType<ZahtjeviProduzenjaViewModel>(view.Model);
            Assert.Single(vm.NaCekanju);
        }

        // ── ObradiZahtjev ─────────────────────────────────────────────────

        [Fact]
        public async Task ObradiZahtjev_ModelStateInvalid_VracaGreskuIRedirektuje()
        {
            _controller.ModelState.AddModelError("ZahtjevId", "Greška");
            var dto = new ZahtjevProduzenjaObradiDto { ZahtjevId = 1, Akcija = "odobreno" };

            var result = await _controller.ObradiZahtjev(dto);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ZahtjeviProduzenja", redirect.ActionName);
            Assert.Equal("Neispravan zahtjev.", _controller.TempData["Greska"]);
        }

        [Fact]
        public async Task ObradiZahtjev_ZahtjevNijePronadjen_VracaGresku()
        {
            _zahtjevMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((ZahtjevProduzenja?)null);
            var dto = new ZahtjevProduzenjaObradiDto { ZahtjevId = 99, Akcija = "odobreno" };

            var result = await _controller.ObradiZahtjev(dto);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ZahtjeviProduzenja", redirect.ActionName);
            Assert.Equal("Zahtjev nije pronađen ili je već obrađen.", _controller.TempData["Greska"]);
        }

        [Fact]
        public async Task ObradiZahtjev_Odobravanje_BezPostojeceClanarine_KreiraNovaIRedirektuje()
        {
            SetAuthenticatedUser(2);
            var zahtjev = new ZahtjevProduzenja
            {
                Id = 1,
                KorisnikId = 1,
                TrajanjeMjeseci = 6,
                Status = "na_cekanju"
            };
            _zahtjevMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(zahtjev);
            _clanarinaMock.Setup(r => r.GetByKorisnikAsync(1)).ReturnsAsync((Clanarina?)null);
            _clanarinaMock.Setup(r => r.CreateAsync(It.IsAny<Clanarina>()))
                .ReturnsAsync((Clanarina c) => c);
            _zahtjevMock.Setup(r => r.UpdateAsync(It.IsAny<ZahtjevProduzenja>())).Returns(Task.CompletedTask);

            var result = await _controller.ObradiZahtjev(
                new ZahtjevProduzenjaObradiDto { ZahtjevId = 1, Akcija = "odobreno" });

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ZahtjeviProduzenja", redirect.ActionName);
            Assert.Equal("odobreno", zahtjev.Status);
            _clanarinaMock.Verify(r => r.CreateAsync(It.IsAny<Clanarina>()), Times.Once);
        }

        [Fact]
        public async Task ObradiZahtjev_Odobravanje_SaPostojecamClanarinom_AzuriraIRedirektuje()
        {
            SetAuthenticatedUser(2);
            var zahtjev = new ZahtjevProduzenja
            {
                Id = 1,
                KorisnikId = 1,
                TrajanjeMjeseci = 3,
                Status = "na_cekanju"
            };
            var clanarina = TestClanarina(1);
            _zahtjevMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(zahtjev);
            _clanarinaMock.Setup(r => r.GetByKorisnikAsync(1)).ReturnsAsync(clanarina);
            _clanarinaMock.Setup(r => r.UpdateAsync(It.IsAny<Clanarina>())).Returns(Task.CompletedTask);
            _zahtjevMock.Setup(r => r.UpdateAsync(It.IsAny<ZahtjevProduzenja>())).Returns(Task.CompletedTask);

            await _controller.ObradiZahtjev(
                new ZahtjevProduzenjaObradiDto { ZahtjevId = 1, Akcija = "odobreno" });

            Assert.Equal("odobreno", zahtjev.Status);
            _clanarinaMock.Verify(r => r.UpdateAsync(clanarina), Times.Once);
            _clanarinaMock.Verify(r => r.CreateAsync(It.IsAny<Clanarina>()), Times.Never);
        }

        [Fact]
        public async Task ObradiZahtjev_Odbijanje_BezRazloga_VracaGresku()
        {
            var zahtjev = new ZahtjevProduzenja
            {
                Id = 1,
                KorisnikId = 1,
                TrajanjeMjeseci = 6,
                Status = "na_cekanju"
            };
            _zahtjevMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(zahtjev);

            var result = await _controller.ObradiZahtjev(new ZahtjevProduzenjaObradiDto
            {
                ZahtjevId = 1,
                Akcija = "odbijeno",
                RazlogOdbijanja = ""
            });

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ZahtjeviProduzenja", redirect.ActionName);
            Assert.Equal("Razlog odbijanja je obavezan.", _controller.TempData["Greska"]);
        }

        [Fact]
        public async Task ObradiZahtjev_Odbijanje_SaRazlogom_Redirektuje()
        {
            SetAuthenticatedUser(2);
            var zahtjev = new ZahtjevProduzenja
            {
                Id = 1,
                KorisnikId = 1,
                TrajanjeMjeseci = 6,
                Status = "na_cekanju"
            };
            _zahtjevMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(zahtjev);
            _zahtjevMock.Setup(r => r.UpdateAsync(It.IsAny<ZahtjevProduzenja>())).Returns(Task.CompletedTask);

            var result = await _controller.ObradiZahtjev(new ZahtjevProduzenjaObradiDto
            {
                ZahtjevId = 1,
                Akcija = "odbijeno",
                RazlogOdbijanja = "Nedovoljan razlog."
            });

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ZahtjeviProduzenja", redirect.ActionName);
            Assert.Equal("odbijeno", zahtjev.Status);
        }

        // ── ProduzenjeClanarine (član) ─────────────────────────────────────

        [Fact]
        public async Task ProduzenjeClanarine_KorisnikNijePrijavljen_VracaUnauthorized()
        {
            // Nije pozvan SetAuthenticatedUser — nema claima
            var result = await _controller.ProduzenjeClanarine();

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task ProduzenjeClanarine_PrijavljenClan_VracaView()
        {
            SetAuthenticatedUser(1);
            _clanarinaMock.Setup(r => r.GetByKorisnikAsync(1)).ReturnsAsync(TestClanarina(1));
            _zahtjevMock.Setup(r => r.GetAktivniZahtjevAsync(1)).ReturnsAsync((ZahtjevProduzenja?)null);
            _zahtjevMock.Setup(r => r.GetZahtjeviByKorisnikAsync(1))
                .ReturnsAsync(new List<ZahtjevProduzenja>());

            var result = await _controller.ProduzenjeClanarine();

            Assert.IsType<ViewResult>(result);
        }

        // ── PodnesizahtjevProduzenja ──────────────────────────────────────

        [Fact]
        public async Task PodnesizahtjevProduzenja_KorisnikNijePrijavljen_VracaUnauthorized()
        {
            var dto = new ZahtjevProduzenjaCreateDto { TrajanjeMjeseci = 6 };

            var result = await _controller.PodnesizahtjevProduzenja(dto);

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task PodnesizahtjevProduzenja_VecPostojiAktivniZahtjev_VracaGresku()
        {
            SetAuthenticatedUser(1);
            _zahtjevMock.Setup(r => r.GetAktivniZahtjevAsync(1))
                .ReturnsAsync(new ZahtjevProduzenja { Id = 5, Status = "na_cekanju" });

            var result = await _controller.PodnesizahtjevProduzenja(
                new ZahtjevProduzenjaCreateDto { TrajanjeMjeseci = 6 });

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ProduzenjeClanarine", redirect.ActionName);
            Assert.Equal("Već imate zahtjev na čekanju. Pričekajte obradu.", _controller.TempData["Greska"]);
        }

        [Fact]
        public async Task PodnesizahtjevProduzenja_NedozvoljenoTrajanje_VracaGresku()
        {
            SetAuthenticatedUser(1);
            _zahtjevMock.Setup(r => r.GetAktivniZahtjevAsync(1))
                .ReturnsAsync((ZahtjevProduzenja?)null);

            var result = await _controller.PodnesizahtjevProduzenja(
                new ZahtjevProduzenjaCreateDto { TrajanjeMjeseci = 5 }); // 5 nije dozvoljeno

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ProduzenjeClanarine", redirect.ActionName);
            Assert.Equal("Neispravan unos. Odaberite trajanje produženja.", _controller.TempData["Greska"]);
        }
    }
}