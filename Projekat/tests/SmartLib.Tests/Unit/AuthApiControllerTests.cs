using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using SmartLib.API.Controllers;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using Xunit;

namespace SmartLib.Tests.Unit
{
    /// <summary>
    /// PB-17: Sistem prijave korisnika:
    ///     US-04: Prijava korisnika
    ///     US-05: Poruke o neuspjehu prijave
    ///     US-06: Odjava korisnika
    ///     US-07: Čuvanje sesije — JWT token expiry
    ///     US-08: Zaštita ruta — Logout zahtijeva autentifikaciju
    ///     US-09: Deaktiviran korisnik ne može pristupiti sistemu
    /// </summary>
    public class AuthApiControllerTests
    {
        private readonly Mock<IKorisnikRepository> _repoMock;
        private readonly IConfiguration _configuration;
        private readonly AuthController _controller;

        // JWT postavke moraju biti iste u svim testovima
        private const string JwtKey = "SmartLibTestKeyKojiJeDovoljnoDugacak123!";
        private const string JwtIssuer = "SmartLib";
        private const string JwtAudience = "SmartLibUsers";
        private const int JwtMinutes = 60;

        public AuthApiControllerTests()
        {
            _repoMock = new Mock<IKorisnikRepository>();

            var inMemorySettings = new Dictionary<string, string>
            {
                { "Jwt:Key",               JwtKey                        },
                { "Jwt:Issuer",            JwtIssuer                     },
                { "Jwt:Audience",          JwtAudience                   },
                { "Jwt:ExpirationMinutes", JwtMinutes.ToString()         }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();

            _controller = new AuthController(_repoMock.Object, _configuration);
        }

        // Pomoćna metoda — standardni aktivni korisnik

        private static Korisnik PravianKorisnik(string uloga = "Član") => new()
        {
            Id = 1,
            Ime = "Test",
            Prezime = "Korisnik",
            Email = "test@smartlib.ba",
            LozinkaHash = SmartLib.Infrastructure.Security.PasswordHasher.HashPassword("Lozinka1!"),
            Status = "aktivan",
            Uloga = new Uloga { Id = 1, Naziv = uloga }
        };

        // Parsira JWT token iz stringa i vraća objekt za daljnju inspekciju
        private static JwtSecurityToken ParseToken(string tokenString)
        {
            return new JwtSecurityTokenHandler().ReadJwtToken(tokenString);
        }
     
        // US-04: USPJEŠNA PRIJAVA     

        [Fact]
        public async Task Login_ValidanEmailILozinka_VracaOkSaTokenom()
        {
            _repoMock.Setup(r => r.GetByEmailAsync("test@smartlib.ba"))
                     .ReturnsAsync(PravianKorisnik());

            var result = await _controller.Login(new LoginRequest
            {
                Email = "test@smartlib.ba",
                Lozinka = "Lozinka1!"
            });

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<LoginResponse>(ok.Value);
            Assert.NotEmpty(response.Token);
        }

        [Fact]
        public async Task Login_UspjesanLogin_ResponseSadrziIme()
        {
            _repoMock.Setup(r => r.GetByEmailAsync("test@smartlib.ba"))
                     .ReturnsAsync(PravianKorisnik());

            var result = await _controller.Login(new LoginRequest
            {
                Email = "test@smartlib.ba",
                Lozinka = "Lozinka1!"
            });

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<LoginResponse>(ok.Value);
            Assert.Equal("Test", response.Ime);
        }

        [Fact]
        public async Task Login_UspjesanLogin_ResponseSadrziPrezime()
        {
            // US-04: Response mora sadržavati prezime korisnika
            _repoMock.Setup(r => r.GetByEmailAsync("test@smartlib.ba"))
                     .ReturnsAsync(PravianKorisnik());

            var result = await _controller.Login(new LoginRequest
            {
                Email = "test@smartlib.ba",
                Lozinka = "Lozinka1!"
            });

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<LoginResponse>(ok.Value);
            Assert.Equal("Korisnik", response.Prezime);
        }

        [Fact]
        public async Task Login_UspjesanLogin_ResponseSadrziUlogu()
        {
            _repoMock.Setup(r => r.GetByEmailAsync("test@smartlib.ba"))
                     .ReturnsAsync(PravianKorisnik("Bibliotekar"));

            var result = await _controller.Login(new LoginRequest
            {
                Email = "test@smartlib.ba",
                Lozinka = "Lozinka1!"
            });

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<LoginResponse>(ok.Value);
            Assert.Equal("Bibliotekar", response.Uloga);
        }

        // US-04: JWT token sadržaj (claims)

        [Fact]
        public async Task Login_UspjesanLogin_TokenSadrziTacniEmail()
        {
            // JWT token mora sadržavati email korisnika kao claim
            _repoMock.Setup(r => r.GetByEmailAsync("test@smartlib.ba"))
                     .ReturnsAsync(PravianKorisnik());

            var result = await _controller.Login(new LoginRequest
            {
                Email = "test@smartlib.ba",
                Lozinka = "Lozinka1!"
            });

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<LoginResponse>(ok.Value);
            var token = ParseToken(response.Token);

            var emailClaim = token.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Email);

            Assert.NotNull(emailClaim);
            Assert.Equal("test@smartlib.ba", emailClaim!.Value);
        }

