using Microsoft.AspNetCore.Mvc;

namespace SmartLib.API.Controllers
{
    /// <summary>
    /// Auth modul — Autentifikacija korisnika
    /// Prijava, odjava, izdavanje JWT tokena
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // TODO: Inject IAuthService

        /// <summary>
        /// Prijava korisnika (email + lozinka) → JWT token
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login(/* TODO: LoginRequest dto */)
        {
            // TODO: Implementirati prijavu
            // 1. Validirati email i lozinku
            // 2. Provjeriti hash lozinke
            // 3. Provjeriti da korisnik nije deaktiviran
            // 4. Izdati JWT token
            // 5. Vratiti generičku grešku ako login ne uspije
            return Ok(new { message = "TODO: Implementirati login" });
        }

        /// <summary>
        /// Odjava korisnika — invalidacija tokena
        /// </summary>
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // TODO: Implementirati odjavu
            return Ok(new { message = "TODO: Implementirati logout" });
        }
    }
}
