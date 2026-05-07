using Microsoft.AspNetCore.Mvc;
using Moq;
using SmartLib.API.Controllers;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using Xunit;

namespace SmartLib.Tests.Unit.APITests
{
    /// <summary>
    /// PB-25: Upravljanje kategorijama knjiga (API)
    ///     US-30: Dodavanje nove kategorije
    ///     US-31: Prikaz liste postojećih kategorija
    ///     US-32: Zabrana brisanja kategorije koja je u upotrebi
    ///     US-33: Uređivanje pojedinačnih kategorija
    ///     US-34: Brisanje kategorije iz sistema
    /// </summary>
    public class KategorijaApiControllerTests
    {
        private readonly Mock<IKategorijaRepository> _kategorijaMock;
        private readonly KategorijaController _controller;

        public KategorijaApiControllerTests()
        {
            _kategorijaMock = new Mock<IKategorijaRepository>();
            _controller = new KategorijaController(_kategorijaMock.Object);
        }

        // Pomoćne metode 

        private static Kategorija TestKategorija(int id = 1, string naziv = "Beletristika") => new()
        {
            Id = id,
            Naziv = naziv,
            Opis = "Književna proza",
            Knjige = new List<Knjiga>()
        };

        private static KategorijaRequest ValidanRequest(string naziv = "Nova kategorija") => new()
        {
            Naziv = naziv,
            Opis = "Opis nove kategorije"
        };

        // US-31: GetAll 

        [Fact]
        public async Task GetAll_VracaOkSaListomKategorija()
        {
            _kategorijaMock.Setup(r => r.GetAllAsync())
                           .ReturnsAsync(new List<Kategorija>
                           {
                               TestKategorija(1, "Beletristika"),
                               TestKategorija(2, "Nauka")
                           });

            var result = await _controller.GetAll();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetAll_NemaKategorija_VracaOkSaPraznomListom()
        {
            _kategorijaMock.Setup(r => r.GetAllAsync())
                           .ReturnsAsync(new List<Kategorija>());

            var result = await _controller.GetAll();

            Assert.IsType<OkObjectResult>(result);
        }

        // US-31: GetById 

        [Fact]
        public async Task GetById_KategorijaPostoji_VracaOkIObjekt()
        {
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKategorija());

            var result = await _controller.GetById(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_KategorijaNePostoji_VracaNotFound()
        {
            _kategorijaMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Kategorija?)null);

            var result = await _controller.GetById(99);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        // US-30: Create 

        [Fact]
        public async Task Create_ValidanRequest_Vraca201Created()
        {
            _kategorijaMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Kategorija>());
            _kategorijaMock.Setup(r => r.CreateAsync(It.IsAny<Kategorija>()))
                           .ReturnsAsync((Kategorija k) => { k.Id = 5; return k; });

            var result = await _controller.Create(ValidanRequest("Historija"));

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, created.StatusCode);
            Assert.Equal(nameof(_controller.GetById), created.ActionName);
        }

        [Fact]
        public async Task Create_NazivVecPostoji_VracaConflict()
        {
            // US-30 AC: Kada kategorija već postoji, sistem prikazuje poruku o grešci
            _kategorijaMock.Setup(r => r.GetAllAsync())
                           .ReturnsAsync(new List<Kategorija> { TestKategorija(1, "Beletristika") });

            var result = await _controller.Create(new KategorijaRequest { Naziv = "Beletristika" });

            Assert.IsType<ConflictObjectResult>(result);
        }

        [Fact]
        public async Task Create_NazivVecPostoji_NePozivaSeSprema()
        {
            // US-30: CreateAsync se ne smije pozvati pri duplikatu
            _kategorijaMock.Setup(r => r.GetAllAsync())
                           .ReturnsAsync(new List<Kategorija> { TestKategorija(1, "Beletristika") });

            await _controller.Create(new KategorijaRequest { Naziv = "Beletristika" });

            _kategorijaMock.Verify(r => r.CreateAsync(It.IsAny<Kategorija>()), Times.Never);
        }

        [Fact]
        public async Task Create_PrazanNaziv_VracaBadRequest()
        {
            // US-30 AC: Naziv je obavezno polje
            var result = await _controller.Create(new KategorijaRequest { Naziv = "   " });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Create_ValidanRequest_SpremaSaIspravnimPodacima()
        {
            // US-30: Naziv se trimuje prije pohrane
            Kategorija? sacuvana = null;

            _kategorijaMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Kategorija>());
            _kategorijaMock.Setup(r => r.CreateAsync(It.IsAny<Kategorija>()))
                           .Callback<Kategorija>(k => sacuvana = k)
                           .ReturnsAsync((Kategorija k) => { k.Id = 3; return k; });

            await _controller.Create(new KategorijaRequest { Naziv = "  Nauka  ", Opis = "Naučna literatura" });

            Assert.NotNull(sacuvana);
            Assert.Equal("Nauka", sacuvana!.Naziv);
            Assert.Equal("Naučna literatura", sacuvana.Opis);
        }

        [Fact]
        public async Task Create_NazivCaseInsensitive_VracaConflict()
        {
            // Provjera duplikata je case-insensitive
            _kategorijaMock.Setup(r => r.GetAllAsync())
                           .ReturnsAsync(new List<Kategorija> { TestKategorija(1, "Beletristika") });

            var result = await _controller.Create(new KategorijaRequest { Naziv = "beletristika" });

            Assert.IsType<ConflictObjectResult>(result);
        }

