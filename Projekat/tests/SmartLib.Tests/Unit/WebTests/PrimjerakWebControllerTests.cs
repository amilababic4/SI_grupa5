using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Web.Controllers;
using Xunit;

namespace SmartLib.Tests.Unit.WebTests
{
    /// <summary>
    /// PB-26: Upravljanje primjercima knjige (Web)
    ///     US-21: Dodavanje novog primjerka postojećoj knjizi
    ///     US-24: Deaktivacija pojedinačnog primjerka
    /// </summary>
    public class PrimjerakWebControllerTests
    {
        private readonly Mock<IPrimjerakRepository> _primjerakMock;
        private readonly Mock<IKnjigaRepository> _knjigaMock;
        private readonly PrimjerakController _controller;

        public PrimjerakWebControllerTests()
        {
            _primjerakMock = new Mock<IPrimjerakRepository>();
            _knjigaMock = new Mock<IKnjigaRepository>();

            _controller = new PrimjerakController(
                _primjerakMock.Object,
                _knjigaMock.Object);

            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            _controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        }

        // Pomoćne metode 

        private static Knjiga TestKnjiga(int id = 1) => new()
        {
            Id = id,
            Naslov = "Test Knjiga",
            Autor = "Test Autor"
        };

        private static Primjerak TestPrimjerak(int id = 1, string status = "dostupan") => new()
        {
            Id = id,
            KnjigaId = 1,
            InventarniBroj = $"INV-1-{id:D3}",
            Status = status
        };

        // US-21: Dodaj GET 

        [Fact]
        public async Task Dodaj_Get_KnjigaPostoji_VracaView()
        {
            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKnjiga());

            var result = await _controller.Dodaj(1);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Dodaj_Get_KnjigaNePostoji_VracaNotFound()
        {
            _knjigaMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Knjiga?)null);

            var result = await _controller.Dodaj(99);

