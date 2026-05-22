using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
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
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IKorisnikRepository korisnikRepository,
            IEmailService emailService,
            ILogger<AuthController> logger)
        {
            _korisnikRepository = korisnikRepository;
            _emailService = emailService;
            _logger = logger;
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

            // Signal the UI to show a one-time welcome animation after successful sign-in
            TempData["ShowWelcomeAnimation"] = "true";
            TempData["WelcomeName"] = $"{korisnik.Ime} {korisnik.Prezime}";

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

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var korisnik = await _korisnikRepository.GetByEmailAsync(model.Email);
            if (korisnik != null && string.Equals(korisnik.Status, "aktivan", StringComparison.OrdinalIgnoreCase))
            {
                // Generate a secure random token
                var rawToken = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));

                // Hash the token before storing it
                korisnik.ResetToken = PasswordHasher.HashPassword(rawToken);
                korisnik.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
                await _korisnikRepository.UpdateAsync(korisnik);

                // Url.Action handles URL-encoding automatically — do NOT pre-encode
                var resetLink = Url.Action("ResetPassword", "Auth", new { token = rawToken, email = model.Email }, Request.Scheme);
                
                if (resetLink != null)
                {
                    var emailBody = $@"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'></head>
<body style='margin:0;padding:0;background:#f0f4f8;font-family:Arial,Helvetica,sans-serif;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='background:#f0f4f8;padding:40px 0;'>
    <tr><td align='center'>
      <table width='520' cellpadding='0' cellspacing='0' style='background:#ffffff;border-radius:16px;overflow:hidden;box-shadow:0 8px 30px rgba(0,0,0,0.08);'>
        <!-- Header -->
        <tr>
          <td style='background:linear-gradient(135deg,#07111f,#173b63);padding:32px 40px;text-align:center;'>
            <h1 style='margin:0;color:#ffffff;font-size:26px;font-weight:800;letter-spacing:-0.5px;'>📚 SmartLib</h1>
            <p style='margin:6px 0 0;color:rgba(255,255,255,0.7);font-size:13px;'>Informacioni sistem za biblioteku</p>
          </td>
        </tr>
        <!-- Body -->
        <tr>
          <td style='padding:36px 40px 20px;'>
            <h2 style='margin:0 0 12px;color:#1f2937;font-size:20px;font-weight:700;'>Resetovanje lozinke</h2>
            <p style='margin:0 0 20px;color:#64748b;font-size:15px;line-height:1.6;'>
              Primili smo zahtjev za resetovanje lozinke vašeg SmartLib naloga. Kliknite na dugme ispod da postavite novu lozinku.
            </p>
            <table width='100%' cellpadding='0' cellspacing='0'>
              <tr><td align='center' style='padding:8px 0 24px;'>
                <a href='{resetLink}' style='display:inline-block;background:linear-gradient(135deg,#1e3a5f,#2f5f8f);color:#ffffff;text-decoration:none;padding:14px 36px;border-radius:999px;font-size:15px;font-weight:700;box-shadow:0 8px 20px rgba(30,58,95,0.25);'>
                  Resetuj lozinku
                </a>
              </td></tr>
            </table>
            <p style='margin:0 0 8px;color:#94a3b8;font-size:13px;line-height:1.5;'>
              Ovaj link je validan <strong>1 sat</strong>. Ako niste vi zatražili promjenu lozinke, slobodno ignorišite ovaj email.
            </p>
          </td>
        </tr>
        <!-- Footer -->
        <tr>
          <td style='padding:20px 40px 28px;border-top:1px solid #e2e8f0;'>
            <p style='margin:0;color:#94a3b8;font-size:12px;text-align:center;'>
              © {DateTime.UtcNow.Year} SmartLib. Sva prava zadržana.
            </p>
          </td>
        </tr>
      </table>
    </td></tr>
  </table>
</body>
</html>";

                    await _emailService.SendEmailAsync(
                        model.Email, 
                        "Resetovanje lozinke - SmartLib", 
                        emailBody);
                    _logger.LogInformation("Password reset link sent to {Email}", model.Email);
                }
            }
            else
            {
                // To prevent email enumeration, we log but don't show an error to the user
                // Consider adding a random delay here to mitigate timing attacks if needed
                _logger.LogWarning("Password reset requested for non-existent or inactive email: {Email}", model.Email);
            }

            // Always return the confirmation view, even if the user doesn't exist
            return View("ForgotPasswordConfirmation");
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return BadRequest("Neispravan zahtjev za resetovanje lozinke.");
            }

            var model = new ResetPasswordRequest { Token = token };
            ViewData["Email"] = email;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest model, string email)
        {
            ViewData["Email"] = email;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var korisnik = await _korisnikRepository.GetByEmailAsync(email);
            
            if (korisnik == null || 
                korisnik.ResetTokenExpiry < DateTime.UtcNow ||
                !string.Equals(korisnik.Status, "aktivan", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Invalid or expired password reset attempt for email: {Email}", email);
                ModelState.AddModelError(string.Empty, "Link za resetovanje je nevažeći ili je istekao.");
                return View(model);
            }

            // Verify the token hash
            var isTokenValid = PasswordHasher.VerifyPassword(model.Token, korisnik.ResetToken ?? string.Empty);
            if (!isTokenValid)
            {
                _logger.LogWarning("Invalid password reset token for email: {Email}", email);
                ModelState.AddModelError(string.Empty, "Link za resetovanje je nevažeći ili je istekao.");
                return View(model);
            }

            // Hash new password and clear token
            korisnik.LozinkaHash = PasswordHasher.HashPassword(model.NovaLozinka);
            korisnik.ResetToken = null;
            korisnik.ResetTokenExpiry = null;
            await _korisnikRepository.UpdateAsync(korisnik);

            _logger.LogInformation("Password successfully reset for {Email}", email);

            return View("ResetPasswordConfirmation");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeaktivirajMojNalog()
        {
          var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
          if (idClaim == null || !int.TryParse(idClaim, out int id))
            return RedirectToAction("Login", "Auth");

          var korisnik = await _korisnikRepository.GetByIdAsync(id);
          if (korisnik == null)
            return NotFound();

          if (string.Equals(korisnik.Uloga?.Naziv, RoleNames.Administrator, StringComparison.OrdinalIgnoreCase))
          {
            TempData["ErrorMessage"] = "Administrator ne može deaktivirati svoj nalog.";
            return RedirectToAction("Profil", "Korisnik");
          }

          if (!string.Equals(korisnik.Status, "deaktiviran", StringComparison.OrdinalIgnoreCase))
          {
            korisnik.Status = "deaktiviran";
            korisnik.DatumDeaktivacije = DateTime.UtcNow;
            await _korisnikRepository.UpdateAsync(korisnik);
          }

          await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
          TempData["SuccessMessage"] = "Nalog je deaktiviran. Kontaktirajte biblioteku ako zelite reaktivaciju.";
          return RedirectToAction("Login", "Auth");
        }

          [Authorize]
          [HttpGet]
          public IActionResult ChangePassword()
          {
            return View(new ChangePasswordRequest());
          }

          [Authorize]
          [HttpPost]
          [ValidateAntiForgeryToken]
          public async Task<IActionResult> ChangePassword(ChangePasswordRequest model)
          {
            if (!ModelState.IsValid)
            {
              return View(model);
            }

            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idClaim == null || !int.TryParse(idClaim, out int id))
            {
              return RedirectToAction("Login", "Auth");
            }

            var korisnik = await _korisnikRepository.GetByIdAsync(id);
            if (korisnik == null)
            {
              return NotFound();
            }

            var validCurrent = PasswordHasher.VerifyPassword(model.TrenutnaLozinka, korisnik.LozinkaHash);
            if (!validCurrent)
            {
              ModelState.AddModelError(nameof(model.TrenutnaLozinka), "Trenutna lozinka nije ispravna.");
              return View(model);
            }

            korisnik.LozinkaHash = PasswordHasher.HashPassword(model.NovaLozinka);
            await _korisnikRepository.UpdateAsync(korisnik);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["SuccessMessage"] = "Lozinka je promijenjena. Prijavite se ponovo.";
            return RedirectToAction("Login", "Auth");
          }
    }
}
