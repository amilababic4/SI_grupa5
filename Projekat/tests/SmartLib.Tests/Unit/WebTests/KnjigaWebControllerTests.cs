using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Services;
using SmartLib.Web.Controllers;
using Xunit;

namespace SmartLib.Tests.Unit.WebTests
{
    /// <summary>
    ///     Upravljanje knjigama i katalog
    /// </summary>
    public class KnjigaWebControllerTests
    {
        private readonly Mock<IKnjigaRepository> _knjigaMock;
        private readonly Mock<IPrimjerakRepository> _primjerakMock;
        private readonly Mock<IKategorijaRepository> _kategorijaMock;
        private readonly Mock<IZaduzenjeRepository> _zaduzenjeMock;
        private readonly Mock<IRezervacijaRepository> _rezervacijaMock;
        // ISPRAVKA #1: IListaKolekcijaRepository je obavezan parametar konstruktora
        private readonly Mock<IListaKolekcijaRepository> _listaKolekcijaRepositoryMock;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<ILogger<KnjigaController>> _loggerMock;
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _configuration;
        private readonly CacheVersionStore _cacheVersions;
        private readonly KnjigaController _controller;

        public KnjigaWebControllerTests()
        {
            _knjigaMock = new Mock<IKnjigaRepository>();
            _primjerakMock = new Mock<IPrimjerakRepository>();
            _kategorijaMock = new Mock<IKategorijaRepository>();
            _zaduzenjeMock = new Mock<IZaduzenjeRepository>();
            _rezervacijaMock = new Mock<IRezervacijaRepository>();
            // ISPRAVKA #1: Dodan nedostajući mock
            _listaKolekcijaRepositoryMock = new Mock<IListaKolekcijaRepository>();
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _loggerMock = new Mock<ILogger<KnjigaController>>();
            _configuration = new ConfigurationBuilder().Build();

            _cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            _cacheVersions = new CacheVersionStore();

            // ISPRAVKA #1: Konstruktor sada uključuje IListaKolekcijaRepository
            _controller = new KnjigaController(
                _knjigaMock.Object,
                _primjerakMock.Object,
                _kategorijaMock.Object,
                _zaduzenjeMock.Object,
                _rezervacijaMock.Object,
                _listaKolekcijaRepositoryMock.Object,
                _httpClientFactoryMock.Object,
                _cache,
                _configuration,
                _loggerMock.Object,
                _cacheVersions
            );

            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            _controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        }

        // ── Pomoćne metode ────────────────────────────────────────────────────────

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

        /// <summary>
        /// Centralni setup za Index pozive.
        /// GetPagedAsync ima 7 parametara (naslov, autor, page, pageSize, kategorijaId, izdavac, godinaIzdanja).
        /// </summary>
        private void SetupIndexMocks(
            List<Knjiga> knjige,
            int ukupno,
            string? naslov = null,
            string? autor = null,
            int page = 1,
            int pageSize = 16,
            int? kategorijaId = null,
            string? izdavac = null,
            int? godinaIzdanja = null)
        {
            _knjigaMock
                .Setup(r => r.GetPagedAsync(naslov, autor, page, pageSize, kategorijaId, izdavac, godinaIzdanja))
                .ReturnsAsync((knjige, ukupno));

            _kategorijaMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Kategorija> { TestKategorija() });

            _knjigaMock
                .Setup(r => r.GetDistinctIzdavaciAsync())
                .ReturnsAsync(new List<string> { "Pearson", "O'Reilly" });

            _knjigaMock
                .Setup(r => r.GetDistinctGodineAsync())
                .ReturnsAsync(new List<int> { 2023, 2022, 2020 });
        }

        // ── Katalog (Index) ───────────────────────────────────────────────────────

        [Fact]
        public async Task Index_VracaKatalogViewModel()
        {
            var knjige = new List<Knjiga> { TestKnjiga() };
            SetupIndexMocks(knjige, 1);

            var result = await _controller.Index(null, null, null, null, null, false, 1);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<KatalogViewModel>(view.Model);
            Assert.Single(model.Knjige);
            Assert.Equal(1, model.UkupnoStavki);
        }

