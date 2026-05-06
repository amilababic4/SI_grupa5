using Microsoft.AspNetCore.Mvc;
using Moq;
using SmartLib.API.Controllers;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using Xunit;

namespace SmartLib.Tests.Unit.APITests
{
    public class KnjigaApiControllerTests
    {
        private readonly Mock<IKnjigaRepository> _knjigaMock;
        private readonly Mock<IPrimjerakRepository> _primjerakMock;
        private readonly Mock<IKategorijaRepository> _kategorijaMock;
        private readonly KnjigaController _controller;

        public KnjigaApiControllerTests()
        {
            _knjigaMock = new Mock<IKnjigaRepository>();
            _primjerakMock = new Mock<IPrimjerakRepository>();
            _kategorijaMock = new Mock<IKategorijaRepository>();

            _controller = new KnjigaController(
                _knjigaMock.Object,
                _primjerakMock.Object,
                _kategorijaMock.Object);
        }

        // Pomoćne metode za testne podatke
        private static Knjiga TestKnjiga(int id = 1) => new()
        {
            Id = id,
            Naslov = "Test Knjiga",
            Autor = "Autor",
            Isbn = "1234567890",
            Primjerci = new List<Primjerak>()
        };

        [Fact]
        public async Task GetById_KnjigaPostoji_VracaOkIObjekt()
        {

            var knjiga = TestKnjiga(10);
            _knjigaMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(knjiga);

            var result = await _controller.GetById(10);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<KnjigaDto>(okResult.Value);
            Assert.Equal(10, dto.Id);
        }

        [Fact]
        public async Task GetById_KnjigaNePostoji_VracaNotFound()
        {
            _knjigaMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Knjiga?)null);

