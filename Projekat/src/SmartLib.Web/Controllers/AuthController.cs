using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;

namespace SmartLib.Web.Controllers
{
    /// <summary>
    /// Auth modul — Prijava i odjava korisnika (MVC)
    /// </summary>
    public class AuthController : Controller
    {
        private readonly IKorisnikRepository _korisnikRepository;

        public AuthController(IKorisnikRepository korisnikRepository)
        {
            _korisnikRepository = korisnikRepository;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var korisnik = await _korisnikRepository.GetByEmailAsync(model.Email);
            if (korisnik is null || !string.Equals(korisnik.Status, "aktivan", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(string.Empty, "Prijava nije uspjela.");
                return View(model);
            }

            var isPasswordValid = PasswordHasher.VerifyPassword(model.Lozinka, korisnik.LozinkaHash);
            if (!isPasswordValid)
            {
                ModelState.AddModelError(string.Empty, "Prijava nije uspjela.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, korisnik.Id.ToString()),
                new(ClaimTypes.Name, $"{korisnik.Ime} {korisnik.Prezime}"),
                new(ClaimTypes.Email, korisnik.Email),
                new(ClaimTypes.Role, korisnik.Uloga?.Naziv ?? string.Empty)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                });

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            if (string.Equals(korisnik.Uloga?.Naziv, "Bibliotekar", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(korisnik.Uloga?.Naziv, "Administrator", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Korisnik");
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }
    }
}