        [Fact]
        public async Task Login_UspjesanLogin_TokenSadrziTacnuUlogu()
        {
            // JWT token mora sadržavati ulogu korisnika — koristi se za autorizaciju
            _repoMock.Setup(r => r.GetByEmailAsync("test@smartlib.ba"))
                     .ReturnsAsync(PravianKorisnik("Administrator"));

            var result = await _controller.Login(new LoginRequest
            {
                Email = "test@smartlib.ba",
                Lozinka = "Lozinka1!"
            });

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<LoginResponse>(ok.Value);
            var token = ParseToken(response.Token);

            var roleClaim = token.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Role);

            Assert.NotNull(roleClaim);
            Assert.Equal("Administrator", roleClaim!.Value);
        }

        [Fact]
        public async Task Login_UspjesanLogin_TokenSadrziTacniKorisnikId()
        {
            // NameIdentifier claim mora biti ID korisnika iz baze
            _repoMock.Setup(r => r.GetByEmailAsync("test@smartlib.ba"))
                     .ReturnsAsync(PravianKorisnik());

            var result = await _controller.Login(new LoginRequest
            {
                Email = "test@smartlib.ba",
                Lozinka = "Lozinka1!"
            });

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<LoginResponse>(ok.Value);
            var token = ParseToken(response.Token);

            var idClaim = token.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            Assert.NotNull(idClaim);
            Assert.Equal("1", idClaim!.Value);  // Id=1 iz PravianKorisnik()
        }

        // US-07: JWT token expiry 

        [Fact]
        public async Task Login_UspjesanLogin_TokenIsteceZa60Minuta()
        {
            // US-07: Sesija mora isteći — token expiry mora biti postavljen
            _repoMock.Setup(r => r.GetByEmailAsync("test@smartlib.ba"))
                     .ReturnsAsync(PravianKorisnik());

            var vrijemePrije = DateTime.UtcNow.AddMinutes(JwtMinutes - 1);
            var vrijemeNakon = DateTime.UtcNow.AddMinutes(JwtMinutes + 1);

            var result = await _controller.Login(new LoginRequest
            {
                Email = "test@smartlib.ba",
                Lozinka = "Lozinka1!"
            });

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<LoginResponse>(ok.Value);
            var token = ParseToken(response.Token);

            // Expiry mora biti unutar očekivanog prozora od +/- 1 minute
            Assert.True(token.ValidTo > vrijemePrije,
                $"Token ističe prerano: {token.ValidTo}");
            Assert.True(token.ValidTo < vrijemeNakon,
                $"Token ističe prekasno: {token.ValidTo}");
        }

        [Fact]
        public async Task Login_UspjesanLogin_TokenJeTrenutnoValidan()
        {
            // US-07: Token mora biti validan odmah nakon izdavanja
            _repoMock.Setup(r => r.GetByEmailAsync("test@smartlib.ba"))
                     .ReturnsAsync(PravianKorisnik());

            var result = await _controller.Login(new LoginRequest
            {
                Email = "test@smartlib.ba",
                Lozinka = "Lozinka1!"
            });

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<LoginResponse>(ok.Value);

            // Validacija tokena kroz Microsoft biblioteku
            var validationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = JwtIssuer,
                ValidAudience = JwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(
                                               Encoding.UTF8.GetBytes(JwtKey))
            };

            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(
                                response.Token, validationParams, out _);

