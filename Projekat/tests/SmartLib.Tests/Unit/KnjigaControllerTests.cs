using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Web.Controllers;
using Xunit;

namespace SmartLib.Tests.Unit
{
    /// <summary>
    /// Sprint 6 — Upravljanje knjigama i katalog
    /// </summary>
    public class KnjigaControllerTests
    {
        private readonly Mock<IKnjigaRepository> _knjigaMock;
        private readonly Mock<IPrimjerakRepository> _primjerakMock;
        private readonly Mock<IKategorijaRepository> _kategorijaMock;
        private readonly KnjigaController _controller;

        public KnjigaControllerTests()
        {
            _knjigaMock = new Mock<IKnjigaRepository>();
            _primjerakMock = new Mock<IPrimjerakRepository>();
            _kategorijaMock = new Mock<IKategorijaRepository>();

            _controller = new KnjigaController(
                _knjigaMock.Object,
                _primjerakMock.Object,
                _kategorijaMock.Object);

            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            _controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        }

        // ─── Pomoćne metode ───────────────────────────────────────────────────

        private static Kategorija TestKategorija(int id = 1) => new()
        {
            Id = id,
            Naziv = "Beletristika",
            Opis = "Test kategorija"
        };

        private static Knjiga TestKnjiga(int id = 1) => new()
        {
            Id = id,
            Naslov = "Test Naslov",
            Autor = "Test Autor",
            Isbn = "9780451524935",
            KategorijaId = 1,
            Kategorija = TestKategorija(),
            GodinaIzdanja = 2020,
            Primjerci = new List<Primjerak>
            {
                new() { Id = 1, KnjigaId = id, InventarniBroj = $"INV-{id}-001", Status = "dostupan" },
                new() { Id = 2, KnjigaId = id, InventarniBroj = $"INV-{id}-002", Status = "zadužen" }
            }
        };

        private KnjigaCreateDto ValidanCreateDto() => new()
        {
            Naslov = "Nova knjiga",
            Autor = "Neki Autor",
            Isbn = "9780451524936",
            KategorijaId = 1,
            GodinaIzdanja = 2022,
            BrojPrimjeraka = 2
        };

        // ─── Katalog (Index) ─────────────────────────────────────────────────

        [Fact]
        public async Task Index_VracaKatalogViewModel()
        {
            var knjige = new List<Knjiga> { TestKnjiga() };
            _knjigaMock.Setup(r => r.GetPagedAsync(null, null, 1, 10))
                       .ReturnsAsync((knjige, 1));

            var result = await _controller.Index(null, null, 1);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<KatalogViewModel>(view.Model);
            Assert.Single(model.Knjige);
            Assert.Equal(1, model.UkupnoStavki);
        }

        [Fact]
        public async Task Index_NemaKnjiga_VracaPrazanKatalog()
        {
            _knjigaMock.Setup(r => r.GetPagedAsync(null, null, 1, 10))
                       .ReturnsAsync((new List<Knjiga>(), 0));

            var result = await _controller.Index(null, null, 1);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<KatalogViewModel>(view.Model);
            Assert.Empty(model.Knjige);
            Assert.Equal(0, model.UkupnoStavki);
        }

        [Fact]
        public async Task Index_PaginacijaMetadata_IspravnaVrijednost()
        {
            var knjige = Enumerable.Range(1, 10).Select(i => TestKnjiga(i)).ToList();
            _knjigaMock.Setup(r => r.GetPagedAsync(null, null, 2, 10))
                       .ReturnsAsync((knjige, 25));

            var result = await _controller.Index(null, null, 2);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<KatalogViewModel>(view.Model);
            Assert.Equal(2, model.TrenutnaStrana);
            Assert.Equal(3, model.UkupnoStrana);
            Assert.Equal(25, model.UkupnoStavki);
            Assert.Equal(10, model.VelicinaStrane);
        }

