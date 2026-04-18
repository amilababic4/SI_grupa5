using Microsoft.AspNetCore.Mvc;

namespace SmartLib.Web.Controllers
{
    /// <summary>
    /// Auth modul — Prijava i odjava korisnika (MVC)
    /// </summary>
    public class AuthController : Controller
    {
        // TODO: Inject IKorisnikRepository i auth servis

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(/* TODO: LoginViewModel model */)
        {
            // TODO: Implementirati prijavu
            // 1. Validirati model
            // 2. Provjeriti email i lozinku
            // 3. Kreirati authentication cookie
            // 4. Redirect na Dashboard
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // TODO: Implementirati odjavu
            // await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
