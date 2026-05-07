using Microsoft.AspNetCore.Mvc;
using Moq;
using SmartLib.API.Controllers;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using Xunit;
using System.ComponentModel.DataAnnotations;

namespace SmartLib.Tests.Unit.APITests
{
    /// <summary>
    /// PB-18: Kreiranje naloga člana
    /// US-01 i US-02: Validacija forme pri registraciji:
    ///     - Sva polja su obavezna (Ime, Prezime, Email, Lozinka)
    ///     - Lozinka mora imati najmanje 8 znakova
    ///     - Email mora biti u ispravnom formatu (mora sadržavati @)
    ///     - Email treba biti jedinstven u sistemu
    /// US-03: Uloga Član se automatski dodjeljuje
    /// US-09: Deaktivacija korisnika
    /// </summary>
    /// 
    public class KorisnikApiControllerTests
    {
        private readonly Mock<IKorisnikRepository> _repoMock;
        private readonly KorisnikController _controller;

        public KorisnikApiControllerTests()
        {
            _repoMock = new Mock<IKorisnikRepository>();
            _controller = new KorisnikController(_repoMock.Object);
        }

        // Pomoćna metoda

        private static IList<ValidationResult> ValidirajDto(object dto)
        {
            var rezultati = new List<ValidationResult>();
            var kontekst = new ValidationContext(dto);
            Validator.TryValidateObject(dto, kontekst, rezultati, validateAllProperties: true);
            return rezultati;
        }

        // US-01: Uspješno kreiranje

        [Fact]
        public async Task Create_ValidanModel_VracaCreated201()
        {
            var dto = new KorisnikCreateDto
            {
                Ime = "Ime",
                Prezime = "Prezime",
                Email = "imeprezime@smartlib.ba",
                Lozinka = "Lozinka1!",
                PotvrdaLozinke = "Lozinka1!"
            };

            _repoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync((Korisnik?)null);
            _repoMock.Setup(r => r.CreateAsync(It.IsAny<Korisnik>()))
                     .ReturnsAsync((Korisnik k) => { k.Id = 10; return k; });

            var result = await _controller.Create(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(201, created.StatusCode);
        }

        // US-03: Uloga se automatski postavlja na "Član" 

        [Fact]
        public async Task Create_NoviKorisnik_UlogaIdJe1Clan()
        {
            Korisnik? sacuvani = null;

            _repoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((Korisnik?)null);
            _repoMock.Setup(r => r.CreateAsync(It.IsAny<Korisnik>()))
                     .Callback<Korisnik>(k => sacuvani = k)
                     .ReturnsAsync((Korisnik k) => k);

            await _controller.Create(new KorisnikCreateDto
            {
                Ime = "Test",
                Prezime = "Clan",
                Email = "noviClan@smartlib.ba",
                Lozinka = "Lozinka1!",
                PotvrdaLozinke = "Lozinka1!"
            });

            Assert.NotNull(sacuvani);
            Assert.Equal(1, sacuvani!.UlogaId);
            Assert.Equal("aktivan", sacuvani.Status);
        }

        //  US-02: Duplikat email 

        [Fact]
        public async Task Create_DuplikatEmail_VracaValidationProblem()
        {
            _repoMock.Setup(r => r.GetByEmailAsync("postojeci@smartlib.ba"))
                     .ReturnsAsync(new Korisnik { Email = "postojeci@smartlib.ba" });

            var result = await _controller.Create(new KorisnikCreateDto
            {
                Ime = "Novi",
                Prezime = "Korisnik",
                Email = "postojeci@smartlib.ba",
                Lozinka = "Lozinka1!",
                PotvrdaLozinke = "Lozinka1!"
            });

            var obj = result.Result as ObjectResult;
            Assert.NotNull(obj);
            Assert.Equal(null, obj!.StatusCode);
        }

        // Lozinka se hešira — ne čuva se u čistom tekstu

        [Fact]
        public async Task Create_LozinkaSeHashuje_NijeChuvanaKaoPlainText()
        {
            Korisnik? sacuvani = null;
            const string plainLozinka = "Lozinka1!";

            _repoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((Korisnik?)null);
            _repoMock.Setup(r => r.CreateAsync(It.IsAny<Korisnik>()))
                     .Callback<Korisnik>(k => sacuvani = k)
                     .ReturnsAsync((Korisnik k) => k);

            await _controller.Create(new KorisnikCreateDto
            {
                Ime = "Test",
                Prezime = "Hash",
                Email = "hash@smartlib.ba",
                Lozinka = plainLozinka,
                PotvrdaLozinke = plainLozinka
            });

            Assert.NotNull(sacuvani);
            Assert.NotEqual(plainLozinka, sacuvani!.LozinkaHash);
            Assert.NotEmpty(sacuvani.LozinkaHash);
        }

        // US-09: Deaktivacija 

        [Fact]
        public async Task Deactivate_PostojeciKorisnik_SetujujeStatusNaDeaktiviran()
        {
            var korisnik = new Korisnik { Id = 5, Status = "aktivan" };
            _repoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(korisnik);
            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Korisnik>())).Returns(Task.CompletedTask);

