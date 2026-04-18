using Microsoft.AspNetCore.Mvc;

namespace SmartLib.Web.Controllers
{
    /// <summary>
    /// Početna stranica i opće rute
    /// </summary>
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            // TODO: Error handling view
            return View();
        }
    }
}
