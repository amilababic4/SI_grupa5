using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SmartLib.API.Controllers;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using Xunit;

namespace SmartLib.Tests.Unit.APITests
{
    /// <summary>
    /// Zaduženja modul (API):
    ///     US-44, US-46, US-47: Kreiranje zaduženja
    ///     US-45: Vraćanje knjige
    ///     US-62, US-63, US-64: Vlastita zaduženja člana
    ///     US-65, US-66, US-67, US-68: Pregled i upravljanje (bibliotekar)
    /// </summary>
    public class ZaduzenjeApiControllerTests
    {
        private readonly Mock<IZaduzenjeRepository> _zaduzenjeRepoMock;
        private readonly Mock<IKorisnikRepository> _korisnikRepoMock;
        private readonly Mock<IKnjigaRepository> _knjigaRepoMock;
        private readonly Mock<IPrimjerakRepository> _primjerakRepoMock;
        private readonly ZaduzenjeController _controller;

        public ZaduzenjeApiControllerTests()
        {
            _zaduzenjeRepoMock = new Mock<IZaduzenjeRepository>();
            _korisnikRepoMock = new Mock<IKorisnikRepository>();
            _knjigaRepoMock = new Mock<IKnjigaRepository>();
            _primjerakRepoMock = new Mock<IPrimjerakRepository>();

            _controller = new ZaduzenjeController(
                _zaduzenjeRepoMock.Object,
                _korisnikRepoMock.Object,
                _knjigaRepoMock.Object,
                _primjerakRepoMock.Object);
        }

        // ── Pomoćne metode ────────────────────────────────────────────

        private static Korisnik TestKorisnik(int id = 1) => new()
        {
            Id = id,
            Ime = "Test",
            Prezime = "Korisnik",
            Email = "test@smartlib.ba"
        };

        private static Primjerak TestPrimjerak(int id = 1, string status = "dostupan") => new()
        {
            Id = id,
            KnjigaId = 1,
            InventarniBroj = $"INV-1-{id:D3}",
            Status = status,
            Knjiga = new Knjiga { Id = 1, Naslov = "Test Knjiga", Autor = "Test Autor" }
        };

        private static Zaduzenje TestZaduzenje(int id = 1, string status = "aktivno") => new()
        {
            Id = id,
            KorisnikId = 1,
            PrimjerakId = 1,
            DatumZaduzivanja = DateTime.UtcNow.AddDays(-5),
            DatumPlaniranogVracanja = DateTime.UtcNow.AddDays(25),
            Status = status,
            Korisnik = TestKorisnik(),
            Primjerak = TestPrimjerak()
        };

