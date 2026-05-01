using Microsoft.AspNetCore.Mvc;
using Moq;
using SmartLib.API.Controllers;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using Xunit;
using System.ComponentModel.DataAnnotations;

namespace SmartLib.Tests.Unit
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

        // Pomoćna metoda za DTO validaciju

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
                Lozinka = "Lozinka1!"
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
                Lozinka = "Lozinka1!"
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
                Lozinka = "Lozinka1!"
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
                Lozinka = plainLozinka
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
                Lozinka = "Lozinka1!"
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
                Lozinka = "Lozinka1!"
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
                Lozinka = "Lozinka1!"
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
                Lozinka = ""                    // testiranje registracije bez lozinke
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
                Ime = "Azra",
                Prezime = "Kovač",
                Email = "azra@smartlib.ba",
                Lozinka = "Abc1!xy"             
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
                Ime = "Azra",
                Prezime = "Kovač",
                Email = "azra@smartlib.ba",
                Lozinka = "A"                   // 1 znak
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
                Ime = "Azra",
                Prezime = "Kovač",
                Email = "azra@smartlib.ba",
                Lozinka = "Abcde1!x"            // tačno 8 znakova
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
                Ime = "Azra",
                Prezime = "Kovač",
                Email = "azrasmartlib.ba",    
                Lozinka = "Lozinka1!"
            };

            var greske = ValidirajDto(dto);

            Assert.Contains(greske, g => g.MemberNames.Contains(nameof(dto.Email)));
        }

        [Fact]
        public void Validacija_EmailBezDomene_VracaGresku()
        {
            var dto = new KorisnikCreateDto
            {
                Ime = "Azra",
                Prezime = "Kovač",
                Email = "azra@",              // @ postoji ali nema domene
                Lozinka = "Lozinka1!"
            };

            var greske = ValidirajDto(dto);

            Assert.Contains(greske, g => g.MemberNames.Contains(nameof(dto.Email)));
        }     
    }
}