        [Fact]
        public async Task Create_NeispravanModel_VracaBadRequest()
        {
            _controller.ModelState.AddModelError("Naziv", "Naziv je obavezan.");

            var result = await _controller.Create(new KategorijaRequest());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // US-33: Update 

        [Fact]
        public async Task Update_PostojecaKategorija_AzuriraPodatkeIVracaOk()
        {
            var kategorija = TestKategorija();

            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(kategorija);
            // GetAllAsync vraća istu kategoriju — jedini zapis, nema konflikta
            _kategorijaMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Kategorija> { kategorija });
            _kategorijaMock.Setup(r => r.UpdateAsync(It.IsAny<Kategorija>())).Returns(Task.CompletedTask);

            var result = await _controller.Update(1, new KategorijaRequest
            {
                Naziv = "Izmijenjeni naziv",
                Opis = "Novi opis"
            });

            Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Izmijenjeni naziv", kategorija.Naziv);
            Assert.Equal("Novi opis", kategorija.Opis);
        }

        [Fact]
        public async Task Update_NepostojecaKategorija_VracaNotFound()
        {
            _kategorijaMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Kategorija?)null);

            var result = await _controller.Update(999, new KategorijaRequest { Naziv = "X" });

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Update_NazivVecPostojiKodDrugeKategorije_VracaConflict()
        {
            // US-33 AC: Kada naziv već postoji (kod druge kategorije), sistem odbija izmjenu
            var kategorija1 = TestKategorija(1, "Beletristika");
            var kategorija2 = TestKategorija(2, "Historija");

            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(kategorija1);
            _kategorijaMock.Setup(r => r.GetAllAsync())
                           .ReturnsAsync(new List<Kategorija> { kategorija1, kategorija2 });

            var result = await _controller.Update(1, new KategorijaRequest { Naziv = "Historija" });

            Assert.IsType<ConflictObjectResult>(result);
            _kategorijaMock.Verify(r => r.UpdateAsync(It.IsAny<Kategorija>()), Times.Never);
        }

        [Fact]
        public async Task Update_IstaNazivIstaKategorija_DozvoljenoAzuriranje()
        {
            // Kategorija može biti snimljena bez promjene naziva (npr. mijenja samo Opis)
            var kategorija = TestKategorija(1, "Beletristika");

            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(kategorija);
            // GetAllAsync vraća samo tu istu kategoriju — k.Id == id, nema konflikta
            _kategorijaMock.Setup(r => r.GetAllAsync())
                           .ReturnsAsync(new List<Kategorija> { kategorija });
            _kategorijaMock.Setup(r => r.UpdateAsync(It.IsAny<Kategorija>())).Returns(Task.CompletedTask);

            var result = await _controller.Update(1, new KategorijaRequest
            {
                Naziv = "Beletristika",
                Opis = "Ažuriran opis"
            });

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Update_PrazanNaziv_VracaBadRequest()
        {
            // US-33 AC: Kada naziv je prazan, sistem prikazuje grešku
            var result = await _controller.Update(1, new KategorijaRequest { Naziv = "" });

            Assert.IsType<BadRequestObjectResult>(result);
            _kategorijaMock.Verify(r => r.UpdateAsync(It.IsAny<Kategorija>()), Times.Never);
        }

        // US-34 + US-32: Delete 

        [Fact]
        public async Task Delete_KategorijaNemaKnjige_BriseIVracaOk()
        {
            // US-34: Kada nema knjiga, brisanje je moguće
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKategorija());
            _kategorijaMock.Setup(r => r.HasBooksAsync(1)).ReturnsAsync(false);
            _kategorijaMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _controller.Delete(1);

            Assert.IsType<OkObjectResult>(result);
            _kategorijaMock.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task Delete_KategorijaImaKnjige_VracaConflict()
        {
            // US-32 AC: Sistem ne dozvoljava brisanje kategorije koja je u upotrebi
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKategorija());
            _kategorijaMock.Setup(r => r.HasBooksAsync(1)).ReturnsAsync(true);

            var result = await _controller.Delete(1);

            Assert.IsType<ConflictObjectResult>(result);
            _kategorijaMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Delete_KategorijaNePostoji_VracaNotFound()
        {
            _kategorijaMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Kategorija?)null);

            var result = await _controller.Delete(99);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Delete_KategorijaImaKnjige_PorukaObjasnjavaRazlog()
        {
            // US-32 AC: Prikazuje se jasna poruka zašto brisanje nije dozvoljeno
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKategorija());
            _kategorijaMock.Setup(r => r.HasBooksAsync(1)).ReturnsAsync(true);

            var result = await _controller.Delete(1);

            var conflict = Assert.IsType<ConflictObjectResult>(result);
            Assert.NotNull(conflict.Value);
        }

        [Fact]
        public async Task Create_NazivSadrziHtml_VracaBadRequest()
        {
            // US-30: Provjera zaštite od HTML injection-a
            var request = new KategorijaRequest
            {
                Naziv = "<b>Nauka</b>",
                Opis = "Validan opis"
            };

            var result = await _controller.Create(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("ne smije sadržavati HTML tagove", badRequest.Value.ToString());
        }

        [Fact]
        public async Task Update_OpisSadrziHtml_VracaBadRequest()
        {
            // US-33: Provjera zaštite od HTML injection-a pri ažuriranju
            var request = new KategorijaRequest
            {
                Naziv = "Validan Naziv",
                Opis = "<script>alert('xss')</script>"
            };

            var result = await _controller.Update(1, request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("ne smije sadržavati HTML tagove", badRequest.Value.ToString());
        }
    }
}