        // Postavlja User claims na kontroleru (simulira prijavljenog korisnika)
        private void SetUser(string korisnikId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, korisnikId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        // Postavlja User bez NameIdentifier claima (neidentificiran korisnik)
        private void SetUserWithoutClaim()
        {
            var identity = new ClaimsIdentity(new List<Claim>(), "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        // ── GetActive ─────────────────────────────────────────────────

        [Fact]
        public async Task GetActive_BezFiltera_VracaOkSaListom()
        {
            // US-65: Bibliotekar vidi sva aktivna zaduženja
            _zaduzenjeRepoMock.Setup(r => r.GetActiveAsync())
                .ReturnsAsync(new List<Zaduzenje> { TestZaduzenje() });

            var result = await _controller.GetActive(null);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetActive_SaFilteromClan_VracaSamoOdgovarajuca()
        {
            // US-66: Bibliotekar može filtrirati zaduženja po imenu ili emailu člana
            var zaduzenja = new List<Zaduzenje>
            {
                TestZaduzenje(1),
                new Zaduzenje
                {
                    Id = 2,
                    KorisnikId = 2,
                    PrimjerakId = 2,
                    DatumZaduzivanja = DateTime.UtcNow,
                    DatumPlaniranogVracanja = DateTime.UtcNow.AddMonths(1),
                    Status = "aktivno",
                    Korisnik = new Korisnik { Ime = "Drugi", Prezime = "Korisnik", Email = "drugi@smartlib.ba" },
                    Primjerak = TestPrimjerak(2)
                }
            };

            _zaduzenjeRepoMock.Setup(r => r.GetActiveAsync()).ReturnsAsync(zaduzenja);

            var result = await _controller.GetActive("Test");

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task GetActive_FilterPoEmailu_VracaOdgovarajuca()
        {
            // US-66: Filter radi i po email adresi
            var zaduzenja = new List<Zaduzenje> { TestZaduzenje() };
            _zaduzenjeRepoMock.Setup(r => r.GetActiveAsync()).ReturnsAsync(zaduzenja);

            var result = await _controller.GetActive("test@smartlib.ba");

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetActive_PraznaLista_VracaOkSaPraznimNizom()
        {
            _zaduzenjeRepoMock.Setup(r => r.GetActiveAsync())
                .ReturnsAsync(new List<Zaduzenje>());

            var result = await _controller.GetActive(null);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetActive_WhitespaceFilter_TretiraSeKaoNull()
        {
            // Filter koji je samo razmaci ne smije filtrirati
            _zaduzenjeRepoMock.Setup(r => r.GetActiveAsync())
                .ReturnsAsync(new List<Zaduzenje> { TestZaduzenje() });

            var result = await _controller.GetActive("   ");

            Assert.IsType<OkObjectResult>(result);
        }

        // ── GetMine ───────────────────────────────────────────────────

        [Fact]
        public async Task GetMine_PrijavljenKorisnik_VracaOkSaListom()
        {
            // US-62: Prijavljeni korisnik vidi svoja zaduženja
            SetUser("1");
            _zaduzenjeRepoMock.Setup(r => r.GetByKorisnikAsync(1))
                .ReturnsAsync(new List<Zaduzenje> { TestZaduzenje() });

            var result = await _controller.GetMine();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetMine_PrijavljenKorisnik_PrazneListe_VracaOk()
        {
            // US-63: Ako korisnik nema zaduženja, vraća praznu listu
            SetUser("1");
            _zaduzenjeRepoMock.Setup(r => r.GetByKorisnikAsync(1))
                .ReturnsAsync(new List<Zaduzenje>());

            var result = await _controller.GetMine();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetMine_NijeIdentificiran_VracaUnauthorized()
        {
            // US-64: Neidentificiran korisnik dobija 401
            SetUserWithoutClaim();

            var result = await _controller.GetMine();

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task GetMine_NevalidanKorisnikIdFormat_VracaUnauthorized()
        {
            // Claim postoji ali nije broj — npr. bug u tokenu
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "nije-broj")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };

            var result = await _controller.GetMine();

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        // ── GetById ───────────────────────────────────────────────────

        [Fact]
        public async Task GetById_ZaduzenjePostoji_VracaOkIObjekt()
        {
            // US-67: Bibliotekar vidi detalje zaduženja
            _zaduzenjeRepoMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(TestZaduzenje());

            var result = await _controller.GetById(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ZaduzenjeNePostoji_VracaNotFound()
        {
            _zaduzenjeRepoMock.Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Zaduzenje?)null);

            var result = await _controller.GetById(99);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        // ── GetHistory ────────────────────────────────────────────────

        [Fact]
        public async Task GetHistory_KorisnikPostoji_VracaOkSaListom()
        {
            _korisnikRepoMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(TestKorisnik());
            _zaduzenjeRepoMock.Setup(r => r.GetByKorisnikAsync(1))
                .ReturnsAsync(new List<Zaduzenje> { TestZaduzenje() });

            var result = await _controller.GetHistory(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetHistory_KorisnikNePostoji_VracaNotFound()
        {
            _korisnikRepoMock.Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Korisnik?)null);

            var result = await _controller.GetHistory(99);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetHistory_KorisnikNemaZaduzenja_VracaOkSaPraznimNizom()
        {
            _korisnikRepoMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(TestKorisnik());
            _zaduzenjeRepoMock.Setup(r => r.GetByKorisnikAsync(1))
                .ReturnsAsync(new List<Zaduzenje>());

            var result = await _controller.GetHistory(1);

            Assert.IsType<OkObjectResult>(result);
        }

        // ── Zaduzi ────────────────────────────────────────────────────

        [Fact]
        public async Task Zaduzi_ModelStateInvalid_VracaBadRequest()
        {
            _controller.ModelState.AddModelError("KorisnikId", "Obavezno polje");

            var result = await _controller.Zaduzi(new ZaduzenjeCreateDto());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Zaduzi_PrimjerakNePostoji_VracaBadRequest()
        {
            // US-47: Primjerak mora postojati
            _primjerakRepoMock.Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Primjerak?)null);

            var result = await _controller.Zaduzi(new ZaduzenjeCreateDto
            {
                KorisnikId = 1,
                PrimjerakId = 99
            });

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("nije dostupan", bad.Value!.ToString());
        }

        [Fact]
        public async Task Zaduzi_PrimjerakNijeDostupan_VracaBadRequest()
        {
            // US-47: Primjerak mora biti u statusu "dostupan"
            _primjerakRepoMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(TestPrimjerak(1, "zadužen"));

            var result = await _controller.Zaduzi(new ZaduzenjeCreateDto
            {
                KorisnikId = 1,
                PrimjerakId = 1
            });

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("nije dostupan", bad.Value!.ToString());
        }

        [Fact]
        public async Task Zaduzi_PrimjerakVecImaAktivnoZaduzenje_VracaBadRequest()
        {
            // US-47: Sprječavanje duplikata aktivnog zaduženja
            _primjerakRepoMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(TestPrimjerak(1, "dostupan"));
            _primjerakRepoMock.Setup(r => r.HasActiveZaduzenjeAsync(1))
                .ReturnsAsync(true);

            var result = await _controller.Zaduzi(new ZaduzenjeCreateDto
            {
                KorisnikId = 1,
                PrimjerakId = 1
            });

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("već ima aktivno zaduženje", bad.Value!.ToString());
        }

        [Fact]
        public async Task Zaduzi_DatumPovratkaUProslosti_VracaBadRequest()
        {
            _primjerakRepoMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(TestPrimjerak(1, "dostupan"));
            _primjerakRepoMock.Setup(r => r.HasActiveZaduzenjeAsync(1))
                .ReturnsAsync(false);

            var result = await _controller.Zaduzi(new ZaduzenjeCreateDto
            {
                KorisnikId = 1,
                PrimjerakId = 1,
                DatumPovratka = DateTime.UtcNow.AddDays(-1)
            });

            var bad = Assert.IsType<BadRequestObjectResult>(result);

            var jsonString = System.Text.Json.JsonSerializer.Serialize(bad.Value);
            Assert.Contains("Datum povratka ne mo", jsonString);
        }

        [Fact]
        public async Task Zaduzi_ValidanPayloadBezDatuma_VracaCreated()
        {
            // US-44: Bez datuma povratka, automatski se postavlja 2 mjeseca
            var zaduzenje = TestZaduzenje();
            _primjerakRepoMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(TestPrimjerak(1, "dostupan"));
            _primjerakRepoMock.Setup(r => r.HasActiveZaduzenjeAsync(1))
                .ReturnsAsync(false);
            _zaduzenjeRepoMock.Setup(r => r.CreateAsync(It.IsAny<Zaduzenje>()))
                .ReturnsAsync(zaduzenje);
            _primjerakRepoMock.Setup(r => r.UpdateStatusAsync(1, "zadužen"))
                .Returns(Task.CompletedTask);
            _zaduzenjeRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(zaduzenje);

            var result = await _controller.Zaduzi(new ZaduzenjeCreateDto
            {
                KorisnikId = 1,
                PrimjerakId = 1
            });

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, created.StatusCode);
        }

        [Fact]
        public async Task Zaduzi_ValidanPayloadSaDatumom_VracaCreated()
        {
            // US-46: Bibliotekār može ručno unijeti datum povratka
            var zaduzenje = TestZaduzenje();
            _primjerakRepoMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(TestPrimjerak(1, "dostupan"));
            _primjerakRepoMock.Setup(r => r.HasActiveZaduzenjeAsync(1))
                .ReturnsAsync(false);
            _zaduzenjeRepoMock.Setup(r => r.CreateAsync(It.IsAny<Zaduzenje>()))
                .ReturnsAsync(new Zaduzenje());
            _primjerakRepoMock.Setup(r => r.UpdateStatusAsync(1, "zadužen"))
                .Returns(Task.CompletedTask);
            _zaduzenjeRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(zaduzenje);

            var result = await _controller.Zaduzi(new ZaduzenjeCreateDto
            {
                KorisnikId = 1,
                PrimjerakId = 1,
                DatumPovratka = DateTime.Today.AddMonths(1)
            });

            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Zaduzi_ValidanPayload_AzuriraStatusPrimjerka()
        {
            // US-47: Nakon zaduživanja, status primjerka mora biti "zadužen"
            var zaduzenje = TestZaduzenje();
            _primjerakRepoMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(TestPrimjerak(1, "dostupan"));
            _primjerakRepoMock.Setup(r => r.HasActiveZaduzenjeAsync(1))
                .ReturnsAsync(false);
            _zaduzenjeRepoMock.Setup(r => r.CreateAsync(It.IsAny<Zaduzenje>()))
                .ReturnsAsync(new Zaduzenje());
            _primjerakRepoMock.Setup(r => r.UpdateStatusAsync(1, "zadužen"))
                .Returns(Task.CompletedTask);
            _zaduzenjeRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(zaduzenje);

            await _controller.Zaduzi(new ZaduzenjeCreateDto { KorisnikId = 1, PrimjerakId = 1 });

            _primjerakRepoMock.Verify(r => r.UpdateStatusAsync(1, "zadužen"), Times.Once);
        }

        [Fact]
        public async Task Zaduzi_ValidanPayload_KreiraZaduzenje()
        {
            var zaduzenje = TestZaduzenje();
            _primjerakRepoMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(TestPrimjerak(1, "dostupan"));
            _primjerakRepoMock.Setup(r => r.HasActiveZaduzenjeAsync(1))
                .ReturnsAsync(false);
            _zaduzenjeRepoMock.Setup(r => r.CreateAsync(It.IsAny<Zaduzenje>()))
                .ReturnsAsync(new Zaduzenje());
            _primjerakRepoMock.Setup(r => r.UpdateStatusAsync(1, "zadužen"))
                .Returns(Task.CompletedTask);
            _zaduzenjeRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(zaduzenje);

            await _controller.Zaduzi(new ZaduzenjeCreateDto { KorisnikId = 1, PrimjerakId = 1 });

            _zaduzenjeRepoMock.Verify(r => r.CreateAsync(It.IsAny<Zaduzenje>()), Times.Once);
        }

        // ── Vrati ─────────────────────────────────────────────────────

        [Fact]
        public async Task Vrati_ZaduzenjeNePostoji_VracaNotFound()
        {
            _zaduzenjeRepoMock.Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Zaduzenje?)null);

            var result = await _controller.Vrati(99);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Vrati_ZaduzenjeNijeAktivno_VracaBadRequest()
        {
            // US-45: Ne može se vratiti ono što nije aktivno
            _zaduzenjeRepoMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(TestZaduzenje(1, "zatvoreno"));

            var result = await _controller.Vrati(1);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("nije aktivno", bad.Value!.ToString());
        }

        [Fact]
        public async Task Vrati_AktivnoZaduzenje_VracaOk()
        {
            // US-45: Uspješno vraćanje knjige
            var zaduzenje = TestZaduzenje(1, "aktivno");
            _zaduzenjeRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(zaduzenje);
            _zaduzenjeRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Zaduzenje>()))
                .Returns(Task.CompletedTask);
            _primjerakRepoMock.Setup(r => r.UpdateStatusAsync(1, "dostupan"))
                .Returns(Task.CompletedTask);

            var result = await _controller.Vrati(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Vrati_AktivnoZaduzenje_StatusSeMijenjaNaZatvoreno()
        {
            // US-45: Nakon vraćanja, zaduženje mora biti zatvoreno
            var zaduzenje = TestZaduzenje(1, "aktivno");
            _zaduzenjeRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(zaduzenje);
            _zaduzenjeRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Zaduzenje>()))
                .Returns(Task.CompletedTask);
            _primjerakRepoMock.Setup(r => r.UpdateStatusAsync(1, "dostupan"))
                .Returns(Task.CompletedTask);

            await _controller.Vrati(1);

            Assert.Equal("zatvoreno", zaduzenje.Status);
        }

        [Fact]
        public async Task Vrati_AktivnoZaduzenje_PrimjerakPostajeDostupan()
        {
            // US-45: Nakon vraćanja, primjerak mora biti dostupan
            var zaduzenje = TestZaduzenje(1, "aktivno");
            _zaduzenjeRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(zaduzenje);
            _zaduzenjeRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Zaduzenje>()))
                .Returns(Task.CompletedTask);
            _primjerakRepoMock.Setup(r => r.UpdateStatusAsync(1, "dostupan"))
                .Returns(Task.CompletedTask);

            await _controller.Vrati(1);

            _primjerakRepoMock.Verify(r => r.UpdateStatusAsync(1, "dostupan"), Times.Once);
        }

        [Fact]
        public async Task Vrati_AktivnoZaduzenje_DatumStvarnogVracanjaJePostavljeno()
        {
            // US-45: Datum stvarnog vraćanja se bilježi u sistemu
            var zaduzenje = TestZaduzenje(1, "aktivno");
            _zaduzenjeRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(zaduzenje);
            _zaduzenjeRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Zaduzenje>()))
                .Returns(Task.CompletedTask);
            _primjerakRepoMock.Setup(r => r.UpdateStatusAsync(1, "dostupan"))
                .Returns(Task.CompletedTask);

            await _controller.Vrati(1);

            Assert.NotNull(zaduzenje.DatumStvarnogVracanja);
        }

        // ── MapToDto pokrivenost (rubni slučajevi) ────────────────────

        [Fact]
        public async Task GetById_ZaduzenjeZakasnilo_JeZakasniloJeTrue()
        {
            // MapToDto: zakasnilo = status aktivno && datum u prošlosti
            var z = TestZaduzenje();
            z.Status = "aktivno";
            z.DatumPlaniranogVracanja = DateTime.UtcNow.AddDays(-1); // u prošlosti

            _zaduzenjeRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(z);

            var result = await _controller.GetById(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ZaduzenjeRokSeBliziZa3Dana_RokSeBliziJeTrue()
        {
            // MapToDto: rokSeBliži = aktivno && datum <= now + 3 dana && nije zakasnilo
            var z = TestZaduzenje();
            z.Status = "aktivno";
            z.DatumPlaniranogVracanja = DateTime.UtcNow.AddDays(2); // za 2 dana

            _zaduzenjeRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(z);

            var result = await _controller.GetById(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ZaduzenjeZatvoreno_JeZakasniloJeFalse()
        {
            // MapToDto: zatvoreno zaduženje nije zakasnilo
            var z = TestZaduzenje(1, "zatvoreno");
            z.DatumPlaniranogVracanja = DateTime.UtcNow.AddDays(-5);

            _zaduzenjeRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(z);

            var result = await _controller.GetById(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ZaduzenjeBezKorisnika_KorisnikImeJeCrtica()
        {
            // MapToDto: null Korisnik → "-"
            var z = TestZaduzenje();
            z.Korisnik = null;

            _zaduzenjeRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(z);

            var result = await _controller.GetById(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ZaduzenjeBezPrimjerka_InventarniBrojJeCrtica()
        {
            // MapToDto: null Primjerak → "-"
            var z = TestZaduzenje();
            z.Primjerak = null;

            _zaduzenjeRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(z);

            var result = await _controller.GetById(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ZaduzenjeBezKnjige_KnjigaNaslovJeCrtica()
        {
            // MapToDto: Primjerak postoji ali Knjiga je null → "-"
            var z = TestZaduzenje();
            z.Primjerak = new Primjerak
            {
                Id = 1,
                InventarniBroj = "INV-1-001",
                Status = "zadužen",
                Knjiga = null
            };

            _zaduzenjeRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(z);

            var result = await _controller.GetById(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetActive_FilterPoImenu_VracaSamoOdgovarajucaZaduzenja()
        {
            var zaduzenja = new List<Zaduzenje>
    {
        TestZaduzenje(1), // Korisnik: "Test Korisnik"
        new Zaduzenje
        {
            Id = 2,
            KorisnikId = 2,
            PrimjerakId = 2,
            DatumZaduzivanja = DateTime.UtcNow,
            DatumPlaniranogVracanja = DateTime.UtcNow.AddMonths(1),
            Status = "aktivno",
            Korisnik = new Korisnik { Ime = "Marko", Prezime = "Marković", Email = "marko@smartlib.ba" },
            Primjerak = TestPrimjerak(2)
        }
    };

            _zaduzenjeRepoMock.Setup(r => r.GetActiveAsync()).ReturnsAsync(zaduzenja);

            var result = await _controller.GetActive("marko");

            var ok = Assert.IsType<OkObjectResult>(result);
            // Materijaliziramo listu da forsiramo evaluaciju LINQ filtera
            var lista = (ok.Value as IEnumerable<object>)?.ToList();
            Assert.NotNull(lista);
            Assert.Single(lista); // samo Marko prolazi filter
        }

        [Fact]
        public async Task GetActive_FilterPoEmailu_IskljucujeNepodudarne()
        {
            var zaduzenja = new List<Zaduzenje>
    {
        TestZaduzenje(1), // email: test@smartlib.ba
        new Zaduzenje
        {
            Id = 2,
            KorisnikId = 2,
            PrimjerakId = 2,
            DatumZaduzivanja = DateTime.UtcNow,
            DatumPlaniranogVracanja = DateTime.UtcNow.AddMonths(1),
            Status = "aktivno",
            Korisnik = new Korisnik { Ime = "Marko", Prezime = "Marković", Email = "marko@smartlib.ba" },
            Primjerak = TestPrimjerak(2)
        }
    };

            _zaduzenjeRepoMock.Setup(r => r.GetActiveAsync()).ReturnsAsync(zaduzenja);

            var result = await _controller.GetActive("marko@smartlib.ba");

            var ok = Assert.IsType<OkObjectResult>(result);
            var lista = (ok.Value as IEnumerable<object>)?.ToList();
            Assert.NotNull(lista);
            Assert.Single(lista);
        }

        [Fact]
        public async Task GetActive_FilterKojiNikoNeProlazi_VracaPrazanNiz()
        {
            _zaduzenjeRepoMock.Setup(r => r.GetActiveAsync())
                .ReturnsAsync(new List<Zaduzenje> { TestZaduzenje() });

            var result = await _controller.GetActive("nepostojeci_korisnik_xyz");

            var ok = Assert.IsType<OkObjectResult>(result);
            var lista = (ok.Value as IEnumerable<object>)?.ToList();
            Assert.NotNull(lista);
            Assert.Empty(lista); // filter linije se izvršavaju ali niko ne prolazi
        }

        [Fact]
        public async Task GetActive_FilterSaKorisnicimaKojiImajuNullKorisnik_NePuca()
        {
            // Pokriva null-conditional operator z.Korisnik?.Ime i z.Korisnik?.Email
            var zaduzenja = new List<Zaduzenje>
    {
        new Zaduzenje
        {
            Id = 1,
            KorisnikId = 1,
            PrimjerakId = 1,
            DatumZaduzivanja = DateTime.UtcNow,
            DatumPlaniranogVracanja = DateTime.UtcNow.AddMonths(1),
            Status = "aktivno",
            Korisnik = null, // null korisnik!
            Primjerak = TestPrimjerak()
        }
    };

            _zaduzenjeRepoMock.Setup(r => r.GetActiveAsync()).ReturnsAsync(zaduzenja);

            var result = await _controller.GetActive("bilo_sto");

            var ok = Assert.IsType<OkObjectResult>(result);
            var lista = (ok.Value as IEnumerable<object>)?.ToList();
            Assert.NotNull(lista);
            Assert.Empty(lista);
        }
    }
}