            Assert.IsType<NotFoundResult>(result);
        }

        // US-21: Dodaj POST 

        [Fact]
        public async Task Dodaj_Post_KnjigaNePostoji_RedirektujeSaGreskom()
        {
            // US-21 AC: Primjerak mora biti povezan s postojećom knjigom
            _knjigaMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Knjiga?)null);

            var result = await _controller.Dodaj(99, 1);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Knjiga", redirect.ControllerName);
            Assert.NotNull(_controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task Dodaj_Post_KnjigaNePostoji_NePozivaSeSprema()
        {
            _knjigaMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Knjiga?)null);

            await _controller.Dodaj(99, 1);

            _primjerakMock.Verify(r => r.CreateAsync(It.IsAny<Primjerak>()), Times.Never);
        }

        [Fact]
        public async Task Dodaj_Post_BrojNovihManjiOd1_RedirektujeSaGreskom()
        {
            // Broj primjeraka mora biti između 1 i 50
            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKnjiga());

            var result = await _controller.Dodaj(1, 0);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirect.ActionName);
            Assert.Equal("Knjiga", redirect.ControllerName);
            Assert.NotNull(_controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task Dodaj_Post_BrojNovihVeciOd50_RedirektujeSaGreskom()
        {
            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKnjiga());

            var result = await _controller.Dodaj(1, 51);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirect.ActionName);
            Assert.NotNull(_controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task Dodaj_Post_ValidanRequest_SpremaIRedirektuje()
        {
            // US-21 AC: Nakon dodavanja, primjerak je evidentiran
            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKnjiga());
            _primjerakMock.Setup(r => r.GetByKnjigaAsync(1))
                          .ReturnsAsync(new List<Primjerak>());
            _primjerakMock.Setup(r => r.CreateAsync(It.IsAny<Primjerak>()))
                          .ReturnsAsync((Primjerak p) => p);

            var result = await _controller.Dodaj(1, 1);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirect.ActionName);
            Assert.Equal("Knjiga", redirect.ControllerName);
        }

        [Fact]
        public async Task Dodaj_Post_ValidanRequest_PrikazujePorukuUspjeha()
        {
            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKnjiga());
            _primjerakMock.Setup(r => r.GetByKnjigaAsync(1))
                          .ReturnsAsync(new List<Primjerak>());
            _primjerakMock.Setup(r => r.CreateAsync(It.IsAny<Primjerak>()))
                          .ReturnsAsync((Primjerak p) => p);

            await _controller.Dodaj(1, 2);

            Assert.NotNull(_controller.TempData["SuccessMessage"]);
        }

        [Fact]
        public async Task Dodaj_Post_BrojNovih3_PozivaSeSprema3Puta()
        {
            // US-21 AC: Svaki primjerak mora biti zaseban zapis u sistemu
            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKnjiga());
            _primjerakMock.Setup(r => r.GetByKnjigaAsync(1))
                          .ReturnsAsync(new List<Primjerak>());
            _primjerakMock.Setup(r => r.CreateAsync(It.IsAny<Primjerak>()))
                          .ReturnsAsync((Primjerak p) => p);

            await _controller.Dodaj(1, 3);

            _primjerakMock.Verify(r => r.CreateAsync(It.IsAny<Primjerak>()), Times.Exactly(3));
        }

        [Fact]
        public async Task Dodaj_Post_NoviPrimjerak_StatusJeDostupan()
        {
            // US-23 AC: Novi primjerak mora početi sa statusom "dostupan"
            var sacuvani = new List<Primjerak>();

            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKnjiga());
            _primjerakMock.Setup(r => r.GetByKnjigaAsync(1))
                          .ReturnsAsync(new List<Primjerak>());
            _primjerakMock.Setup(r => r.CreateAsync(It.IsAny<Primjerak>()))
                          .Callback<Primjerak>(p => sacuvani.Add(p))
                          .ReturnsAsync((Primjerak p) => p);

            await _controller.Dodaj(1, 2);

            Assert.Equal(2, sacuvani.Count);
            Assert.All(sacuvani, p => Assert.Equal("dostupan", p.Status));
        }

        [Fact]
        public async Task Dodaj_Post_PostojeciPrimjerci_RedniBrojSeNastavlja()
        {
            // Ako knjiga već ima 2 primjerka, sljedeći dobija redni broj 3
            var sacuvani = new List<Primjerak>();
            var postojeci = new List<Primjerak> { TestPrimjerak(1), TestPrimjerak(2) };

            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKnjiga());
            _primjerakMock.Setup(r => r.GetByKnjigaAsync(1)).ReturnsAsync(postojeci);
            _primjerakMock.Setup(r => r.CreateAsync(It.IsAny<Primjerak>()))
                          .Callback<Primjerak>(p => sacuvani.Add(p))
                          .ReturnsAsync((Primjerak p) => p);

            await _controller.Dodaj(1, 1);

            Assert.Single(sacuvani);
            Assert.Equal("INV-1-003", sacuvani[0].InventarniBroj);
        }

        // US-24: Deaktiviraj 

        [Fact]
        public async Task Deaktiviraj_PrimjerakNePostoji_RedirektujeSaGreskom()
        {
            _primjerakMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Primjerak?)null);

            var result = await _controller.Deaktiviraj(999);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Knjiga", redirect.ControllerName);
            Assert.NotNull(_controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task Deaktiviraj_ImaAktivnoZaduzenje_RedirektujeSaGreskom()
        {
            // US-24 AC: Sistem ne dozvoljava deaktivaciju aktivno zaduženog primjerka
            var primjerak = TestPrimjerak(1, "dostupan");
            _primjerakMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(primjerak);
            _primjerakMock.Setup(r => r.HasActiveZaduzenjeAsync(1)).ReturnsAsync(true);

            var result = await _controller.Deaktiviraj(1);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirect.ActionName);
            Assert.Equal("Knjiga", redirect.ControllerName);
            Assert.NotNull(_controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task Deaktiviraj_ImaAktivnoZaduzenje_NePozivaseDeactivate()
        {
            // US-24 AC: DeactivateAsync se ne smije pozvati ako postoji aktivno zaduženje
            var primjerak = TestPrimjerak(1, "dostupan");
            _primjerakMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(primjerak);
            _primjerakMock.Setup(r => r.HasActiveZaduzenjeAsync(1)).ReturnsAsync(true);

            await _controller.Deaktiviraj(1);

            _primjerakMock.Verify(r => r.DeactivateAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Deaktiviraj_VecDeaktiviran_RedirektujeSaGreskom()
        {
            // Web kontroler prvo provjerava zaduženje, pa tek onda status
            var primjerak = TestPrimjerak(1, "deaktiviran");
            _primjerakMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(primjerak);
            _primjerakMock.Setup(r => r.HasActiveZaduzenjeAsync(1)).ReturnsAsync(false);

            var result = await _controller.Deaktiviraj(1);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirect.ActionName);
            Assert.NotNull(_controller.TempData["ErrorMessage"]);
            _primjerakMock.Verify(r => r.DeactivateAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Deaktiviraj_Uspjesno_RedirektujeSaPorukомUspjeha()
        {
            var primjerak = TestPrimjerak(1, "dostupan");
            _primjerakMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(primjerak);
            _primjerakMock.Setup(r => r.HasActiveZaduzenjeAsync(1)).ReturnsAsync(false);
            _primjerakMock.Setup(r => r.DeactivateAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.Deaktiviraj(1);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirect.ActionName);
            Assert.Equal("Knjiga", redirect.ControllerName);
            Assert.NotNull(_controller.TempData["SuccessMessage"]);
        }

        [Fact]
        public async Task Deaktiviraj_Uspjesno_PozivaseDeactivate()
        {
            var primjerak = TestPrimjerak(1, "dostupan");
            _primjerakMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(primjerak);
            _primjerakMock.Setup(r => r.HasActiveZaduzenjeAsync(1)).ReturnsAsync(false);
            _primjerakMock.Setup(r => r.DeactivateAsync(1)).Returns(Task.CompletedTask);

            await _controller.Deaktiviraj(1);

            _primjerakMock.Verify(r => r.DeactivateAsync(1), Times.Once);
        }
    }
}