        [Fact]
        public async Task Index_NemaKnjiga_VracaPrazanKatalog()
        {
            SetupIndexMocks(new List<Knjiga>(), 0);

            var result = await _controller.Index(null, null, null, null, null, false, 1);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<KatalogViewModel>(view.Model);
            Assert.Empty(model.Knjige);
            Assert.Equal(0, model.UkupnoStavki);
        }

        [Fact]
        public async Task Index_PaginacijaMetadata_IspravnaVrijednost()
        {
            var knjige = Enumerable.Range(1, 10).Select(i => TestKnjiga(i)).ToList();
            SetupIndexMocks(knjige, 25, page: 2);

            var result = await _controller.Index(null, null, null, null, null, false, 2);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<KatalogViewModel>(view.Model);
            Assert.Equal(2, model.TrenutnaStrana);
            Assert.Equal(2, model.UkupnoStrana);
            Assert.Equal(25, model.UkupnoStavki);
            Assert.Equal(16, model.VelicinaStrane);
        }

        [Fact]
        public async Task Index_BrojDostupnihIzPrimjeraka_IspravanapoPrimjercima()
        {
            var knjiga = TestKnjiga();
            // Knjiga ima 2 primjerka: 1 dostupan, 1 zaduzen
            SetupIndexMocks(new List<Knjiga> { knjiga }, 1);

            var result = await _controller.Index(null, null, null, null, null, false, 1);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<KatalogViewModel>(view.Model);
            var dto = model.Knjige.First();
            Assert.Equal(2, dto.BrojPrimjeraka);
            Assert.Equal(1, dto.BrojDostupnih);
        }

        [Fact]
        public async Task Index_PageManjiOd1_KoristiStranu1()
        {
            SetupIndexMocks(new List<Knjiga>(), 0);

            var result = await _controller.Index(null, null, null, null, null, false, -5);

            // ISPRAVKA #2: Index prima page kao 7. argument, a pageSize kao 8.
            // Signatura: Index(naslov, autor, kategorijaId, izdavac, godinaIzdanja, samoNeprocitane=false, page=1, pageSize=16)
            // Sa pozivom _controller.Index(null, null, null, null, null, -5) — šesti argument je samoNeprocitane (bool),
            // što znači da se -5 kastuje u bool (true). Page ostaje default 1.
            // Stari test je koristio pogrešnu signaturu — ispraviti Verify da odgovara stvarnoj logici.
            _knjigaMock.Verify(r => r.GetPagedAsync(null, null, 1, 16, null, null, null), Times.Once);
        }

        [Fact]
        public async Task Index_KnjigaBezKategorije_KategorijaJeNull()
        {
            var knjiga = TestKnjiga();
            knjiga.Kategorija = null;
            SetupIndexMocks(new List<Knjiga> { knjiga }, 1);

            var result = await _controller.Index(null, null, null, null, null, false, 1);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<KatalogViewModel>(view.Model);
            Assert.Null(model.Knjige.First().Kategorija);
        }

        [Fact]
        public async Task Index_NemaKnjiga_UkupnoStranaJe1()
        {
            SetupIndexMocks(new List<Knjiga>(), 0);

            var result = await _controller.Index(null, null, null, null, null, false, 1);

            var model = ((ViewResult)result).Model as KatalogViewModel;
            Assert.Equal(1, model!.UkupnoStrana);
        }

        // ── PB-44: Napredni filteri (Index) ───────────────────────────────────────

        [Fact]
        public async Task Index_FilterPoKategoriji_VracaSamoBooksIzKategorije()
        {
            // US-74: Filter po kategoriji
            var knjige = new List<Knjiga> { TestKnjiga() };
            SetupIndexMocks(knjige, 1, kategorijaId: 1);

            // ISPRAVKA #3: Signatura je Index(naslov, autor, kategorijaId, izdavac, godinaIzdanja, page)
            // ali stvarna signatura kontrolera je:
            // Index(naslov, autor, kategorijaId, izdavac, godinaIzdanja, samoNeprocitane=false, page=1, pageSize=16)
            // Pozivi sa 6 argumenata: 6. je samoNeprocitane (bool), page ide kao 7.
            // Stari testovi su slali page kao 6. argument — potrebno ih je ispraviti.
            var result = await _controller.Index(null, null, 1, null, null, false, 1);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<KatalogViewModel>(view.Model);
            Assert.Single(model.Knjige);
            Assert.Equal(1, model.KategorijaId);
        }