            var result = await _controller.Deactivate(5);

            Assert.IsType<NoContentResult>(result);
            Assert.Equal("deaktiviran", korisnik.Status);
        }

        [Fact]
        public async Task Deactivate_NepostojećiId_VracaNotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Korisnik?)null);

            var result = await _controller.Deactivate(999);

            Assert.IsType<NotFoundResult>(result);
        }

        // US-01: GetAll i GetById

        [Fact]
        public async Task GetAll_VracaListuKorisnika()
        {
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Korisnik>
            {
                new() { Id = 1, Ime = "A", Prezime = "B", Email = "a@b.com",
                        Status = "aktivan", Uloga = new Uloga { Naziv = "Član" } }
            });

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var lista = Assert.IsAssignableFrom<IEnumerable<KorisnikDto>>(ok.Value);
            Assert.Single(lista);
        }

        [Fact]
        public async Task GetById_NepostojeciId_VracaNotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Korisnik?)null);

            var result = await _controller.GetById(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        // US-02: VALIDACIJA OBAVEZNIH POLJA
        // Svaki test provjerava jedno polje — prazan string aktivira [Required]
        [Fact]
        public void Validacija_PraznoIme_VracaGresku()
        {
            var dto = new KorisnikCreateDto
            {
                Ime = "",                  // testiranje registracije bez imena
                Prezime = "Prezime",
                Email = "prezime@smartlib.ba",
                Lozinka = "Lozinka1!",
                PotvrdaLozinke = "Lozinka1!"
            };

            var greske = ValidirajDto(dto);

            Assert.Contains(greske, g => g.MemberNames.Contains(nameof(dto.Ime)));
        }

        [Fact]
        public void Validacija_PraznoPrezime_VracaGresku()
        {
            var dto = new KorisnikCreateDto
            {
                Ime = "Ime",
                Prezime = "",                  // testiranje registracije bez prezimena
                Email = "ime@smartlib.ba",
                Lozinka = "Lozinka1!",
                PotvrdaLozinke = "Lozinka1!"
            };

            var greske = ValidirajDto(dto);

            Assert.Contains(greske, g => g.MemberNames.Contains(nameof(dto.Prezime)));
        }

        [Fact]
        public void Validacija_PrazanEmail_VracaGresku()
        {
            var dto = new KorisnikCreateDto
            {
                Ime = "Ime",
                Prezime = "Prezime",
                Email = "",                   // testiranje registracije bez emaila
                Lozinka = "Lozinka1!",
                PotvrdaLozinke = "Lozinka1!"
            };

            var greske = ValidirajDto(dto);

            Assert.Contains(greske, g => g.MemberNames.Contains(nameof(dto.Email)));
        }

        [Fact]
        public void Validacija_PraznaLozinka_VracaGresku()
        {
            var dto = new KorisnikCreateDto
            {
                Ime = "Ime",
                Prezime = "Prezime",
                Email = "imeprezime@smartlib.ba",
                Lozinka = "",                    // testiranje registracije bez lozinke
                PotvrdaLozinke = ""
            };

            var greske = ValidirajDto(dto);

            Assert.Contains(greske, g => g.MemberNames.Contains(nameof(dto.Lozinka)));
        }

        // US-02: VALIDACIJA MINIMALNE DUŽINE LOZINKE (MinLength = 8)

        [Fact]
        public void Validacija_LozinkaKracaOd8Znakova_VracaGresku()
        {
            // 7 znakova — mora biti odbijena
            var dto = new KorisnikCreateDto
            {
                Ime = "Ime",
                Prezime = "Prezime",
                Email = "ime@smartlib.ba",
                Lozinka = "Abc1!xy",
                PotvrdaLozinke = "Abc1!xy"
            };

            var greske = ValidirajDto(dto);

            Assert.Contains(greske, g => g.MemberNames.Contains(nameof(dto.Lozinka)));
        }

        [Fact]
        public void Validacija_LozinkaSa1Znakom_VracaGresku()
        {
            // Granični slučaj — jedna jedina cifra
            var dto = new KorisnikCreateDto
            {
                Ime = "Ime",
                Prezime = "Prezime",
                Email = "ime@smartlib.ba",
                Lozinka = "A",                   // 1 znak
                PotvrdaLozinke = "A"
            };

            var greske = ValidirajDto(dto);

            Assert.Contains(greske, g => g.MemberNames.Contains(nameof(dto.Lozinka)));
        }

        [Fact]
        public void Validacija_LozinkaTacno8Znakova_JeValidna()
        {
            // Granični slučaj — tačno 8 znakova mora proći
            var dto = new KorisnikCreateDto
            {
                Ime = "Ime",
                Prezime = "Prezime",
                Email = "ime@smartlib.ba",
                Lozinka = "Abcde1!x",            // tačno 8 znakova
                PotvrdaLozinke = "Abcde1!x"
            };

            var greske = ValidirajDto(dto);

            Assert.DoesNotContain(greske, g => g.MemberNames.Contains(nameof(dto.Lozinka)));
        }

        // US-02: VALIDACIJA FORMATA EMAIL ADRESE ([EmailAddress] anotacija)
        // Anotacija provjerava da postoji @ i ispravna domena 

        [Fact]
        public void Validacija_EmailBezAtZnaka_VracaGresku()
        {
            var dto = new KorisnikCreateDto
            {
                Ime = "Ime",
                Prezime = "Prezime",
                Email = "imesmartlib.ba",
                Lozinka = "Lozinka1!",
                PotvrdaLozinke = "Lozinka1!"
            };

            var greske = ValidirajDto(dto);

            Assert.Contains(greske, g => g.MemberNames.Contains(nameof(dto.Email)));
        }

        [Fact]
        public void Validacija_EmailBezDomene_VracaGresku()
        {
            var dto = new KorisnikCreateDto
            {
                Ime = "Ime",
                Prezime = "Prezime",
                Email = "ime@",              // @ postoji ali nema domene
                Lozinka = "Lozinka1!",
                PotvrdaLozinke = "Lozinka1!"
            };

            var greske = ValidirajDto(dto);

            Assert.Contains(greske, g => g.MemberNames.Contains(nameof(dto.Email)));
        }

        [Fact]
        public async Task Create_ImeSadrziHtml_VracaBadRequest()
        {
            var dto = new KorisnikCreateDto
            {
                Ime = "<b>Opasno Ime</b>",
                Prezime = "Prezime",
                Email = "xss@smartlib.ba",
                Lozinka = "Lozinka1!",
                PotvrdaLozinke = "Lozinka1!"
            };

            var result = await _controller.Create(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("ne smije sadržavati HTML tagove", badRequest.Value.ToString());
        }

        [Fact]
        public async Task Create_PrezimeSadrziHtml_VracaBadRequest()
        {
            var dto = new KorisnikCreateDto
            {
                Ime = "Ime",
                Prezime = "<script>alert(1)</script>",
                Email = "xss2@smartlib.ba",
                Lozinka = "Lozinka1!",
                PotvrdaLozinke = "Lozinka1!"
            };

            var result = await _controller.Create(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("ne smije sadržavati HTML tagove", badRequest.Value.ToString());
        }

        [Fact]
        public async Task Create_ModelStateInvalid_VracaValidationProblem()
        {
            _controller.ModelState.AddModelError("Email", "Neispravan email");

            var result = await _controller.Create(new KorisnikCreateDto());

            var objectResult = result.Result as ObjectResult;
            Assert.NotNull(objectResult);

            Assert.IsAssignableFrom<ValidationProblemDetails>(objectResult.Value);
        }

        [Fact]
        public async Task GetById_KorisnikNemaUlogu_VracaEmptyStringZaUlogu()
        {
            // Scenario gdje navigacijsko svojstvo Uloga nije učitano
            var korisnik = new Korisnik
            {
                Id = 1,
                Ime = "Test",
                Prezime = "Test",
                Email = "test@test.com",
                Uloga = null
            };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(korisnik);

            var result = await _controller.GetById(1);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<KorisnikDto>(ok.Value);
            Assert.Equal(string.Empty, dto.Uloga);
        }

        [Fact]
        public void ChangeRole_VracaOkSaTodoPorukom()
        {
            var result = _controller.ChangeRole(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("TODO", ok.Value.ToString());
        }

        // Validacija da se lozinke moraju poklapati

        [Fact]
        public void Validacija_PotvrdaLozinkeNePoklapa_VracaGresku()
        {
            var dto = new KorisnikCreateDto
            {
                Ime = "Ime",
                Prezime = "Prezime",
                Email = "ime@smartlib.ba",
                Lozinka = "Lozinka1!",
                PotvrdaLozinke = "DrugaLozinka!"
            };

            var greske = ValidirajDto(dto);

            Assert.Contains(greske, g => g.MemberNames.Contains(nameof(dto.PotvrdaLozinke)));
        }

        [Fact]
        public void Validacija_PotvrdaLozinkePrazna_VracaGresku()
        {
            var dto = new KorisnikCreateDto
            {
                Ime = "Ime",
                Prezime = "Prezime",
                Email = "ime@smartlib.ba",
                Lozinka = "Lozinka1!",
                PotvrdaLozinke = ""
            };

            var greske = ValidirajDto(dto);

            Assert.Contains(greske, g => g.MemberNames.Contains(nameof(dto.PotvrdaLozinke)));
        }

        [Fact]
        public void Validacija_PotvrdaLozinkePoklapa_BezGreske()
        {
            var dto = new KorisnikCreateDto
            {
                Ime = "Ime",
                Prezime = "Prezime",
                Email = "ime@smartlib.ba",
                Lozinka = "Lozinka1!",
                PotvrdaLozinke = "Lozinka1!"
            };

            var greske = ValidirajDto(dto);

            Assert.DoesNotContain(greske, g => g.MemberNames.Contains(nameof(dto.PotvrdaLozinke)));
        }

        [Fact]
        public async Task Create_LozinkeSeNePoklapaju_VracaValidationProblem()
        {
            var dto = new KorisnikCreateDto
            {
                Ime = "Ime",
                Prezime = "Prezime",
                Email = "test@smartlib.ba",
                Lozinka = "Lozinka1!",
                PotvrdaLozinke = "DrugaLozinka!"
            };

            var result = await _controller.Create(dto);

            var objectResult = result.Result as ObjectResult;
            Assert.NotNull(objectResult);
            Assert.True(_controller.ModelState.ContainsKey("PotvrdaLozinke"));
        }
    }
}