            var result = await _controller.GetById(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ValidnaKnjiga_Vraca201Created()
        {
            var createDto = new KnjigaCreateDto
            {
                Naslov = "Nova",
                Autor = "Autor",
                Isbn = "1234567890",
                KategorijaId = 1,
                BrojPrimjeraka = 1
            };

            _knjigaMock.Setup(r => r.GetByIsbnAsync(It.IsAny<string>())).ReturnsAsync((Knjiga?)null);
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Kategorija { Id = 1 });
            _knjigaMock.Setup(r => r.CreateAsync(It.IsAny<Knjiga>())).ReturnsAsync(TestKnjiga(5));
            _knjigaMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(TestKnjiga(5));

            var result = await _controller.Create(createDto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(201, createdResult.StatusCode);
            Assert.Equal("GetById", createdResult.ActionName);
        }

        [Fact]
        public async Task Create_NeispravanIsbn_VracaBadRequest()
        {
            var createDto = new KnjigaCreateDto { Isbn = "short" }; // Nevalidan ISBN

            var result = await _controller.Create(createDto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("ISBN nije u ispravnom formatu.", badRequest.Value);
        }

        [Fact]
        public async Task Delete_KnjigaImaAktivnaZaduzenja_VracaBadRequest()
        {
            // Simuliramo poslovno pravilo: HasActiveLoansAsync vraća true
            _knjigaMock.Setup(r => r.HasActiveLoansAsync(1)).ReturnsAsync(true);

            var result = await _controller.Delete(1);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("aktivna zaduženja", badRequest.Value.ToString());

            _knjigaMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Delete_Uspjesno_VracaNoContent()
        {
            _knjigaMock.Setup(r => r.HasActiveLoansAsync(1)).ReturnsAsync(false);
            _knjigaMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Create_DupliIsbn_VracaConflict()
        {
            var dto = new KnjigaCreateDto { Isbn = "1234567890", Naslov = "Test" };
            _knjigaMock.Setup(r => r.GetByIsbnAsync(It.IsAny<string>())).ReturnsAsync(new Knjiga());

            var result = await _controller.Create(dto);

            Assert.IsType<ConflictObjectResult>(result.Result);
        }

        [Fact]
        public async Task Update_PogresanId_VracaBadRequest()
        {
            var result = await _controller.Update(1, new KnjigaEditDto { Id = 2 });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetAll_VracaOkSaListomKnjiga()
        {
            _knjigaMock.Setup(r => r.GetPagedAsync(null, null, 1, 10))
                       .ReturnsAsync((new List<Knjiga> { TestKnjiga() }, 1));

            var result = await _controller.GetAll(null, null, 1, 10);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task GetAll_PaginacijaMetadata_IspravnaVrijednost()
        {
            var knjige = Enumerable.Range(1, 10).Select(i => TestKnjiga(i)).ToList();
            _knjigaMock.Setup(r => r.GetPagedAsync(null, null, 2, 10))
                       .ReturnsAsync((knjige, 25));

            var result = await _controller.GetAll(null, null, 2, 10);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var json = ok.Value!.ToString();
            Assert.Contains("25", json); // UkupnoStavki
        }

        [Fact]
        public async Task GetAll_FiltriranjePoPasvaNaslov_ProslijedjeParametarRepozitoriju()
        {
            _knjigaMock.Setup(r => r.GetPagedAsync("Harry", null, 1, 10))
                       .ReturnsAsync((new List<Knjiga>(), 0));

            var result = await _controller.GetAll("Harry", null, 1, 10);

            _knjigaMock.Verify(r => r.GetPagedAsync("Harry", null, 1, 10), Times.Once);
        }

        // kategorija ne postoji → 400
        [Fact]
        public async Task Create_NepostojecaKategorija_VracaBadRequest()
        {
            var dto = new KnjigaCreateDto
            {
                Naslov = "Test",
                Autor = "Autor",
                Isbn = "1234567890",
                KategorijaId = 99
            };

            _knjigaMock.Setup(r => r.GetByIsbnAsync(It.IsAny<string>())).ReturnsAsync((Knjiga?)null);
            _kategorijaMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Kategorija?)null);

            var result = await _controller.Create(dto);

            var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Odabrana kategorija nije validna.", bad.Value);
        }

        // BrojPrimjeraka=3 → CreateAsync za primjerak se poziva 3 puta
        [Fact]
        public async Task Create_BrojPrimjeraka3_KreiraTacno3Primjerka()
        {
            var dto = new KnjigaCreateDto
            {
                Naslov = "Test",
                Autor = "Autor",
                Isbn = "1234567890",
                KategorijaId = 1,
                BrojPrimjeraka = 3
            };

            _knjigaMock.Setup(r => r.GetByIsbnAsync(It.IsAny<string>())).ReturnsAsync((Knjiga?)null);
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Kategorija { Id = 1 });
            _knjigaMock.Setup(r => r.CreateAsync(It.IsAny<Knjiga>())).ReturnsAsync(TestKnjiga(5));
            _knjigaMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(TestKnjiga(5));
            _primjerakMock.Setup(r => r.CreateAsync(It.IsAny<Primjerak>()))
                          .ReturnsAsync((Primjerak p) => p);

            await _controller.Create(dto);

            _primjerakMock.Verify(r => r.CreateAsync(It.IsAny<Primjerak>()), Times.Exactly(3));
        }

        // novi primjerci imaju status "dostupan" i ispravan inventarni broj
        [Fact]
        public async Task Create_NoviPrimjerci_StatusDostupanIIspravanInventarniBroj()
        {
            var sacuvani = new List<Primjerak>();
            var dto = new KnjigaCreateDto
            {
                Naslov = "Test",
                Autor = "Autor",
                Isbn = "1234567890",
                KategorijaId = 1,
                BrojPrimjeraka = 2
            };

            _knjigaMock.Setup(r => r.GetByIsbnAsync(It.IsAny<string>())).ReturnsAsync((Knjiga?)null);
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Kategorija { Id = 1 });
            _knjigaMock.Setup(r => r.CreateAsync(It.IsAny<Knjiga>())).ReturnsAsync(TestKnjiga(5));
            _knjigaMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(TestKnjiga(5));
            _primjerakMock.Setup(r => r.CreateAsync(It.IsAny<Primjerak>()))
                          .Callback<Primjerak>(p => sacuvani.Add(p))
                          .ReturnsAsync((Primjerak p) => p);

            await _controller.Create(dto);

            Assert.Equal(2, sacuvani.Count);
            Assert.All(sacuvani, p => Assert.Equal("dostupan", p.Status));
            Assert.Equal("INV-5-001", sacuvani[0].InventarniBroj);
            Assert.Equal("INV-5-002", sacuvani[1].InventarniBroj);
        }

        // ISBN sa crticama se normalizuje
        [Fact]
        public async Task Create_IsbnSaCrticama_NormalizujeSeIsprePohrane()
        {
            Knjiga? sacuvana = null;
            var dto = new KnjigaCreateDto
            {
                Naslov = "Test",
                Autor = "Autor",
                Isbn = "978-0-451-52493-5",
                KategorijaId = 1,
                BrojPrimjeraka = 0
            };

            _knjigaMock.Setup(r => r.GetByIsbnAsync("9780451524935")).ReturnsAsync((Knjiga?)null);
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Kategorija { Id = 1 });
            _knjigaMock.Setup(r => r.CreateAsync(It.IsAny<Knjiga>()))
                       .Callback<Knjiga>(k => sacuvana = k)
                       .ReturnsAsync(TestKnjiga(1));
            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKnjiga(1));

            await _controller.Create(dto);

            Assert.NotNull(sacuvana);
            Assert.Equal("9780451524935", sacuvana!.Isbn);
        }