        [Fact]
        public async Task Index_FilterPoKategoriji_NemaRezultata_VracaPraznuListu()
        {
            // US-74: Ako ne postoji nijedna knjiga u kategoriji
            SetupIndexMocks(new List<Knjiga>(), 0, kategorijaId: 99);

            var result = await _controller.Index(null, null, 99, null, null, false, 1);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<KatalogViewModel>(view.Model);
            Assert.Empty(model.Knjige);
            Assert.Equal(0, model.UkupnoStavki);
        }

        [Fact]
        public async Task Index_FilterPoIzdavacu_VracaSamoKnjigeIzdavaca()
        {
            // US-75: Filter po izdavaču
            var knjiga = TestKnjiga();
            knjiga.Izdavac = "Pearson";
            SetupIndexMocks(new List<Knjiga> { knjiga }, 1, izdavac: "Pearson");

            var result = await _controller.Index(null, null, null, "Pearson", null, false, 1);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<KatalogViewModel>(view.Model);
            Assert.Single(model.Knjige);
            Assert.Equal("Pearson", model.Izdavac);
        }

        [Fact]
        public async Task Index_FilterPoIzdavacu_NemaRezultata_VracaPraznuListu()
        {
            // US-75: Ako nema knjiga za odabranog izdavača
            SetupIndexMocks(new List<Knjiga>(), 0, izdavac: "Nepostojeci");

            var result = await _controller.Index(null, null, null, "Nepostojeci", null, false, 1);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<KatalogViewModel>(view.Model);
            Assert.Empty(model.Knjige);
        }

        [Fact]
        public async Task Index_FilterPoGodini_VracaSamoKnjigeIzGodine()
        {
            // US-76: Filter po godini izdanja
            var knjiga = TestKnjiga();
            SetupIndexMocks(new List<Knjiga> { knjiga }, 1, godinaIzdanja: 2020);

            var result = await _controller.Index(null, null, null, null, 2020, false, 1);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<KatalogViewModel>(view.Model);
            Assert.Single(model.Knjige);
            Assert.Equal(2020, model.GodinaIzdanja);
        }

        [Fact]
        public async Task Index_FilterPoGodini_NemaRezultata_VracaPraznuListu()
        {
            // US-76: Ako nema knjiga za odabranu godinu
            SetupIndexMocks(new List<Knjiga>(), 0, godinaIzdanja: 1800);

            var result = await _controller.Index(null, null, null, null, 1800, false, 1);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<KatalogViewModel>(view.Model);
            Assert.Empty(model.Knjige);
        }

        [Fact]
        public async Task Index_KombinacijaFiltera_ProslijedjujeSveFiltereRepozitoriju()
        {
            // US-78: Kombinacija filtera — sve aktivne filtere šalje repozitoriju
            var knjige = new List<Knjiga> { TestKnjiga() };
            SetupIndexMocks(knjige, 1,
                naslov: "Test",
                autor: "Autor",
                kategorijaId: 1,
                izdavac: "Pearson",
                godinaIzdanja: 2020);

            var result = await _controller.Index("Test", "Autor", 1, "Pearson", 2020, false, 1);

            _knjigaMock.Verify(
                r => r.GetPagedAsync("Test", "Autor", 1, 16, 1, "Pearson", 2020),
                Times.Once);
        }

        [Fact]
        public async Task Index_KombinacijaFiltera_NemaRezultata_VracaPraznuListu()
        {
            // US-78: Nijedna knjiga ne zadovoljava kombinovane filtere
            SetupIndexMocks(new List<Knjiga>(), 0,
                kategorijaId: 1,
                izdavac: "Pearson",
                godinaIzdanja: 2020);

            var result = await _controller.Index(null, null, 1, "Pearson", 2020, false, 1);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<KatalogViewModel>(view.Model);
            Assert.Empty(model.Knjige);
            Assert.Equal(0, model.UkupnoStavki);
        }

        [Fact]
        public async Task Index_ModelSadrziKategorijeListe_ZaDropdown()
        {
            // Provjera da ViewModel sadrži kategorije za filter dropdown
            SetupIndexMocks(new List<Knjiga>(), 0);

            var result = await _controller.Index(null, null, null, null, null, false, 1);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<KatalogViewModel>(view.Model);
            Assert.NotEmpty(model.Kategorije);
        }

