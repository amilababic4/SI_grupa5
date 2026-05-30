using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using System.Security.Claims;
using System.Text;

namespace SmartLib.Web.Controllers;

[Authorize(Roles = $"{RoleNames.Administrator},{RoleNames.Bibliotekar}")]
public class NabavkaController : Controller
{
    private readonly INabavkaRepository _repo;
    private readonly IEmailService _email;

    public NabavkaController(INabavkaRepository repo, IEmailService email)
    {
        _repo = repo;
        _email = email;
    }

    // GET: /Nabavka/Zahtjev
    public async Task<IActionResult> Zahtjev()
    {
        var vm = new NabavkaPageViewModel
        {
            ZadnjeNabavke = await _repo.GetZadnjeAsync(3),
            DistributerEmail = await _repo.GetDistributerEmailAsync() ?? string.Empty
        };
        return View(vm);
    }

    // POST: /Nabavka/Zahtjev
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Zahtjev(NabavkaZahtjevDto dto)
    {
        if (!ModelState.IsValid)
        {
            var vm = new NabavkaPageViewModel
            {
                Forma = dto,
                ZadnjeNabavke = await _repo.GetZadnjeAsync(3),
                DistributerEmail = await _repo.GetDistributerEmailAsync() ?? string.Empty
            };
            return View(vm);
        }

        var distributerEmail = await _repo.GetDistributerEmailAsync();
        var ime = User.FindFirstValue(ClaimTypes.Name) ?? "Bibliotekar";
        var korisnikId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

        var subject = $"Zahtjev za nabavku: {dto.NazivKnjige}";
        var body = new StringBuilder();
        body.AppendLine("<h2 style='color:#173b63;font-family:sans-serif;'>Zahtjev za nabavku knjige</h2>");
        body.AppendLine("<table style='border-collapse:collapse;font-family:sans-serif;font-size:14px;'>");
        body.AppendLine($"<tr><td style='padding:6px 16px 6px 0;color:#64748b;font-weight:700;'>Naziv knjige:</td><td>{dto.NazivKnjige}</td></tr>");
        body.AppendLine($"<tr><td style='padding:6px 16px 6px 0;color:#64748b;font-weight:700;'>Autor:</td><td>{dto.Autor}</td></tr>");
        body.AppendLine($"<tr><td style='padding:6px 16px 6px 0;color:#64748b;font-weight:700;'>Izdavač:</td><td>{dto.Izdavac}</td></tr>");
        body.AppendLine($"<tr><td style='padding:6px 16px 6px 0;color:#64748b;font-weight:700;'>Broj primjeraka:</td><td>{dto.BrojPrimjeraka}</td></tr>");
        body.AppendLine($"<tr><td style='padding:6px 16px 6px 0;color:#64748b;font-weight:700;'>Napomena:</td><td style='font-style:italic;'>{dto.Napomena ?? "—"}</td></tr>");
        body.AppendLine("</table>");
        body.AppendLine($"<p style='margin-top:16px;font-family:sans-serif;font-size:13px;color:#64748b;'>Podnio: <strong>{ime}</strong> — {DateTime.Now:dd.MM.yyyy HH:mm}</p>");

        bool poslan = false;
        try
        {
            await _email.SendEmailAsync(distributerEmail!, subject, body.ToString());
            poslan = true;
        }
        catch { poslan = false; }

        await _repo.CreateAsync(new NabavkaZahtjev
        {
            NazivKnjige = dto.NazivKnjige,
            Autor = dto.Autor,
            Izdavac = dto.Izdavac,
            BrojPrimjeraka = dto.BrojPrimjeraka,
            Napomena = dto.Napomena?.Trim(),
            EmailPoslan = poslan,
            PodnosilacId = korisnikId
        });

        TempData[poslan ? "Uspjeh" : "Greska"] = poslan
            ? "Zahtjev je uspješno poslan distributeru."
            : "Zahtjev je evidentiran, ali slanje emaila nije uspjelo.";

        return RedirectToAction(nameof(Zahtjev));
    }

    // POST: /Nabavka/PromijeniEmail
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PromijeniEmail(PromijeniDistributerDto dto)
    {
        if (!ModelState.IsValid)
        {
            TempData["Greska"] = "Unesite ispravnu email adresu.";
            return RedirectToAction(nameof(Zahtjev));
        }
        await _repo.SetDistributerEmailAsync(dto.Email.Trim());
        TempData["Uspjeh"] = $"Email distributera je promijenjen na: {dto.Email}";
        return RedirectToAction(nameof(Zahtjev));
    }
}