using System.Security.Claims;
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
    public class ZaduzenjeWebControllerTests
    {
        private readonly Mock<IZaduzenjeRepository> _zaduzenjeRepo;
        private readonly Mock<IKorisnikRepository> _korisnikRepo;
        private readonly Mock<IKnjigaRepository> _knjigaRepo;
        private readonly Mock<IPrimjerakRepository> _primjerakRepo;
        private readonly ZaduzenjeController _controller;

        public ZaduzenjeWebControllerTests()
        {
            _zaduzenjeRepo = new Mock<IZaduzenjeRepository>();
            _korisnikRepo = new Mock<IKorisnikRepository>();
            _knjigaRepo = new Mock<IKnjigaRepository>();
            _primjerakRepo = new Mock<IPrimjerakRepository>();

            _controller = new ZaduzenjeController(
                _zaduzenjeRepo.Object,
                _korisnikRepo.Object,
                _knjigaRepo.Object,
                _primjerakRepo.Object);

            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            _controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static Korisnik TestClan(int id = 10) => new()
        {
            Id = id,
            Ime = "Ana",
            Prezime = "Anić",
            Email = "ana@test.ba",
            UlogaId = 1,
            Status = "aktivan",
            Uloga = new Uloga { Id = 1, Naziv = "Član" }
        };

        private static Knjiga TestKnjiga(int id = 1) => new()
        {
            Id = id,
            Naslov = "Test knjiga",
            Autor = "Test autor",
            Isbn = "9780000000001",
            KategorijaId = 1,
            GodinaIzdanja = 2020,
            Primjerci = new List<Primjerak>
            {
                new() { Id = 5, KnjigaId = id, InventarniBroj = "INV-1-001", Status = "dostupan" }
            }
        };

        private static Primjerak TestPrimjerak(int id = 5, string status = "dostupan") => new()
        {
            Id = id,
            KnjigaId = 1,
            InventarniBroj = "INV-1-001",
            Status = status,
            Knjiga = TestKnjiga()
        };

        private static Zaduzenje TestZaduzenje(int id = 1, string status = "aktivno") => new()
        {
            Id = id,
            KorisnikId = 10,
            PrimjerakId = 5,
            DatumZaduzivanja = DateTime.UtcNow.AddDays(-3),
            DatumPlaniranogVracanja = DateTime.UtcNow.AddDays(11),
            Status = status,
            Korisnik = TestClan(),
            Primjerak = TestPrimjerak()
        };

        private void SetAuthenticatedUser(int userId = 10)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Name, "Ana Anić"),
                new(ClaimTypes.Role, "Član")
            };
            _controller.ControllerContext.HttpContext.User =
                new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
        }

        private ZaduzenjeCreateDto ValidDto() => new()
        {
            KorisnikId = 10,
            KnjigaId = 1,
            PrimjerakId = 5
        };

        private void SetupCreateDropdownMocks()
        {
            _korisnikRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Korisnik> { TestClan() });
            _knjigaRepo.Setup(r => r.SearchAsync(null, null)).ReturnsAsync(new List<Knjiga> { TestKnjiga() });
        }

        // ── Index ─────────────────────────────────────────────────────────────────

        [Fact]
        public async Task Index_VracaAktivnaZaduzenja()
        {
            _zaduzenjeRepo.Setup(r => r.GetActiveAsync())
                .ReturnsAsync(new List<Zaduzenje> { TestZaduzenje() });

            var result = await _controller.Index(null);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AktivnaZaduzenjaViewModel>(view.Model);
            Assert.Single(model.Zaduzenja);
        }

        [Fact]
        public async Task Index_NemaZaduzenja_VracaPrazanModel()
        {
            _zaduzenjeRepo.Setup(r => r.GetActiveAsync())
                .ReturnsAsync(new List<Zaduzenje>());

            var result = await _controller.Index(null);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AktivnaZaduzenjaViewModel>(view.Model);
            Assert.Empty(model.Zaduzenja);
        }

        [Fact]
        public async Task Index_FilterPoClanu_VracaFilteriranaZaduzenja()
        {
            var zaduzenja = new List<Zaduzenje>
            {
                TestZaduzenje(1),
                new()
                {
                    Id = 2,
                    KorisnikId = 99,
                    PrimjerakId = 5,
                    DatumZaduzivanja = DateTime.UtcNow.AddDays(-1),
                    DatumPlaniranogVracanja = DateTime.UtcNow.AddDays(13),
                    Status = "aktivno",
                    Korisnik = new Korisnik { Ime = "Drugi", Prezime = "Korisnik", Email = "drugi@test.ba" },
                    Primjerak = TestPrimjerak()
                }
            };
            _zaduzenjeRepo.Setup(r => r.GetActiveAsync()).ReturnsAsync(zaduzenja);

            var result = await _controller.Index("Ana");

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AktivnaZaduzenjaViewModel>(view.Model);
            Assert.Single(model.Zaduzenja);
            Assert.Contains("Ana", model.Zaduzenja.First().KorisnikIme);
        }

        [Fact]
        public async Task Index_AktivnaZaduzenjaSortiranaPoRoku()
        {
            var z1 = TestZaduzenje(1);
            z1.DatumPlaniranogVracanja = DateTime.UtcNow.AddDays(10);
            var z2 = TestZaduzenje(2);
            z2.DatumPlaniranogVracanja = DateTime.UtcNow.AddDays(2);

            // Repository vraća sortirano (ORDER BY u SQL-u)
            _zaduzenjeRepo.Setup(r => r.GetActiveAsync())
                .ReturnsAsync(new List<Zaduzenje> { z2, z1 });

            var result = await _controller.Index(null);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AktivnaZaduzenjaViewModel>(view.Model);
            var lista = model.Zaduzenja.ToList();
            Assert.True(lista[0].DatumPlaniranogVracanja <= lista[1].DatumPlaniranogVracanja);
        }

        // ── Moja ──────────────────────────────────────────────────────────────────

        [Fact]
        public async Task Moja_VracaSamoVlastiteAktivnaZaduzenja()
        {
            SetAuthenticatedUser(10);
            _zaduzenjeRepo.Setup(r => r.GetByKorisnikAsync(10))
                .ReturnsAsync(new List<Zaduzenje> { TestZaduzenje() });

            var result = await _controller.Moja();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ZaduzenjeViewModel>>(view.Model);
            Assert.Single(model);
            _zaduzenjeRepo.Verify(r => r.GetByKorisnikAsync(10), Times.Once);
            _zaduzenjeRepo.Verify(r => r.GetByKorisnikAsync(It.Is<int>(id => id != 10)), Times.Never);
        }

        [Fact]
        public async Task Moja_KorisnikBezZaduzenja_VracaPrazanSeznam()
        {
            SetAuthenticatedUser(10);
            _zaduzenjeRepo.Setup(r => r.GetByKorisnikAsync(10))
                .ReturnsAsync(new List<Zaduzenje>());

            var result = await _controller.Moja();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ZaduzenjeViewModel>>(view.Model);
            Assert.Empty(model);
        }

        // ── Create / Zaduzi ───────────────────────────────────────────────────────

        [Fact]
        public async Task Zaduzi_ValidniPodaci_KreiraZaduzenjeIRedirektuje()
        {
            var dto = ValidDto();
            _primjerakRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(TestPrimjerak(5, "dostupan"));
            _primjerakRepo.Setup(r => r.HasActiveZaduzenjeAsync(5)).ReturnsAsync(false);
            _zaduzenjeRepo.Setup(r => r.CreateAsync(It.IsAny<Zaduzenje>()))
                .ReturnsAsync((Zaduzenje z) => z);
            _primjerakRepo.Setup(r => r.UpdateStatusAsync(5, "zadužen")).Returns(Task.CompletedTask);

            var result = await _controller.Zaduzi(dto);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task Zaduzi_BezDatumaPovratka_PostavljaRok2Mjeseca()
        {
            var dto = ValidDto();
            Zaduzenje? saved = null;
            _primjerakRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(TestPrimjerak(5, "dostupan"));
            _primjerakRepo.Setup(r => r.HasActiveZaduzenjeAsync(5)).ReturnsAsync(false);
            _zaduzenjeRepo.Setup(r => r.CreateAsync(It.IsAny<Zaduzenje>()))
                .Callback<Zaduzenje>(z => saved = z)
                .ReturnsAsync((Zaduzenje z) => z);
            _primjerakRepo.Setup(r => r.UpdateStatusAsync(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            await _controller.Zaduzi(dto);

            Assert.NotNull(saved);
            Assert.Equal(saved!.DatumZaduzivanja.AddMonths(2).Date, saved.DatumPlaniranogVracanja.Date);
        }

        [Fact]
        public async Task Zaduzi_SaValidnimDatumomPovratka_KoristitiTajDatum()
        {
            var targetDate = DateTime.Today.AddDays(21);
            var dto = ValidDto();
            dto.DatumPovratka = targetDate;
            Zaduzenje? saved = null;
            _primjerakRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(TestPrimjerak(5, "dostupan"));
            _primjerakRepo.Setup(r => r.HasActiveZaduzenjeAsync(5)).ReturnsAsync(false);
            _zaduzenjeRepo.Setup(r => r.CreateAsync(It.IsAny<Zaduzenje>()))
                .Callback<Zaduzenje>(z => saved = z)
                .ReturnsAsync((Zaduzenje z) => z);
            _primjerakRepo.Setup(r => r.UpdateStatusAsync(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            await _controller.Zaduzi(dto);

            Assert.NotNull(saved);
            Assert.Equal(targetDate.Date, saved!.DatumPlaniranogVracanja.Date);
        }

        [Fact]
        public async Task Zaduzi_SProslinDatumomPovratka_VracaValidacijskuGresku()
        {
            var dto = ValidDto();
            dto.DatumPovratka = DateTime.Today.AddDays(-1);
            _primjerakRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(TestPrimjerak(5, "dostupan"));
            _primjerakRepo.Setup(r => r.HasActiveZaduzenjeAsync(5)).ReturnsAsync(false);
            SetupCreateDropdownMocks();

            var result = await _controller.Zaduzi(dto);

            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal("Create", view.ViewName);
            Assert.False(_controller.ModelState.IsValid);
            _zaduzenjeRepo.Verify(r => r.CreateAsync(It.IsAny<Zaduzenje>()), Times.Never);
        }

        [Fact]
        public async Task Zaduzi_ValidniPodaci_MijenjaSatusPrimjerakaUZaduzen()
        {
            var dto = ValidDto();
            _primjerakRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(TestPrimjerak(5, "dostupan"));
            _primjerakRepo.Setup(r => r.HasActiveZaduzenjeAsync(5)).ReturnsAsync(false);
            _zaduzenjeRepo.Setup(r => r.CreateAsync(It.IsAny<Zaduzenje>()))
                .ReturnsAsync((Zaduzenje z) => z);
            _primjerakRepo.Setup(r => r.UpdateStatusAsync(5, "zadužen")).Returns(Task.CompletedTask);

            await _controller.Zaduzi(dto);

            _primjerakRepo.Verify(r => r.UpdateStatusAsync(5, "zadužen"), Times.Once);
        }

        [Fact]
        public async Task Zaduzi_PrimjerakNedostupan_VracaViewSaGreskom()
        {
            var dto = ValidDto();
            _primjerakRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(TestPrimjerak(5, "zadužen"));
            SetupCreateDropdownMocks();

            var result = await _controller.Zaduzi(dto);

            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal("Create", view.ViewName);
            Assert.False(_controller.ModelState.IsValid);
        }

        [Fact]
        public async Task Zaduzi_AktivnoZaduzenjePrimjeraka_VracaViewSaGreskom()
        {
            var dto = ValidDto();
            _primjerakRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(TestPrimjerak(5, "dostupan"));
            _primjerakRepo.Setup(r => r.HasActiveZaduzenjeAsync(5)).ReturnsAsync(true);
            SetupCreateDropdownMocks();

            var result = await _controller.Zaduzi(dto);

            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal("Create", view.ViewName);
            Assert.False(_controller.ModelState.IsValid);
            _zaduzenjeRepo.Verify(r => r.CreateAsync(It.IsAny<Zaduzenje>()), Times.Never);
        }

        [Fact]
        public async Task Zaduzi_NeispravanModel_VracaViewCreate()
        {
            _controller.ModelState.AddModelError("KorisnikId", "Obavezno.");
            SetupCreateDropdownMocks();

            var result = await _controller.Zaduzi(new ZaduzenjeCreateDto());

            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal("Create", view.ViewName);
            _zaduzenjeRepo.Verify(r => r.CreateAsync(It.IsAny<Zaduzenje>()), Times.Never);
        }

        // ── Vrati ────────────────────────────────────────────────────────────────

        [Fact]
        public async Task Vrati_AktivnoZaduzenje_ZatvaraZaduzenjeIVracaPrimjerak()
        {
            var z = TestZaduzenje(1, "aktivno");
            _zaduzenjeRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(z);
            _zaduzenjeRepo.Setup(r => r.UpdateAsync(It.IsAny<Zaduzenje>())).Returns(Task.CompletedTask);
            _primjerakRepo.Setup(r => r.UpdateStatusAsync(5, "dostupan")).Returns(Task.CompletedTask);

            var result = await _controller.Vrati(1);

            Assert.Equal("zatvoreno", z.Status);
            Assert.NotNull(z.DatumStvarnogVracanja);
            _primjerakRepo.Verify(r => r.UpdateStatusAsync(5, "dostupan"), Times.Once);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirect.ActionName);
        }

        [Fact]
        public async Task Vrati_NepostojeceZaduzenje_VracaNotFound()
        {
            _zaduzenjeRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Zaduzenje?)null);

            var result = await _controller.Vrati(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Vrati_VecZatvoreno_RedirektujeSaGreskom()
        {
            var z = TestZaduzenje(1, "zatvoreno");
            _zaduzenjeRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(z);

            var result = await _controller.Vrati(1);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            _zaduzenjeRepo.Verify(r => r.UpdateAsync(It.IsAny<Zaduzenje>()), Times.Never);
        }

        // ── ZaduzenjeViewModel computed fields ───────────────────────────────────

        [Fact]
        public async Task Moja_ZakasnjeloZaduzenje_JeZakasniloJeTrue()
        {
            SetAuthenticatedUser(10);
            var z = TestZaduzenje();
            z.DatumPlaniranogVracanja = DateTime.UtcNow.AddDays(-2); // prošlo
            _zaduzenjeRepo.Setup(r => r.GetByKorisnikAsync(10))
                .ReturnsAsync(new List<Zaduzenje> { z });

            var result = await _controller.Moja();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ZaduzenjeViewModel>>(view.Model).ToList();
            Assert.True(model[0].JeZakasnilo);
        }

        [Fact]
        public async Task Moja_RokUskoro_RokSeBliziJeTrue()
        {
            SetAuthenticatedUser(10);
            var z = TestZaduzenje();
            z.DatumPlaniranogVracanja = DateTime.UtcNow.AddDays(2); // unutar 3 dana
            _zaduzenjeRepo.Setup(r => r.GetByKorisnikAsync(10))
                .ReturnsAsync(new List<Zaduzenje> { z });

            var result = await _controller.Moja();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ZaduzenjeViewModel>>(view.Model).ToList();
            Assert.False(model[0].JeZakasnilo);
            Assert.True(model[0].RokSeBliži);
        }
    }
}