        [Fact]
        public async Task Index_ModelSadrziIzdavaceListe_ZaDropdown()
        {
            // Provjera da ViewModel sadrži izdavače za filter dropdown
            SetupIndexMocks(new List<Knjiga>(), 0);

            var result = await _controller.Index(null, null, null, null, null, false, 1);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<KatalogViewModel>(view.Model);
            Assert.NotEmpty(model.Izdavaci);
        }

        [Fact]
        public async Task Index_ModelSadrziGodineListu_ZaDropdown()
        {
            // Provjera da ViewModel sadrži godine za filter dropdown
            SetupIndexMocks(new List<Knjiga>(), 0);

            var result = await _controller.Index(null, null, null, null, null, false, 1);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<KatalogViewModel>(view.Model);
            Assert.NotEmpty(model.Godine);
        }

        [Fact]
        public async Task Index_BezAktivnihFiltera_ImaAktivnihFilteraJeFalse()
        {
            SetupIndexMocks(new List<Knjiga>(), 0);

            var result = await _controller.Index(null, null, null, null, null, false, 1);

            var model = ((ViewResult)result).Model as KatalogViewModel;
            Assert.False(model!.ImaAktivnihFiltera);
        }

        [Fact]
        public async Task Index_SaKategorijaFilterom_ImaNapredniFilterJeTrue()
        {
            SetupIndexMocks(new List<Knjiga>(), 0, kategorijaId: 1);

            // ISPRAVKA #3 primijenjena: samoNeprocitane eksplicitno false, page 1
            var result = await _controller.Index(null, null, 1, null, null, false, 1);

            var model = ((ViewResult)result).Model as KatalogViewModel;
            Assert.True(model!.ImaNapredniFilter);
        }

        [Fact]
        public async Task Index_SamoOsnovnaPretraga_ImaNapredniFilterJeFalse()
        {
            // Osnovna pretraga ne aktivira "napredni filter" indikator
            SetupIndexMocks(new List<Knjiga>(), 0, naslov: "Test");

            var result = await _controller.Index("Test", null, null, null, null, false, 1);

            var model = ((ViewResult)result).Model as KatalogViewModel;
            Assert.False(model!.ImaNapredniFilter);
            Assert.True(model.ImaAktivnihFiltera); // ali ImaAktivnihFiltera je true
        }

        // ── Dodavanje knjige (Create) ─────────────────────────────────────────────

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
            dto.Isbn = "9780451524935";

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

        [Fact]
        public async Task Create_NeispravanIsbn_NeProvjeravaDuplikate()
        {
            var dto = ValidanCreateDto();
            dto.Isbn = "12345678901"; // 11 znakova — nevalidan

            _kategorijaMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Kategorija> { TestKategorija() });

            await _controller.Create(dto);

            // GetByIsbnAsync se ne smije pozvati jer ISBN nije ni validan
            _knjigaMock.Verify(r => r.GetByIsbnAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Create_IsbnSacrticama_NormalizujeSeIsprePohrane()
        {
            Knjiga? saved = null;
            var dto = ValidanCreateDto();
            dto.Isbn = "978-0-451-52493-6";

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

        [Fact]
        public async Task Create_IsbnSaXNaKraju_Validan()
        {
            var dto = ValidanCreateDto();
            dto.Isbn = "080442957X";

            _knjigaMock.Setup(r => r.GetByIsbnAsync("080442957X")).ReturnsAsync((Knjiga?)null);
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKategorija());
            _kategorijaMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Kategorija> { TestKategorija() });
            _knjigaMock.Setup(r => r.CreateAsync(It.IsAny<Knjiga>()))
                       .ReturnsAsync((Knjiga k) => { k.Id = 1; return k; });
            _primjerakMock.Setup(r => r.CreateAsync(It.IsAny<Primjerak>()))
                          .ReturnsAsync((Primjerak p) => p);

            var result = await _controller.Create(dto);

            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Create_IsbnPogresneDuzine_VracaBadRequest()
        {
            var dto = ValidanCreateDto();
            dto.Isbn = "12345678901"; // 11 znakova

            _kategorijaMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Kategorija> { TestKategorija() });

            var result = await _controller.Create(dto);

            var view = Assert.IsType<ViewResult>(result);
            Assert.True(_controller.ModelState.ContainsKey("Isbn"));
        }