        [Fact]
        public async Task Index_BrojDostupnihIzPrimjeraka_IspravanapoPrimjercima()
        {
            var knjiga = TestKnjiga();
            // Knjiga ima 2 primjerka: 1 dostupan, 1 zaduzen
            _knjigaMock.Setup(r => r.GetPagedAsync(null, null, 1, 10))
                       .ReturnsAsync((new List<Knjiga> { knjiga }, 1));

            var result = await _controller.Index(null, null, 1);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<KatalogViewModel>(view.Model);
            var dto = model.Knjige.First();
            Assert.Equal(2, dto.BrojPrimjeraka);
            Assert.Equal(1, dto.BrojDostupnih);
        }

        // ─── Dodavanje knjige (Create) ────────────────────────────────────────

        [Fact]
        public async Task Create_ValidanModel_SpremaKnjiguIRedirektuje()
        {
            var dto = ValidanCreateDto();
            _knjigaMock.Setup(r => r.GetByIsbnAsync("9780451524936")).ReturnsAsync((Knjiga?)null);
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKategorija());
            _kategorijaMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Kategorija> { TestKategorija() });
            _knjigaMock.Setup(r => r.CreateAsync(It.IsAny<Knjiga>()))
                       .ReturnsAsync((Knjiga k) => { k.Id = 10; return k; });
            _primjerakMock.Setup(r => r.CreateAsync(It.IsAny<Primjerak>()))
                          .ReturnsAsync((Primjerak p) => p);

