using Microsoft.AspNetCore.Mvc;
using Moq;
using SmartLib.API.Controllers;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using Xunit;

namespace SmartLib.Tests.Unit.APITests
{
    /// <summary>
    /// PB-26: Upravljanje primjercima knjige (API):
    ///     US-21: Dodavanje novog primjerka postojećoj knjizi
    ///     US-22: Pregled primjeraka knjige
    ///     US-23: Pregled pojedinačnih statusa primjeraka
    ///     US-24: Deaktivacija pojedinačnog primjerka
    /// </summary>
    public class PrimjerakApiControllerTests
    {
        private readonly Mock<IPrimjerakRepository> _primjerakMock;
        private readonly Mock<IKnjigaRepository> _knjigaMock;
        private readonly PrimjerakController _controller;

        public PrimjerakApiControllerTests()
        {
            _primjerakMock = new Mock<IPrimjerakRepository>();
            _knjigaMock = new Mock<IKnjigaRepository>();

            _controller = new PrimjerakController(
                _primjerakMock.Object,
                _knjigaMock.Object);
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
            Status = status,
            DatumNabave = DateTime.UtcNow
        };

        // US-22 + US-23: GetByKnjiga

        [Fact]
        public async Task GetByKnjiga_KnjigaPostoji_VracaOkSaListomPrimjeraka()
        {
            // US22 AC: Kada korisnik otvori detalje knjige, sistem prikazuje listu primjeraka
            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKnjiga());
            _primjerakMock.Setup(r => r.GetByKnjigaAsync(1))
                          .ReturnsAsync(new List<Primjerak>
                          {
                              TestPrimjerak(1, "dostupan"),
                              TestPrimjerak(2, "zadužen")
                          });

            var result = await _controller.GetByKnjiga(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetByKnjiga_KnjigaNePostoji_VracaNotFound()
        {
            _knjigaMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Knjiga?)null);

            var result = await _controller.GetByKnjiga(99);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetByKnjiga_KnjigaNemaaPrimjeraka_VracaOkSaPraznomListom()
        {
            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKnjiga());
            _primjerakMock.Setup(r => r.GetByKnjigaAsync(1))
                          .ReturnsAsync(new List<Primjerak>());

            var result = await _controller.GetByKnjiga(1);

            Assert.IsType<OkObjectResult>(result);
        }

        // US-22 + US-23: GetById 

        [Fact]
        public async Task GetById_PrimjerakPostoji_VracaOkIObjekt()
        {
            _primjerakMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestPrimjerak());

            var result = await _controller.GetById(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_PrimjerakNePostoji_VracaNotFound()
        {
            _primjerakMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Primjerak?)null);

            var result = await _controller.GetById(99);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        // US-21: Create