        // knjiga ne postoji → 404
        [Fact]
        public async Task Update_KnjigaNePostoji_VracaNotFound()
        {
            _knjigaMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Knjiga?)null);

            var result = await _controller.Update(99, new KnjigaEditDto
            {
                Id = 99,
                Naslov = "X",
                Autor = "Y",
                KategorijaId = 1
            });

            Assert.IsType<NotFoundResult>(result);
        }

        // kategorija ne postoji → 400
        [Fact]
        public async Task Update_NepostojecaKategorija_VracaBadRequest()
        {
            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKnjiga());
            _kategorijaMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Kategorija?)null);

            var result = await _controller.Update(1, new KnjigaEditDto
            {
                Id = 1,
                Naslov = "X",
                Autor = "Y",
                KategorijaId = 99
            });

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Odabrana kategorija nije validna.", bad.Value);
        }

        // uspješan update → 204 NoContent, podaci ažurirani
        [Fact]
        public async Task Update_Uspjesno_VracaNoContentIPodaciAzurirani()
        {
            var knjiga = TestKnjiga();

            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(knjiga);
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Kategorija { Id = 1 });
            _knjigaMock.Setup(r => r.UpdateAsync(It.IsAny<Knjiga>())).Returns(Task.CompletedTask);

            var result = await _controller.Update(1, new KnjigaEditDto
            {
                Id = 1,
                Naslov = "Novi naslov",
                Autor = "Novi autor",
                KategorijaId = 1
            });

            Assert.IsType<NoContentResult>(result);
            Assert.Equal("Novi naslov", knjiga.Naslov);
            Assert.Equal("Novi autor", knjiga.Autor);
            _knjigaMock.Verify(r => r.UpdateAsync(It.IsAny<Knjiga>()), Times.Once);
        }

        // DeleteAsync vraća false → 404
        [Fact]
        public async Task Delete_KnjigaNePostoji_VracaNotFound()
        {
            _knjigaMock.Setup(r => r.HasActiveLoansAsync(99)).ReturnsAsync(false);
            _knjigaMock.Setup(r => r.DeleteAsync(99)).ReturnsAsync(false);

            var result = await _controller.Delete(99);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Create_SaIzdavacem_SpremaIzdavacaTrimovan()
        {
            Knjiga? sacuvana = null;
            var dto = new KnjigaCreateDto
            {
                Naslov = "Test",
                Autor = "Autor",
                Isbn = "1234567890",
                KategorijaId = 1,
                BrojPrimjeraka = 0,
                Izdavac = "  Pearson  "  // sa razmacima → treba biti trimovan
            };

            _knjigaMock.Setup(r => r.GetByIsbnAsync(It.IsAny<string>())).ReturnsAsync((Knjiga?)null);
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Kategorija { Id = 1 });
            _knjigaMock.Setup(r => r.CreateAsync(It.IsAny<Knjiga>()))
                       .Callback<Knjiga>(k => sacuvana = k)
                       .ReturnsAsync(TestKnjiga(1));
            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKnjiga(1));

            await _controller.Create(dto);

            Assert.NotNull(sacuvana);
            Assert.Equal("Pearson", sacuvana!.Izdavac);
        }

        [Fact]
        public async Task Update_SaIzdavacem_AzuriraIzdavacaTrimovan()
        {
            var knjiga = TestKnjiga();

            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(knjiga);
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Kategorija { Id = 1 });
            _knjigaMock.Setup(r => r.UpdateAsync(It.IsAny<Knjiga>())).Returns(Task.CompletedTask);

            await _controller.Update(1, new KnjigaEditDto
            {
                Id = 1,
                Naslov = "Naslov",
                Autor = "Autor",
                KategorijaId = 1,
                Izdavac = "  O'Reilly  "  // sa razmacima → treba biti trimovan
            });

            Assert.Equal("O'Reilly", knjiga.Izdavac);
        }

        [Fact]
        public async Task Create_Isbn10SaNevazanimZadnjimZnakom_VracaBadRequest()
        {
            var dto = new KnjigaCreateDto { Isbn = "080442957Z" }; // Z nije digit ni X

            var result = await _controller.Create(dto);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Create_IsbnPogresneDuzine_VracaBadRequest()
        {
            var dto = new KnjigaCreateDto { Isbn = "12345678901" }; // 11 znakova

            var result = await _controller.Create(dto);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetById_KnjigaBezKategorije_KategorijaJeNull()
        {
            var knjiga = TestKnjiga();
            knjiga.Kategorija = null;
            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(knjiga);

            var result = await _controller.GetById(1);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<KnjigaDto>(ok.Value);
            Assert.Null(dto.Kategorija);
        }

        [Fact]
        public async Task GetAll_NemaKnjiga_VracaPrazanRezultat()
        {
            _knjigaMock.Setup(r => r.GetPagedAsync(null, null, 1, 10))
                       .ReturnsAsync((new List<Knjiga>(), 0));

            var result = await _controller.GetAll(null, null, 1, 10);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task Create_IsbnSaXNaKraju_Validan()
        {
            var dto = new KnjigaCreateDto
            {
                Naslov = "Test",
                Autor = "Autor",
                Isbn = "080442957X",
                KategorijaId = 1,
                BrojPrimjeraka = 0
            };

            _knjigaMock.Setup(r => r.GetByIsbnAsync("080442957X")).ReturnsAsync((Knjiga?)null);
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Kategorija { Id = 1 });
            _knjigaMock.Setup(r => r.CreateAsync(It.IsAny<Knjiga>())).ReturnsAsync(TestKnjiga(1));
            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKnjiga(1));

            var result = await _controller.Create(dto);

            Assert.IsType<CreatedAtActionResult>(result.Result);
        }
    }
}