            Assert.NotNull(principal);
        }
        
        // US-05: NEUSPJEŠNA PRIJAVA — greške i poruke       

        [Fact]
        public async Task Login_NetacnaLozinka_VracaUnauthorized()
        {
            _repoMock.Setup(r => r.GetByEmailAsync("test@smartlib.ba"))
                     .ReturnsAsync(PravianKorisnik());

            var result = await _controller.Login(new LoginRequest
            {
                Email = "test@smartlib.ba",
                Lozinka = "PogrešnaLozinka!"
            });

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        [Fact]
        public async Task Login_NepostojeciEmail_VracaUnauthorized()
        {
            _repoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                     .ReturnsAsync((Korisnik?)null);

            var result = await _controller.Login(new LoginRequest
            {
                Email = "nepostoji@smartlib.ba",
                Lozinka = "BilaSta1!"
            });

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        [Fact]
        public async Task Login_Neuspjeh_PorukaJeGenerickaNeSadrziDetalje()
        {
            // US-05: Poruka ne smije otkrivati da li greška leži u emailu ili lozinci
            _repoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                     .ReturnsAsync((Korisnik?)null);

            var result = await _controller.Login(new LoginRequest
            {
                Email = "x@x.com",
                Lozinka = "x"
            });

            var unauth = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            var body = unauth.Value?.ToString() ?? string.Empty;

            Assert.DoesNotContain("email", body, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("lozinka", body, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Login_NetacnaLozinka_IDeaktiviran_PorukaJeIstaGenerička()
        {
            // US-05 + US-09: Sve greške moraju dati istu poruku — sistem ne smije
            // otkrivati razlog odbijanja (da li je korisnik deaktiviran ili lozinka pogrešna)
            var deaktiviran = PravianKorisnik();
            deaktiviran.Status = "deaktiviran";

            _repoMock.Setup(r => r.GetByEmailAsync("test@smartlib.ba"))
                     .ReturnsAsync(deaktiviran);

            var rezultatDeaktiviran = await _controller.Login(new LoginRequest
            {
                Email = "test@smartlib.ba",
                Lozinka = "Lozinka1!"
            });

            _repoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                     .ReturnsAsync((Korisnik?)null);

            var rezultatNepostoji = await _controller.Login(new LoginRequest
            {
                Email = "nepostoji@smartlib.ba",
                Lozinka = "Lozinka1!"
            });

            var unauthDeaktiviran = Assert.IsType<UnauthorizedObjectResult>(rezultatDeaktiviran.Result);
            var unauthNepostoji = Assert.IsType<UnauthorizedObjectResult>(rezultatNepostoji.Result);
           
            Assert.Equal(
                unauthDeaktiviran.Value?.ToString(),
                unauthNepostoji.Value?.ToString()
            );
        }
      
        // US-09: DEAKTIVIRAN KORISNIK

        [Fact]
        public async Task Login_DeaktiviranKorisnik_VracaUnauthorized()
        {
            var korisnik = PravianKorisnik();
            korisnik.Status = "deaktiviran";

            _repoMock.Setup(r => r.GetByEmailAsync("test@smartlib.ba"))
                     .ReturnsAsync(korisnik);

            var result = await _controller.Login(new LoginRequest
            {
                Email = "test@smartlib.ba",
                Lozinka = "Lozinka1!"
            });

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        [Fact]
        public async Task Login_DeaktiviranKorisnik_NeDobivaToken()
        {
            // US-09: Deaktiviran korisnik ne smije dobiti JWT token ni u kakvom obliku
            var korisnik = PravianKorisnik();
            korisnik.Status = "deaktiviran";

            _repoMock.Setup(r => r.GetByEmailAsync("test@smartlib.ba"))
                     .ReturnsAsync(korisnik);

            var result = await _controller.Login(new LoginRequest
            {
                Email = "test@smartlib.ba",
                Lozinka = "Lozinka1!"
            });

            // Rezultat mora biti Unauthorized — ne smije biti OkObjectResult sa tokenom
            Assert.IsNotType<OkObjectResult>(result.Result);
        }
       
        // US-06: ODJAVA KORISNIKA      

        [Fact]
        public void Logout_VracaOkSaPorukom()
        {
            // US-06: Endpoint za odjavu mora vratiti potvrdu
            var result = _controller.Logout();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Logout_ResponseSadrziPoruku()
        {
            // US-06: Poruka potvrde mora biti prisutna u odgovoru
            var result = _controller.Logout();
            var ok = Assert.IsType<OkObjectResult>(result);
            var body = ok.Value?.ToString() ?? string.Empty;

            Assert.NotEmpty(body);
        }
 
        // US-04: VALIDACIJA MODELA — prazna i neispravna polja       

        [Theory]
        [InlineData("", "Lozinka1!")]
        [InlineData("test@smartlib.ba", "")]
        [InlineData("nijeEmail", "Lozinka1!")]
        public async Task Login_NeispravanModel_VracaBadRequest(string email, string lozinka)
        {           
            _controller.ModelState.AddModelError("test", "Greška validacije");
            
            var result = await _controller.Login(new LoginRequest
            {
                Email = email,
                Lozinka = lozinka
            });

            // Provjeravamo da li je rezultat ObjectResult
            var objectResult = Assert.IsType<ObjectResult>(result.Result);

            Assert.IsAssignableFrom<Microsoft.AspNetCore.Mvc.ProblemDetails>(objectResult.Value);
        }

        [Fact]
        public async Task Login_PrazanEmail_NePozivaSeBaza()
        {
            // Ako model nije validan, baza se ne smije ni kontaktirati
            _controller.ModelState.AddModelError(nameof(LoginRequest.Email), "Email je obavezan.");

            await _controller.Login(new LoginRequest
            {
                Email = "",
                Lozinka = "Lozinka1!"
            });

            _repoMock.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Login_PraznaLozinka_NePozivaSeBaza()
        {
            // Isto kao gore — prazna lozinka ne smije dopustiti poziv bazi
            _controller.ModelState.AddModelError(nameof(LoginRequest.Lozinka), "Lozinka je obavezna.");

            await _controller.Login(new LoginRequest
            {
                Email = "test@smartlib.ba",
                Lozinka = ""
            });

            _repoMock.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), Times.Never);
        }
    }
}