        [Fact]
        public async Task Create_KnjigaNePostoji_VracaBadRequest()
        {
            // US-21 AC: Sistem ne smije dozvoliti kreiranje primjeraka bez povezane knjige
            _knjigaMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Knjiga?)null);

            var result = await _controller.Create(new PrimjerakCreateRequest
            {
                KnjigaId = 999               
            });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Create_KnjigaNePostoji_NePozivaSeSprema()
        {
            // US-21: CreateAsync se ne smije pozvati ako knjiga ne postoji
            _knjigaMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Knjiga?)null);

            await _controller.Create(new PrimjerakCreateRequest { KnjigaId = 999, BrojNovih = 1 });

            _primjerakMock.Verify(r => r.CreateAsync(It.IsAny<Primjerak>()), Times.Never);
        }

        [Fact]
        public async Task Create_BrojNovihManjiOd1_VracaBadRequest()
        {
            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKnjiga());

            var result = await _controller.Create(new PrimjerakCreateRequest
            {
                KnjigaId = 1,
                BrojNovih = 0
            });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Create_BrojNovihVeciOd50_VracaBadRequest()
        {
            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKnjiga());

            var result = await _controller.Create(new PrimjerakCreateRequest
            {
                KnjigaId = 1,
                BrojNovih = 51
            });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Create_ValidanRequest_VracaCreatedAtAction()
        {
            // US-21 AC: Nakon dodavanja, primjerak je vidljiv u listi
            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKnjiga());
            _primjerakMock.Setup(r => r.GetByKnjigaAsync(1))
                          .ReturnsAsync(new List<Primjerak>());
            _primjerakMock.Setup(r => r.CreateAsync(It.IsAny<Primjerak>()))
                          .ReturnsAsync((Primjerak p) => { p.Id = 10; return p; });

            var result = await _controller.Create(new PrimjerakCreateRequest
            {
                KnjigaId = 1         
            });

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetByKnjiga), created.ActionName);
        }

        [Fact]
        public async Task Create_BrojNovih3_PozivaSeSprema3Puta()
        {
            // US-21 AC: Svaki primjerak mora biti zaseban zapis u sistemu
            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKnjiga());
            _primjerakMock.Setup(r => r.GetByKnjigaAsync(1))
                          .ReturnsAsync(new List<Primjerak>());
            _primjerakMock.Setup(r => r.CreateAsync(It.IsAny<Primjerak>()))
                          .ReturnsAsync((Primjerak p) => p);

            await _controller.Create(new PrimjerakCreateRequest { KnjigaId = 1, BrojNovih = 3 });

            _primjerakMock.Verify(r => r.CreateAsync(It.IsAny<Primjerak>()), Times.Exactly(3));
        }

        [Fact]
        public async Task Create_NoviPrimjerak_StatusJeDostupanByDefault()
        {
            // US-23 AC: Svaki primjerak mora imati definisan status - novi su uvijek "dostupan"
            var sacuvani = new List<Primjerak>();

            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKnjiga());
            _primjerakMock.Setup(r => r.GetByKnjigaAsync(1))
                          .ReturnsAsync(new List<Primjerak>());
            _primjerakMock.Setup(r => r.CreateAsync(It.IsAny<Primjerak>()))
                          .Callback<Primjerak>(p => sacuvani.Add(p))
                          .ReturnsAsync((Primjerak p) => p);

            await _controller.Create(new PrimjerakCreateRequest { KnjigaId = 1, BrojNovih = 2 });

            Assert.Equal(2, sacuvani.Count);
            Assert.All(sacuvani, p => Assert.Equal("dostupan", p.Status));
        }

        [Fact]
        public async Task Create_NoviPrimjerak_InventarniBrojSadrziKnjigaId()
        {
            // Inventarni broj se automatski generiše u formatu INV-{knjigaId}-{redni}
            var sacuvani = new List<Primjerak>();

            _knjigaMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(TestKnjiga(5));
            _primjerakMock.Setup(r => r.GetByKnjigaAsync(5))
                          .ReturnsAsync(new List<Primjerak>());
            _primjerakMock.Setup(r => r.CreateAsync(It.IsAny<Primjerak>()))
                          .Callback<Primjerak>(p => sacuvani.Add(p))
                          .ReturnsAsync((Primjerak p) => p);

            await _controller.Create(new PrimjerakCreateRequest { KnjigaId = 5 });

            Assert.Single(sacuvani);
            Assert.StartsWith("INV-5-", sacuvani[0].InventarniBroj);
        }

        [Fact]
        public async Task Create_PostojeciPrimjerci_RedniBrojSeNastavlja()
        {
            // Ako knjiga već ima 2 primjerka, sljedeći počinje od rednog broja 3
            var sacuvani = new List<Primjerak>();
            var postojeci = new List<Primjerak>
            {
                TestPrimjerak(1), TestPrimjerak(2)
            };

            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKnjiga());
            _primjerakMock.Setup(r => r.GetByKnjigaAsync(1)).ReturnsAsync(postojeci);
            _primjerakMock.Setup(r => r.CreateAsync(It.IsAny<Primjerak>()))
                          .Callback<Primjerak>(p => sacuvani.Add(p))
                          .ReturnsAsync((Primjerak p) => p);

            await _controller.Create(new PrimjerakCreateRequest { KnjigaId = 1, BrojNovih = 1 });

            Assert.Single(sacuvani);
            Assert.Equal("INV-1-003", sacuvani[0].InventarniBroj);
        }

        // US-24: Deaktiviraj

        [Fact]
        public async Task Deaktiviraj_PrimjerakNePostoji_VracaNotFound()
        {
            _primjerakMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Primjerak?)null);

            var result = await _controller.Deaktiviraj(999);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Deaktiviraj_VecDeaktiviran_VracaConflict()
        {
            // US-24: Ne smijemo dvaput deaktivirati isti primjerak
            var primjerak = TestPrimjerak(1, "deaktiviran");
            _primjerakMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(primjerak);

            var result = await _controller.Deaktiviraj(1);

            var conflict = Assert.IsType<ConflictObjectResult>(result);
            Assert.Contains("već deaktiviran", conflict.Value!.ToString());
        }

        [Fact]
        public async Task Deaktiviraj_ImaAktivnoZaduzenje_VracaConflict()
        {
            // US-24 AC: Sistem ne dozvoljava deaktivaciju primjerka koji je aktivno zadužen
            var primjerak = TestPrimjerak(1, "dostupan");
            _primjerakMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(primjerak);
            _primjerakMock.Setup(r => r.HasActiveZaduzenjeAsync(1)).ReturnsAsync(true);

            var result = await _controller.Deaktiviraj(1);

            var conflict = Assert.IsType<ConflictObjectResult>(result);
            Assert.Contains("aktivo zadužen", conflict.Value!.ToString());
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
        public async Task Deaktiviraj_Uspjesno_VracaOk()
        {
            var primjerak = TestPrimjerak(1, "dostupan");
            _primjerakMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(primjerak);
            _primjerakMock.Setup(r => r.HasActiveZaduzenjeAsync(1)).ReturnsAsync(false);
            _primjerakMock.Setup(r => r.DeactivateAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.Deaktiviraj(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("uspješno deaktiviran", ok.Value!.ToString());
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