using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Services;

namespace SmartLib.Web.Controllers
{
    [Authorize(Roles = $"{RoleNames.Administrator},{RoleNames.Bibliotekar}")]
    public class IzvjestajController : Controller
    {
        private readonly IIzvjestajService _izvjestajService;

        public IzvjestajController(IIzvjestajService izvjestajService)
        {
            _izvjestajService = izvjestajService;
        }

        // GET: /Izvjestaj
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.TrenutnaGodina = DateTime.Now.Year;
            ViewBag.TrenutniMjesec = DateTime.Now.Month;
            return View();
        }

        // POST: /Izvjestaj/Pregled
        [HttpPost]
        public async Task<IActionResult> Pregled(int mjesec, int godina, string tipIzvjestaja)
        {
            if (mjesec < 1 || mjesec > 12 || godina < 2000 || godina > DateTime.Now.Year + 1)
            {
                TempData["Greska"] = "Nevažeći period.";
                return RedirectToAction("Index");
            }

            return tipIzvjestaja switch
            {
                "rezervacije" => await PregledRezervacija(mjesec, godina),
                "clanovi" => await PregledClanova(mjesec, godina),
                _ => View("Pregled",
                                     await _izvjestajService.GenerirajMjesecnaZaduzenja(mjesec, godina))
            };
        }

        // GET: /Izvjestaj/PdfPreuzimanje?mjesec=5&godina=2025
        [HttpGet]
        public async Task<IActionResult> PdfPreuzimanje(int mjesec, int godina)
        {
            if (mjesec < 1 || mjesec > 12 || godina < 2000 || godina > DateTime.Now.Year + 1)
                return BadRequest("Nevažeći period.");

            var data = await _izvjestajService.GenerirajMjesecnaZaduzenja(mjesec, godina);
            var pdfBytes = IzvjestajPdfService.GenerirajMjesecnaZaduzenjaPdf(data);

            var fileName = $"SmartLib_Zaduzenja_{data.NazivMjeseca}_{data.Godina}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }

        // POST: /Izvjestaj/PregledRezervacija
        [HttpPost]
        public async Task<IActionResult> PregledRezervacija(int mjesec, int godina)
        {
            if (mjesec < 1 || mjesec > 12 || godina < 2000 || godina > DateTime.Now.Year + 1)
            {
                TempData["Greska"] = "Nevažeći period.";
                return RedirectToAction("Index");
            }
            var data = await _izvjestajService.GenerirajMjesecneRezervacije(mjesec, godina);
            return View("PregledRezervacija", data);
        }

        // GET: /Izvjestaj/PdfRezervacije?mjesec=5&godina=2025
        [HttpGet]
        public async Task<IActionResult> PdfRezervacije(int mjesec, int godina)
        {
            if (mjesec < 1 || mjesec > 12 || godina < 2000 || godina > DateTime.Now.Year + 1)
                return BadRequest("Nevažeći period.");

            var data = await _izvjestajService.GenerirajMjesecneRezervacije(mjesec, godina);
            var pdfBytes = IzvjestajPdfService.GenerirajMjesecneRezervacijePdf(data);
            return File(pdfBytes, "application/pdf", $"SmartLib_Rezervacije_{data.NazivMjeseca}_{data.Godina}.pdf");
        }

        // POST: /Izvjestaj/PregledClanova
        [HttpPost]
        public async Task<IActionResult> PregledClanova(int mjesec, int godina)
        {
            if (mjesec < 1 || mjesec > 12 || godina < 2000 || godina > DateTime.Now.Year + 1)
            {
                TempData["Greska"] = "Nevažeći period.";
                return RedirectToAction("Index");
            }
            var data = await _izvjestajService.GenerirajMjesecneKlanove(mjesec, godina);
            return View("PregledClanova", data);
        }

        // GET: /Izvjestaj/PdfClanovi?mjesec=5&godina=2025
        [HttpGet]
        public async Task<IActionResult> PdfClanovi(int mjesec, int godina)
        {
            if (mjesec < 1 || mjesec > 12 || godina < 2000 || godina > DateTime.Now.Year + 1)
                return BadRequest("Nevažeći period.");

            var data = await _izvjestajService.GenerirajMjesecneKlanove(mjesec, godina);
            var pdfBytes = IzvjestajPdfService.GenerirajMjesecneClanovePdf(data);
            return File(pdfBytes, "application/pdf", $"SmartLib_Clanovi_{data.NazivMjeseca}_{data.Godina}.pdf");
        }
    }
}