        [Fact]
        public async Task Create_Isbn10SaNevazanimZadnjimZnakom_VracaGresku()
        {
            var dto = ValidanCreateDto();
            dto.Isbn = "080442957Z"; // ISBN-10 ali Z na kraju

            _kategorijaMock.Setup(r => r.GetAllAsync())
                           .ReturnsAsync(new List<Kategorija> { TestKategorija() });

            var result = await _controller.Create(dto);

            var view = Assert.IsType<ViewResult>(result);
            Assert.True(_controller.ModelState.ContainsKey("Isbn"));
        }

        [Fact]
        public async Task Create_SaIzdavacem_TrimujеIzdavaca()
        {
            Knjiga? saved = null;
            var dto = ValidanCreateDto();
            dto.Izdavac = "  Pearson  ";

            _knjigaMock.Setup(r => r.GetByIsbnAsync(It.IsAny<string>())).ReturnsAsync((Knjiga?)null);
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKategorija());
            _kategorijaMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Kategorija> { TestKategorija() });
            _knjigaMock.Setup(r => r.CreateAsync(It.IsAny<Knjiga>()))
                       .Callback<Knjiga>(k => saved = k)
                       .ReturnsAsync((Knjiga k) => { k.Id = 1; return k; });
            _primjerakMock.Setup(r => r.CreateAsync(It.IsAny<Primjerak>()))
                          .ReturnsAsync((Primjerak p) => p);

            await _controller.Create(dto);

            Assert.Equal("Pearson", saved!.Izdavac);
        }

        [Fact]
        public async Task Create_Get_VracaViewSaPraznimModelom()
        {
            _kategorijaMock.Setup(r => r.GetAllAsync())
                           .ReturnsAsync(new List<Kategorija> { TestKategorija() });

            var result = await _controller.Create();

            var view = Assert.IsType<ViewResult>(result);
            Assert.IsType<KnjigaCreateDto>(view.Model);
        }

        [Fact]
        public async Task Create_ValidanModel_BumpujeBooksVersion()
        {
            // Provjera da se cache invalidira nakon uspješnog kreiranja
            var dto = ValidanCreateDto();
            var versionPrije = _cacheVersions.BooksVersion;

            _knjigaMock.Setup(r => r.GetByIsbnAsync(It.IsAny<string>())).ReturnsAsync((Knjiga?)null);
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKategorija());
            _kategorijaMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Kategorija> { TestKategorija() });
            _knjigaMock.Setup(r => r.CreateAsync(It.IsAny<Knjiga>()))
                       .ReturnsAsync((Knjiga k) => { k.Id = 10; return k; });
            _primjerakMock.Setup(r => r.CreateAsync(It.IsAny<Primjerak>()))
                          .ReturnsAsync((Primjerak p) => p);

            await _controller.Create(dto);

            Assert.NotEqual(versionPrije, _cacheVersions.BooksVersion);
        }

        // ── Uređivanje knjige (Edit) ──────────────────────────────────────────────

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
        public async Task Edit_Get_PostavljaIsbnUViewBag()
        {
            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKnjiga());
            _kategorijaMock.Setup(r => r.GetAllAsync())
                           .ReturnsAsync(new List<Kategorija> { TestKategorija() });

            await _controller.Edit(1);

            Assert.Equal("9780451524935", _controller.ViewBag.Isbn);
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

        [Fact]
        public async Task Edit_Post_NevalidnaKategorija_VracaViewSaGreskom()
        {
            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKnjiga());
            _kategorijaMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Kategorija?)null);
            _kategorijaMock.Setup(r => r.GetAllAsync())
                           .ReturnsAsync(new List<Kategorija> { TestKategorija() });

            var result = await _controller.Edit(new KnjigaEditDto
            {
                Id = 1,
                Naslov = "Test",
                Autor = "Autor",
                KategorijaId = 99,
                GodinaIzdanja = 2020
            });

            var view = Assert.IsType<ViewResult>(result);
            Assert.True(_controller.ModelState.ContainsKey("KategorijaId"));
        }

        [Fact]
        public async Task Edit_Post_SaIzdavacem_TrimujеIzdavaca()
        {
            var knjiga = TestKnjiga();

            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(knjiga);
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKategorija());
            _kategorijaMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Kategorija> { TestKategorija() });
            _knjigaMock.Setup(r => r.UpdateAsync(It.IsAny<Knjiga>())).Returns(Task.CompletedTask);

            await _controller.Edit(new KnjigaEditDto
            {
                Id = 1,
                Naslov = "Test",
                Autor = "Autor",
                KategorijaId = 1,
                GodinaIzdanja = 2020,
                Izdavac = "  Pearson  "
            });

            Assert.Equal("Pearson", knjiga.Izdavac);
        }

        [Fact]
        public async Task Edit_Post_ValidanModel_BumpujeBooksVersion()
        {
            // Provjera da se cache invalidira nakon uspješnog ažuriranja
            var knjiga = TestKnjiga();
            var versionPrije = _cacheVersions.BooksVersion;

            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(knjiga);
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKategorija());
            _kategorijaMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Kategorija> { TestKategorija() });
            _knjigaMock.Setup(r => r.UpdateAsync(It.IsAny<Knjiga>())).Returns(Task.CompletedTask);

            await _controller.Edit(new KnjigaEditDto
            {
                Id = 1,
                Naslov = "Test",
                Autor = "Autor",
                KategorijaId = 1,
                GodinaIzdanja = 2020
            });

            Assert.NotEqual(versionPrije, _cacheVersions.BooksVersion);
        }

        // ── Details ───────────────────────────────────────────────────────────────

        [Fact]
        public async Task Details_PostojecaKnjiga_VracaView()
        {
            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKnjiga());
            // ISPRAVKA #4: Details prima i IRecenzijaPrijavaRepository kao FromServices
            var recenzijaMock = new Mock<IRecenzijaRepository>();
            var prijavaRepositoryMock = new Mock<IRecenzijaPrijavaRepository>();
            recenzijaMock.Setup(r => r.GetByKnjigaIdAsync(1)).ReturnsAsync(new List<Recenzija>());
            // ISPRAVKA #5: ZaduzenjeRepository.CountByKnjigaIdAsync je potreban za Details
            _zaduzenjeMock.Setup(r => r.CountByKnjigaIdAsync(1)).ReturnsAsync(0);

            var result = await _controller.Details(1, recenzijaMock.Object, prijavaRepositoryMock.Object);

            var view = Assert.IsType<ViewResult>(result);
            Assert.IsType<KnjigaDto>(view.Model);
        }

        [Fact]
        public async Task Details_NepostojecaKnjiga_VracaNotFound()
        {
            _knjigaMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Knjiga?)null);
            var recenzijaMock = new Mock<IRecenzijaRepository>();
            var prijavaRepositoryMock = new Mock<IRecenzijaPrijavaRepository>();
            _zaduzenjeMock.Setup(r => r.CountByKnjigaIdAsync(99)).ReturnsAsync(0);

            var result = await _controller.Details(99, recenzijaMock.Object, prijavaRepositoryMock.Object);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_KnjigaBezKategorije_KategorijaJeNull()
        {
            var knjiga = TestKnjiga();
            knjiga.Kategorija = null;
            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(knjiga);
            var recenzijaMock = new Mock<IRecenzijaRepository>();
            var prijavaRepositoryMock = new Mock<IRecenzijaPrijavaRepository>();
            recenzijaMock.Setup(r => r.GetByKnjigaIdAsync(1)).ReturnsAsync(new List<Recenzija>());
            _zaduzenjeMock.Setup(r => r.CountByKnjigaIdAsync(1)).ReturnsAsync(0);

            var result = await _controller.Details(1, recenzijaMock.Object, prijavaRepositoryMock.Object);

            var view = Assert.IsType<ViewResult>(result);
            var dto = Assert.IsType<KnjigaDto>(view.Model);
            Assert.Null(dto.Kategorija);
        }

        [Fact]
        public async Task Details_PostavljaBrojZaduzenjaUViewBag()
        {
            // Provjera da se ViewBag.BrojZaduzenja ispravno popunjava
            _knjigaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestKnjiga());
            var recenzijaMock = new Mock<IRecenzijaRepository>();
            var prijavaRepositoryMock = new Mock<IRecenzijaPrijavaRepository>();
            recenzijaMock.Setup(r => r.GetByKnjigaIdAsync(1)).ReturnsAsync(new List<Recenzija>());
            _zaduzenjeMock.Setup(r => r.CountByKnjigaIdAsync(1)).ReturnsAsync(5);

            await _controller.Details(1, recenzijaMock.Object, prijavaRepositoryMock.Object);

            Assert.Equal(5, _controller.ViewBag.BrojZaduzenja);
        }

        // ── Delete ────────────────────────────────────────────────────────────────

        [Fact]
        public async Task Delete_KnjigaImaAktivnaZaduzenja_RedirektujeSaGreskom()
        {
            _knjigaMock.Setup(r => r.HasActiveLoansAsync(1)).ReturnsAsync(true);

            var result = await _controller.Delete(1);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.NotNull(_controller.TempData["ErrorMessage"]);
            _knjigaMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Delete_DeleteAsyncVracaFalse_PrikazujeGresku()
        {
            _knjigaMock.Setup(r => r.HasActiveLoansAsync(1)).ReturnsAsync(false);
            _knjigaMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(false);

            var result = await _controller.Delete(1);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(_controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task Delete_ExceptionPriBrisanju_PrikazujeGresku()
        {
            _knjigaMock.Setup(r => r.HasActiveLoansAsync(1)).ReturnsAsync(false);
            _knjigaMock.Setup(r => r.DeleteAsync(1)).ThrowsAsync(new Exception("DB greška"));

            var result = await _controller.Delete(1);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(_controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task Delete_Uspjesno_PrikazujePorukuUspjeha()
        {
            _knjigaMock.Setup(r => r.HasActiveLoansAsync(1)).ReturnsAsync(false);
            _knjigaMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            await _controller.Delete(1);

            Assert.NotNull(_controller.TempData["SuccessMessage"]);
        }

        [Fact]
        public async Task Delete_Uspjesno_BumpujeBooksVersion()
        {
            // Provjera da se cache invalidira samo pri uspješnom brisanju
            var versionPrije = _cacheVersions.BooksVersion;
            _knjigaMock.Setup(r => r.HasActiveLoansAsync(1)).ReturnsAsync(false);
            _knjigaMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            await _controller.Delete(1);

            Assert.NotEqual(versionPrije, _cacheVersions.BooksVersion);
        }

        [Fact]
        public async Task Delete_Neuspjesno_NeBumpujeBooksVersion()
        {
            // Cache se ne smije invalidirati ako brisanje nije uspjelo
            var versionPrije = _cacheVersions.BooksVersion;
            _knjigaMock.Setup(r => r.HasActiveLoansAsync(1)).ReturnsAsync(false);
            _knjigaMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(false);

            await _controller.Delete(1);

            Assert.Equal(versionPrije, _cacheVersions.BooksVersion);
        }

        // ── Korice ────────────────────────────────────────────────────────────────

        [Fact]
        public async Task Korice_PrazanIsbn_VracaNotFound()
        {
            var result = await _controller.Korice("");
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Korice_NullIsbn_VracaNotFound()
        {
            var result = await _controller.Korice(null!);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Korice_CacheHit_VracaFileIzCachea()
        {
            var imageBytes = new byte[] { 1, 2, 3 };
            await _cache.SetBytesAsync("cover_1234567890_M", imageBytes, TimeSpan.FromHours(1));

            var result = await _controller.Korice("1234567890");

            Assert.IsType<FileContentResult>(result);
        }

        [Fact]
        public async Task Korice_IsbnSaCrticama_NormalizujeSeZaCacheKljuc()
        {
            var imageBytes = new byte[] { 1, 2, 3 };
            await _cache.SetBytesAsync("cover_9780451524935_M", imageBytes, TimeSpan.FromHours(1));

            var result = await _controller.Korice("978-0-451-52493-5");

            Assert.IsType<FileContentResult>(result);
        }

        [Fact]
        public async Task Korice_DrugacijaVelicina_KoristiIspravanCacheKljuc()
        {
            var imageBytes = new byte[] { 1, 2, 3 };
            await _cache.SetBytesAsync("cover_1234567890_L", imageBytes, TimeSpan.FromHours(1));

            var result = await _controller.Korice("1234567890", "L");

            Assert.IsType<FileContentResult>(result);
        }

        [Fact]
        public async Task Korice_HttpClientVracaUspjesanOdgovor_VracaFileISpremaUCache()
        {
            var imageBytes = new byte[3001];
            imageBytes[0] = 0xFF;
            imageBytes[1] = 0xD8;
            imageBytes[2] = 0xFF;

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(imageBytes)
                });

            var httpClient = new HttpClient(handlerMock.Object);
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));

            var controller = BuildController(httpClientFactoryMock.Object, cache);
            SetupControllerContext(controller);

            var result = await controller.Korice("1234567890");

            Assert.IsType<FileContentResult>(result);
            var cached = await cache.GetBytesAsync("cover_1234567890_M");
            Assert.Equal(imageBytes, cached);
        }

        [Fact]
        public async Task Korice_HttpClientVracaNeuspjesanStatusCode_VracaFallbackSvg()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.NotFound));

            var httpClient = new HttpClient(handlerMock.Object);
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            var controller = BuildController(httpClientFactoryMock.Object, cache);
            SetupControllerContext(controller);

            var result = await controller.Korice("1234567890");

            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("image/svg+xml", fileResult.ContentType);
        }

        [Fact]
        public async Task Korice_HttpClientBacaException_VracaFallbackSvg()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network error"));

            var httpClient = new HttpClient(handlerMock.Object);
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            var controller = BuildController(httpClientFactoryMock.Object, cache);
            SetupControllerContext(controller);

            var result = await controller.Korice("1234567890");

            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("image/svg+xml", fileResult.ContentType);
        }

        // ── GetCatalogRecommendation ───────────────────────────────────────────────

        [Fact]
        public async Task GetCatalogRecommendation_VracaJsonSaKnjigom()
        {
            // Provjera osnovnog toka — slučajan izbor knjige
            var knjige = new List<Knjiga> { TestKnjiga() };
            _knjigaMock.Setup(r => r.GetRandomAsync(50)).ReturnsAsync(knjige);

            var result = await _controller.GetCatalogRecommendation();

            Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public async Task GetCatalogRecommendation_NemaKnjiga_VracaNotFound()
        {
            _knjigaMock.Setup(r => r.GetRandomAsync(50)).ReturnsAsync(new List<Knjiga>());

            var result = await _controller.GetCatalogRecommendation();

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetCatalogRecommendation_SaKategorijom_PreferirsKnjigeTeKategorije()
        {
            var knjigaUKategoriji = TestKnjiga(1);
            knjigaUKategoriji.Kategorija = TestKategorija();

            var knjigaDrugaKategorija = TestKnjiga(2);
            knjigaDrugaKategorija.Kategorija = new Kategorija { Id = 2, Naziv = "Nauka" };

            _knjigaMock.Setup(r => r.GetRandomAsync(50))
                       .ReturnsAsync(new List<Knjiga> { knjigaUKategoriji, knjigaDrugaKategorija });

            var result = await _controller.GetCatalogRecommendation("Beletristika");

            var jsonResult = Assert.IsType<JsonResult>(result);
            // Rezultat mora biti serijalizabilan — samo provjera tipa
            Assert.NotNull(jsonResult.Value);
        }

        [Fact]
        public async Task GetCatalogRecommendation_RepozitorijBacaException_VracaStatusCode500()
        {
            _knjigaMock.Setup(r => r.GetRandomAsync(50)).ThrowsAsync(new Exception("DB greška"));

            var result = await _controller.GetCatalogRecommendation();

            var statusResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        // ── Helperi ───────────────────────────────────────────────────────────────

        /// <summary>
        /// ISPRAVKA #1: BuildController je refaktorisan da uključuje IListaKolekcijaRepository.
        /// </summary>
        private KnjigaController BuildController(IHttpClientFactory factory, IDistributedCache cache)
        {
            return new KnjigaController(
                _knjigaMock.Object,
                _primjerakMock.Object,
                _kategorijaMock.Object,
                _zaduzenjeMock.Object,
                _rezervacijaMock.Object,
                _listaKolekcijaRepositoryMock.Object,
                factory,
                cache,
                _configuration,
                _loggerMock.Object,
                _cacheVersions);
        }

        private void SetupControllerContext(KnjigaController controller)
        {
            var httpContext = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        }
    }
}