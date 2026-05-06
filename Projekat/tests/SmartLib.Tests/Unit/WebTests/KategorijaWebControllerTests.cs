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
    /// PB-25: Upravljanje kategorijama knjiga (Web)
    ///     US-30: Dodavanje nove kategorije
    ///     US-31: Prikaz liste postojećih kategorija
    ///     US-32: Zabrana brisanja kategorije koja je u upotrebi
    ///     US-33: Uređivanje pojedinačnih kategorija
    ///     US-34: Brisanje kategorije iz sistema
    /// </summary>
    public class KategorijaWebControllerTests
    {
        private readonly Mock<IKategorijaRepository> _kategorijaMock;
        private readonly KategorijaController _controller;

        public KategorijaWebControllerTests()
        {
            _kategorijaMock = new Mock<IKategorijaRepository>();

            _controller = new KategorijaController(_kategorijaMock.Object);

            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            _controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        }

        // Pomoćne metode

        private static Kategorija TestKategorija(int id = 1, string naziv = "Beletristika") => new()
        {
            Id = id,
            Naziv = naziv,
            Opis = "Književna proza",
            Knjige = new List<Knjiga>()
        };

        // US-31: Index 

        [Fact]
        public async Task Index_VracaViewSaListomKategorija()
        {
            _kategorijaMock.Setup(r => r.GetAllAsync())
                           .ReturnsAsync(new List<Kategorija>
                           {
                               TestKategorija(1, "Beletristika"),
                               TestKategorija(2, "Nauka")
                           });

            var result = await _controller.Index();

            var view = Assert.IsType<ViewResult>(result);
            Assert.NotNull(view.Model);
        }

        [Fact]
        public async Task Index_NemaKategorija_VracaPrazanView()
        {
            // US-31 AC: Ako nema nijedne kategorije, prikazuje se odgovarajuća poruka
            _kategorijaMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Kategorija>());

            var result = await _controller.Index();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Kategorija>>(view.Model);
            Assert.Empty(model);
        }

        // US-30: Create 

        [Fact]
        public async Task Create_ValidanNaziv_SpremaIRedirektuje()
        {
            // US-30 AC: Kada kategorija uspješno dodata, prikazuje se u listi
            _kategorijaMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Kategorija>());
            _kategorijaMock.Setup(r => r.CreateAsync(It.IsAny<Kategorija>()))
                           .ReturnsAsync((Kategorija k) => { k.Id = 5; return k; });

            var result = await _controller.Create("Historija", "Historijska literatura");

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task Create_ValidanNaziv_PrikazujePorukuUspjeha()
        {
            _kategorijaMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Kategorija>());
            _kategorijaMock.Setup(r => r.CreateAsync(It.IsAny<Kategorija>()))
                           .ReturnsAsync((Kategorija k) => { k.Id = 5; return k; });

            await _controller.Create("Historija", null);

            Assert.NotNull(_controller.TempData["SuccessMessage"]);
            Assert.False(string.IsNullOrWhiteSpace(_controller.TempData["SuccessMessage"]?.ToString()));
        }

        [Fact]
        public async Task Create_PrazanNaziv_PrikazujeGreskuIRedirektuje()
        {
            // US-30 AC: Kada obavezni podaci nedostaju, prikazuje se poruka o grešci
            var result = await _controller.Create("   ", null);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.NotNull(_controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task Create_PrazanNaziv_NePozivaSeSprema()
        {
            await _controller.Create("", null);

            _kategorijaMock.Verify(r => r.CreateAsync(It.IsAny<Kategorija>()), Times.Never);
        }

        [Fact]
        public async Task Create_NazivVecPostoji_PrikazujeGreskuIRedirektuje()
        {
            // US-30 AC: Kada kategorija već postoji, sistem prikazuje poruku o grešci
            _kategorijaMock.Setup(r => r.GetAllAsync())
                           .ReturnsAsync(new List<Kategorija> { TestKategorija(1, "Beletristika") });

            var result = await _controller.Create("Beletristika", null);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.NotNull(_controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task Create_NazivVecPostoji_NePozivaSeSprema()
        {
            // Duplikat ne smije aktivirati CreateAsync
            _kategorijaMock.Setup(r => r.GetAllAsync())
                           .ReturnsAsync(new List<Kategorija> { TestKategorija(1, "Beletristika") });

            await _controller.Create("Beletristika", null);

            _kategorijaMock.Verify(r => r.CreateAsync(It.IsAny<Kategorija>()), Times.Never);
        }

        [Fact]
        public async Task Create_NazivCaseInsensitive_PrikazujeGresku()
        {
            // Provjera duplikata je case-insensitive
            _kategorijaMock.Setup(r => r.GetAllAsync())
                           .ReturnsAsync(new List<Kategorija> { TestKategorija(1, "Beletristika") });

            var result = await _controller.Create("beletristika", null);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(_controller.TempData["ErrorMessage"]);
        }

        // US-33: Edit 

        [Fact]
        public async Task Edit_ValidanModel_AzuriraIRedirektuje()
        {
            var kategorija = TestKategorija();

            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(kategorija);
            // Samo ta jedna kategorija postoji — nema konflikta
            _kategorijaMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Kategorija> { kategorija });
            _kategorijaMock.Setup(r => r.UpdateAsync(It.IsAny<Kategorija>())).Returns(Task.CompletedTask);

            var result = await _controller.Edit(1, "Nauka", "Naučna literatura");

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Nauka", kategorija.Naziv);
        }

        [Fact]
        public async Task Edit_ValidanModel_PrikazujePorukuUspjeha()
        {
            var kategorija = TestKategorija();

            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(kategorija);
            _kategorijaMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Kategorija> { kategorija });
            _kategorijaMock.Setup(r => r.UpdateAsync(It.IsAny<Kategorija>())).Returns(Task.CompletedTask);

            await _controller.Edit(1, "Nauka", null);

            Assert.NotNull(_controller.TempData["SuccessMessage"]);
        }

        [Fact]
        public async Task Edit_PrazanNaziv_PrikazujeGreskuIRedirektuje()
        {
            // US-33 AC: Kada naziv je prazan, sistem prikazuje grešku
            var result = await _controller.Edit(1, "  ", null);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.NotNull(_controller.TempData["ErrorMessage"]);

            _kategorijaMock.Verify(r => r.UpdateAsync(It.IsAny<Kategorija>()), Times.Never);
        }

        [Fact]
        public async Task Edit_NepostojecaKategorija_VracaNotFound()
        {
            _kategorijaMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Kategorija?)null);

            var result = await _controller.Edit(999, "Neki naziv", null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_NazivVecPostojiKodDrugeKategorije_PrikazujeGresku()
        {
            // US-33 AC: Kada naziv već postoji, sistem odbija izmjenu
            var kategorija1 = TestKategorija(1, "Beletristika");
            var kategorija2 = TestKategorija(2, "Historija");

            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(kategorija1);
            _kategorijaMock.Setup(r => r.GetAllAsync())
                           .ReturnsAsync(new List<Kategorija> { kategorija1, kategorija2 });

            var result = await _controller.Edit(1, "Historija", null);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.NotNull(_controller.TempData["ErrorMessage"]);
            _kategorijaMock.Verify(r => r.UpdateAsync(It.IsAny<Kategorija>()), Times.Never);
        }

        [Fact]
        public async Task Edit_IstaNazivIstaKategorija_DozvoljenoAzuriranje()
        {
            // Kategorija može biti snimljena bez promjene naziva (npr. mijenja samo Opis)
            var kategorija = TestKategorija(1, "Beletristika");

            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(kategorija);
            // GetAllAsync vraća samo tu kategoriju — k.Id == id, nema konflikta
            _kategorijaMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Kategorija> { kategorija });
            _kategorijaMock.Setup(r => r.UpdateAsync(It.IsAny<Kategorija>())).Returns(Task.CompletedTask);

            var result = await _controller.Edit(1, "Beletristika", "Ažuriran opis");

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.NotNull(_controller.TempData["SuccessMessage"]);
        }

        // US-34 + US-32: Delete 

        [Fact]
        public async Task Delete_KategorijaNemaKnjige_BriseIRedirektuje()
        {
            // US-34 AC: Kada kategorija nema knjiga, brisanje je dozvoljeno
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKategorija());
            _kategorijaMock.Setup(r => r.HasBooksAsync(1)).ReturnsAsync(false);
            _kategorijaMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _controller.Delete(1);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            _kategorijaMock.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task Delete_KategorijaNemaKnjige_PrikazujePorukuUspjeha()
        {
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKategorija());
            _kategorijaMock.Setup(r => r.HasBooksAsync(1)).ReturnsAsync(false);
            _kategorijaMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            await _controller.Delete(1);

            Assert.NotNull(_controller.TempData["SuccessMessage"]);
        }

        [Fact]
        public async Task Delete_KategorijaImaKnjige_PrikazujeGreskuINeBrise()
        {
            // US-32 AC: Sistem ne dozvoljava brisanje kategorije koja ima knjige
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKategorija());
            _kategorijaMock.Setup(r => r.HasBooksAsync(1)).ReturnsAsync(true);

            var result = await _controller.Delete(1);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.NotNull(_controller.TempData["ErrorMessage"]);
            _kategorijaMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Delete_KategorijaImaKnjige_PorukaJasnaKorisniku()
        {
            // US-32 AC: Prikazuje se jasna poruka zašto brisanje nije dozvoljeno
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKategorija());
            _kategorijaMock.Setup(r => r.HasBooksAsync(1)).ReturnsAsync(true);

            await _controller.Delete(1);

            var poruka = _controller.TempData["ErrorMessage"]?.ToString();
            Assert.False(string.IsNullOrWhiteSpace(poruka));
        }

        [Fact]
        public async Task Delete_NepostojecaKategorija_RedirektujeSaGreskom()
        {
            // Web kontroler ne vraća NotFound — redirekuje sa porukom
            _kategorijaMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Kategorija?)null);

            var result = await _controller.Delete(999);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.NotNull(_controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task GetById_PostojecaKategorija_VracaJson()
        {
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKategorija());

            var result = await _controller.GetById(1);

            Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public async Task GetById_NepostojecaKategorija_VracaNotFound()
        {
            _kategorijaMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Kategorija?)null);

            var result = await _controller.GetById(99);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_OpisSamoRazmaci_SpremaSeKaoNull()
        {
            _kategorijaMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Kategorija>());
            _kategorijaMock.Setup(r => r.CreateAsync(It.IsAny<Kategorija>()))
                           .ReturnsAsync((Kategorija k) => k);

            await _controller.Create("NovaKategorija", "   "); // whitespace opis

            _kategorijaMock.Verify(r => r.CreateAsync(
                It.Is<Kategorija>(k => k.Opis == null)), Times.Once);
        }

        [Fact]
        public async Task Create_ExceptionPriSprema_PrikazujeGresku()
        {
            _kategorijaMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Kategorija>());
            _kategorijaMock.Setup(r => r.CreateAsync(It.IsAny<Kategorija>()))
                           .ThrowsAsync(new Exception("DB greška"));

            var result = await _controller.Create("NovaKategorija", null);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.NotNull(_controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task Edit_ExceptionPriAzuriranju_PrikazujeGresku()
        {
            var kategorija = TestKategorija();
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(kategorija);
            _kategorijaMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Kategorija> { kategorija });
            _kategorijaMock.Setup(r => r.UpdateAsync(It.IsAny<Kategorija>()))
                           .ThrowsAsync(new Exception("DB greška"));

            var result = await _controller.Edit(1, "Nauka", null);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(_controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task Delete_ExceptionPriBrisanju_PrikazujeGresku()
        {
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKategorija());
            _kategorijaMock.Setup(r => r.HasBooksAsync(1)).ReturnsAsync(false);
            _kategorijaMock.Setup(r => r.DeleteAsync(1))
                           .ThrowsAsync(new Exception("DB greška"));

            var result = await _controller.Delete(1);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(_controller.TempData["ErrorMessage"]);
        }
    }
}