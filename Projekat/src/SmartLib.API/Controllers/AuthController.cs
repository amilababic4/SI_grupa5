using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Infrastructure.Security;

namespace SmartLib.API.Controllers
{
    ///US 04 
    /// US 05
    /// US 09
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IKorisnikRepository _korisnikRepository;
        private readonly IConfiguration _configuration;

        public AuthController(IKorisnikRepository korisnikRepository, IConfiguration configuration)
        {
            _korisnikRepository = korisnikRepository;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest model)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var korisnik = await _korisnikRepository.GetByEmailAsync(model.Email);

            if (korisnik is null ||
                !string.Equals(korisnik.Status, "aktivan", StringComparison.OrdinalIgnoreCase) ||
                !PasswordHasher.VerifyPassword(model.Lozinka, korisnik.LozinkaHash))
            {
                return Unauthorized(new { message = "Prijava nije uspjela." });
            }

            var token = GenerateJwtToken(korisnik);

            return Ok(new LoginResponse
            {
                Token = token,
                Ime = korisnik.Ime,
                Prezime = korisnik.Prezime,
                Uloga = korisnik.Uloga?.Naziv ?? string.Empty
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { message = "Korisnik je odjavljen." });
        }

        private string GenerateJwtToken(SmartLib.Core.Models.Korisnik korisnik)
        {
            var jwt = _configuration.GetSection("Jwt");

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, korisnik.Id.ToString()),
                new(ClaimTypes.Name, $"{korisnik.Ime} {korisnik.Prezime}"),
                new(ClaimTypes.Email, korisnik.Email),
                new(ClaimTypes.Role, korisnik.Uloga?.Naziv ?? string.Empty)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt["Key"]!)
            );

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpirationMinutes"]!)),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}