            var result = await _controller.Create(dto);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task Create_ValidanModel_KreiraPrimjerkePremaKolicini()
        {
            var dto = ValidanCreateDto();
            dto.BrojPrimjeraka = 3;

            _knjigaMock.Setup(r => r.GetByIsbnAsync(It.IsAny<string>())).ReturnsAsync((Knjiga?)null);
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKategorija());
            _kategorijaMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Kategorija> { TestKategorija() });
            _knjigaMock.Setup(r => r.CreateAsync(It.IsAny<Knjiga>()))
                       .ReturnsAsync((Knjiga k) => { k.Id = 5; return k; });
            _primjerakMock.Setup(r => r.CreateAsync(It.IsAny<Primjerak>()))
                          .ReturnsAsync((Primjerak p) => p);

            await _controller.Create(dto);

            _primjerakMock.Verify(r => r.CreateAsync(It.IsAny<Primjerak>()), Times.Exactly(3));
        }

        [Fact]
        public async Task Create_NulaKopija_NeKreiraPrimjerke()
        {
            var dto = ValidanCreateDto();
            dto.BrojPrimjeraka = 0;

            _knjigaMock.Setup(r => r.GetByIsbnAsync(It.IsAny<string>())).ReturnsAsync((Knjiga?)null);
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKategorija());
            _kategorijaMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Kategorija> { TestKategorija() });
            _knjigaMock.Setup(r => r.CreateAsync(It.IsAny<Knjiga>()))
                       .ReturnsAsync((Knjiga k) => { k.Id = 6; return k; });

            await _controller.Create(dto);

            _primjerakMock.Verify(r => r.CreateAsync(It.IsAny<Primjerak>()), Times.Never);
        }

        [Fact]
        public async Task Create_NeispravanModel_VracaView()
        {
            _controller.ModelState.AddModelError("Naslov", "Naslov je obavezan.");
            _kategorijaMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Kategorija> { TestKategorija() });

            var result = await _controller.Create(new KnjigaCreateDto());

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_NevazanIsbn_DodajeModelGreskuIVracaView()
        {
            var dto = ValidanCreateDto();
            dto.Isbn = "123"; // prekratak ISBN

            _kategorijaMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Kategorija> { TestKategorija() });

            var result = await _controller.Create(dto);

            var view = Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
            Assert.True(_controller.ModelState.ContainsKey("Isbn"));
        }

        [Fact]
        public async Task Create_DuplikatIsbn_DodajeModelGreskuIVracaView()
        {
            var dto = ValidanCreateDto();
            dto.Isbn = "9780451524935"; // normalizovan ISBN

            _knjigaMock.Setup(r => r.GetByIsbnAsync("9780451524935")).ReturnsAsync(TestKnjiga());
            _kategorijaMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Kategorija> { TestKategorija() });

            var result = await _controller.Create(dto);

            var view = Assert.IsType<ViewResult>(result);
            Assert.True(_controller.ModelState.ContainsKey("Isbn"));
        }

        [Fact]
        public async Task Create_NevalidnaKategorija_DodajeModelGresku()
        {
            var dto = ValidanCreateDto();

            _knjigaMock.Setup(r => r.GetByIsbnAsync(It.IsAny<string>())).ReturnsAsync((Knjiga?)null);
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Kategorija?)null);
            _kategorijaMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Kategorija>());

            var result = await _controller.Create(dto);

            var view = Assert.IsType<ViewResult>(result);
            Assert.True(_controller.ModelState.ContainsKey("KategorijaId"));
        }

        // ─── Uređivanje knjige (Edit) ─────────────────────────────────────────

        [Fact]
        public async Task Edit_Get_PostojecaKnjiga_VracaView()
        {
            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKnjiga());
            _kategorijaMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Kategorija> { TestKategorija() });

            var result = await _controller.Edit(1);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<KnjigaEditDto>(view.Model);
            Assert.Equal("Test Naslov", model.Naslov);
        }

        [Fact]
        public async Task Edit_Get_NepostojecaKnjiga_VracaNotFound()
        {
            _knjigaMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Knjiga?)null);

            var result = await _controller.Edit(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_ValidanModel_AzuriraKnjiguIRedirektuje()
        {
            var knjiga = TestKnjiga();
            var editDto = new KnjigaEditDto
            {
                Id = 1,
                Naslov = "Novi naslov",
                Autor = "Novi autor",
                KategorijaId = 1,
                GodinaIzdanja = 2023
            };

            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(knjiga);
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKategorija());
            _kategorijaMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Kategorija> { TestKategorija() });
            _knjigaMock.Setup(r => r.UpdateAsync(It.IsAny<Knjiga>())).Returns(Task.CompletedTask);

            var result = await _controller.Edit(editDto);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Novi naslov", knjiga.Naslov);
            Assert.Equal("Novi autor", knjiga.Autor);
        }

        [Fact]
        public async Task Edit_Post_NeispravanModel_VracaView()
        {
            _controller.ModelState.AddModelError("Naslov", "Naslov je obavezan.");
            _kategorijaMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Kategorija> { TestKategorija() });

            var result = await _controller.Edit(new KnjigaEditDto { Id = 1 });

            Assert.IsType<ViewResult>(result);
            _knjigaMock.Verify(r => r.UpdateAsync(It.IsAny<Knjiga>()), Times.Never);
        }

        [Fact]
        public async Task Edit_Post_NepostojecaKnjiga_VracaNotFound()
        {
            _knjigaMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Knjiga?)null);
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKategorija());

            var result = await _controller.Edit(new KnjigaEditDto
            {
                Id = 999,
                Naslov = "X",
                Autor = "Y",
                KategorijaId = 1,
                GodinaIzdanja = 2020
            });

            Assert.IsType<NotFoundResult>(result);
        }

        // ─── ISBN normalizacija ───────────────────────────────────────────────

        [Fact]
        public async Task Create_IsbnSacrticama_NormalizujeSeIsprePohrane()
        {
            Knjiga? saved = null;
            var dto = ValidanCreateDto();
            dto.Isbn = "978-0-451-52493-6"; // ISBN sa crticama → treba normalizovati

            _knjigaMock.Setup(r => r.GetByIsbnAsync("9780451524936")).ReturnsAsync((Knjiga?)null);
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKategorija());
            _kategorijaMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Kategorija> { TestKategorija() });
            _knjigaMock.Setup(r => r.CreateAsync(It.IsAny<Knjiga>()))
                       .Callback<Knjiga>(k => saved = k)
                       .ReturnsAsync((Knjiga k) => { k.Id = 7; return k; });
            _primjerakMock.Setup(r => r.CreateAsync(It.IsAny<Primjerak>()))
                          .ReturnsAsync((Primjerak p) => p);

            await _controller.Create(dto);

            Assert.NotNull(saved);
            Assert.Equal("9780451524936", saved!.Isbn);
        }
    }
}
