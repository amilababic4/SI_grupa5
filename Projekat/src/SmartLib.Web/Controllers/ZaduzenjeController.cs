using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Services;

namespace SmartLib.Web.Controllers
{
    [Authorize]
    public class ZaduzenjeController : Controller
    {
        private readonly IZaduzenjeRepository _zaduzenjeRepo;
        private readonly IKorisnikRepository _korisnikRepo;
        private readonly IKnjigaRepository _knjigaRepo;
        private readonly IPrimjerakRepository _primjerakRepo;
        private readonly IRezervacijaRepository _rezervacijaRepo;
        private readonly IRecenzijaRepository _recenzijaRepo;
        private readonly INotifikacijaRepository _notifikacijaRepo;
        private readonly CacheVersionStore _cacheVersions;
        private readonly BibliotekariNotifikacijaService _bibliotekariNotifikacija;
        private readonly ILogger<ZaduzenjeController> _logger;

        public ZaduzenjeController(
            IZaduzenjeRepository zaduzenjeRepo,
            IKorisnikRepository korisnikRepo,
            IKnjigaRepository knjigaRepo,
            IPrimjerakRepository primjerakRepo,
            IRezervacijaRepository rezervacijaRepo,
            IRecenzijaRepository recenzijaRepo,
            INotifikacijaRepository notifikacijaRepo,
            CacheVersionStore cacheVersions,
            BibliotekariNotifikacijaService bibliotekariNotifikacija,
            ILogger<ZaduzenjeController> logger)
        {
            _zaduzenjeRepo = zaduzenjeRepo;
            _korisnikRepo = korisnikRepo;
            _knjigaRepo = knjigaRepo;
            _primjerakRepo = primjerakRepo;
            _rezervacijaRepo = rezervacijaRepo;
            _recenzijaRepo = recenzijaRepo;
            _notifikacijaRepo = notifikacijaRepo;
            _cacheVersions = cacheVersions;
            _bibliotekariNotifikacija = bibliotekariNotifikacija;
            _logger = logger;
        }

        // US-65, US-66, US-68: Aktivna zaduženja (bibliotekar)
        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        public async Task<IActionResult> Index(string? clan)
        {
            var sva = await _zaduzenjeRepo.GetActiveAsync();

            if (!string.IsNullOrWhiteSpace(clan))
            {
                var filter = clan.Trim().ToLowerInvariant();
                sva = sva.Where(z =>
                    ($"{z.Korisnik?.Ime} {z.Korisnik?.Prezime}").ToLowerInvariant().Contains(filter) ||
                    (z.Korisnik?.Email ?? string.Empty).ToLowerInvariant().Contains(filter));
            }

            var model = new AktivnaZaduzenjaViewModel
            {
                Clan = clan,
                Zaduzenja = sva.Select(MapToViewModel).ToList()
            };

            return View(model);
        }

        // US-62, US-63, US-64: Vlastita zaduženja (prijavljeni korisnik)
        public async Task<IActionResult> Moja()
        {
            var korisnikIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(korisnikIdStr, out var korisnikId))
                return RedirectToAction("Login", "Auth");

            var zaduzenja = await _zaduzenjeRepo.GetByKorisnikAsync(korisnikId);
            var model = zaduzenja.Select(MapToViewModel).ToList();
            return View(model);
        }

        // US-67: Detalji zaduženja (bibliotekar)
        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        public async Task<IActionResult> Details(int id, string? returnUrl = null,
            int? korisnikId = null, int? primjerakId = null)
        {
            var z = await _zaduzenjeRepo.GetByIdAsync(id);
            if (z == null) return NotFound();

            var poznatUrl = new[] { "Historija", "ZaduzenjaClana", "HistorijaClana", "ZaduzenjaPrimjerka" };
            ViewBag.ReturnUrl = poznatUrl.Contains(returnUrl) ? returnUrl : "Index";
            ViewBag.KorisnikId = korisnikId;
            ViewBag.PrimjerakId = primjerakId;

            return View(MapToViewModel(z));
        }

        // US-44: Forma za novo zaduživanje (bibliotekar)
        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateCreateDropdowns();
            return View(new ZaduzenjeCreateDto());
        }

        // US-44, US-46, US-47: Kreiranje zaduženja (bibliotekar)
        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Zaduzi(ZaduzenjeCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCreateDropdowns(model);
                return View("Create", model);
            }

            // US-47: Provjera dostupnosti primjerka
            var primjerak = await _primjerakRepo.GetByIdAsync(model.PrimjerakId);
            if (primjerak == null || primjerak.Status != "dostupan")
            {
                ModelState.AddModelError(string.Empty, "Odabrani primjerak nije dostupan za zaduživanje.");
                await PopulateCreateDropdowns(model);
                return View("Create", model);
            }

            // US-47: Provjera duplikata aktivnog zaduženja za isti primjerak
            var imaDuplikat = await _primjerakRepo.HasActiveZaduzenjeAsync(model.PrimjerakId);
            if (imaDuplikat)
            {
                ModelState.AddModelError(string.Empty, "Odabrani primjerak već ima aktivno zaduženje.");
                await PopulateCreateDropdowns(model);
                return View("Create", model);
            }

            var imaKasnela = await _zaduzenjeRepo.ImaKasnelaZaduzenjaAsync(model.KorisnikId);
            if (imaKasnela)
            {
                ModelState.AddModelError(string.Empty,
                    "Nije moguće kreirati zaduženje — odabrani član ima jedno ili više zakasnjelih zaduženja koja nisu vraćena.");
                await PopulateCreateDropdowns(model);
                return View("Create", model);
            }

            // Provjera da rok vraćanja nije u prošlosti
            if (model.DatumPovratka.HasValue && model.DatumPovratka.Value.Date < DateTime.Today)
            {
                ModelState.AddModelError(nameof(model.DatumPovratka), "Datum povratka ne može biti u prošlosti.");
                await PopulateCreateDropdowns(model);
                return View("Create", model);
            }

            // Rok vraćanja: uneseni datum ili automatski 2 mjeseca
            var datumZaduzivanja = DateTime.UtcNow;
            var datumPlaniranogVracanja = model.DatumPovratka.HasValue
                ? DateTime.SpecifyKind(model.DatumPovratka.Value.Date, DateTimeKind.Utc)
                : datumZaduzivanja.AddMonths(2);

            var zaduzenje = new Zaduzenje
            {
                KorisnikId = model.KorisnikId,
                PrimjerakId = model.PrimjerakId,
                DatumZaduzivanja = datumZaduzivanja,
                DatumPlaniranogVracanja = datumPlaniranogVracanja,
                Status = "aktivno"
            };

            await _zaduzenjeRepo.CreateAsync(zaduzenje);
            await _primjerakRepo.UpdateStatusAsync(model.PrimjerakId, "zadužen");
            await _rezervacijaRepo.FulfillAsync(model.KorisnikId, primjerak.KnjigaId);

            // ── Email notifikacija bibliotekarima ────────────────────────────
            _logger.LogWarning(">>> DEBUG Zaduzi: Zaduzenje kreirano ID={Id}, dohvatam sa navigacijom...", zaduzenje.Id);
            try
            {
                var zaduzenjeZaEmail = await _zaduzenjeRepo.GetByIdAsync(zaduzenje.Id);
                _logger.LogWarning(">>> DEBUG Zaduzi: GetByIdAsync={Null}, Korisnik={Korisnik}, Knjiga={Knjiga}",
                    zaduzenjeZaEmail == null ? "NULL" : "OK",
                    zaduzenjeZaEmail?.Korisnik?.Email ?? "NULL",
                    zaduzenjeZaEmail?.Primjerak?.Knjiga?.Naslov ?? "NULL");

                if (zaduzenjeZaEmail != null)
                {
                    await _bibliotekariNotifikacija.ObavijestiBibliotekareNovoZaduzenjeAsync(zaduzenjeZaEmail);
                    _logger.LogWarning(">>> DEBUG Zaduzi: ObavijestiBibliotekare dovršeno.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ">>> DEBUG Zaduzi: GREŠKA pri slanju email notifikacije za zaduženje #{Id}", zaduzenje.Id);
            }
            // ─────────────────────────────────────────────────────────────────

            _cacheVersions.BumpBooksVersion();
            TempData["SuccessMessage"] = "Zaduživanje je uspješno evidentirano.";
            return RedirectToAction(nameof(Index));
        }

        // US-45: Stranica za potvrdu vraćanja (bibliotekar)
        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        [HttpGet]
        public async Task<IActionResult> VratiPotvrda(int id)
        {
            var z = await _zaduzenjeRepo.GetByIdAsync(id);
            if (z == null) return NotFound();
            if (z.Status != "aktivno") return RedirectToAction(nameof(Index));
            return View(MapToViewModel(z));
        }

        // US-45: Evidencija vraćanja knjige (bibliotekar)
        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Vrati(int id)
        {
            var z = await _zaduzenjeRepo.GetByIdAsync(id);
            if (z == null) return NotFound();

            if (z.Status == "aktivno")
            {
                z.Status = "zatvoreno";
                z.DatumStvarnogVracanja = DateTime.UtcNow;
                await _zaduzenjeRepo.UpdateAsync(z);
                await _primjerakRepo.UpdateStatusAsync(z.PrimjerakId, "dostupan");

                var rezervacija = await _rezervacijaRepo.GetNextActiveForBookAsync(z.Primjerak!.KnjigaId);
                if (rezervacija != null)
                {
                    var linkUrl = Url.Action("Moje", "Rezervacija") ?? "/Rezervacija/Moje";
                    var hasRecent = await _notifikacijaRepo.HasRecentAsync(
                        rezervacija.KorisnikId,
                        "Rezervacija",
                        linkUrl,
                        TimeSpan.FromHours(12));

                    if (!hasRecent)
                    {
                        var naslov = z.Primjerak?.Knjiga?.Naslov ?? "Knjiga";
                        await _notifikacijaRepo.AddAsync(new Notifikacija
                        {
                            KorisnikId = rezervacija.KorisnikId,
                            Naslov = "Knjiga je dostupna",
                            Poruka = $"Rezervisana knjiga \"{naslov}\" je sada dostupna za preuzimanje.",
                            Tip = "Rezervacija",
                            LinkUrl = linkUrl,
                            DatumKreiranja = DateTime.UtcNow
                        });
                    }
                }
            }

            _cacheVersions.BumpBooksVersion();
            TempData["SuccessMessage"] = "Vraćanje knjige je uspješno evidentirano.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // US-54: Historija zaduženja (bibliotekar)
        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        public async Task<IActionResult> Historija(string? clan)
        {
            var granica = DateTime.UtcNow.AddYears(-3);
            var sva = await _zaduzenjeRepo.GetClosedSinceAsync(granica);

            if (!string.IsNullOrWhiteSpace(clan))
            {
                var filter = clan.Trim().ToLowerInvariant();
                sva = sva.Where(z =>
                    ($"{z.Korisnik?.Ime} {z.Korisnik?.Prezime}").ToLowerInvariant().Contains(filter) ||
                    (z.Korisnik?.Email ?? string.Empty).ToLowerInvariant().Contains(filter));
            }

            var model = new HistorijaZaduzenjaViewModel
            {
                Clan = clan,
                Zaduzenja = sva.Select(MapToViewModel).ToList()
            };

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> MojaHistorija()
        {
            var korisnikIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(korisnikIdStr, out var korisnikId))
                return RedirectToAction("Login", "Auth");

            var granica = DateTime.UtcNow.AddYears(-3);
            var mojaZaduzenja = await _zaduzenjeRepo.GetClosedHistoryForKorisnikAsync(korisnikId, granica);
            var model = mojaZaduzenja.Select(MapToViewModel).ToList();

            foreach (var item in model)
            {
                item.ProcitanaKnjiga = item.DatumStvarnogVracanja.HasValue;
                if (item.KnjigaId > 0)
                    item.ImaRecenziju = await _recenzijaRepo.HasUserReviewedAsync(item.KnjigaId, korisnikId);
            }

            return View("MojaHistorija", model);
        }

        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        public async Task<IActionResult> HistorijaClana(int korisnikId)
        {
            var korisnik = await _korisnikRepo.GetByIdAsync(korisnikId);
            if (korisnik == null) return NotFound();

            var granica = DateTime.UtcNow.AddYears(-3);
            var zatvorenaZaduzenja = await _zaduzenjeRepo.GetClosedHistoryForKorisnikAsync(korisnikId, granica);
            var historija = zatvorenaZaduzenja.Select(MapToViewModel).ToList();

            ViewBag.KorisnikId = korisnikId;
            ViewBag.KorisnikIme = $"{korisnik.Ime} {korisnik.Prezime}";

            return View(historija);
        }

        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        public async Task<IActionResult> ZaduzenjaClana(int korisnikId)
        {
            var korisnik = await _korisnikRepo.GetByIdAsync(korisnikId);
            if (korisnik == null) return NotFound();

            var sva = await _zaduzenjeRepo.GetByKorisnikAsync(korisnikId);
            var aktivna = sva
                .Where(z => z.Status != "zatvoreno")
                .Select(MapToViewModel)
                .ToList();

            ViewBag.KorisnikId = korisnikId;
            ViewBag.KorisnikIme = $"{korisnik.Ime} {korisnik.Prezime}";

            return View(aktivna);
        }

        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        public async Task<IActionResult> ZaduzenjaPrimjerka(int primjerakId)
        {
            var primjerak = await _primjerakRepo.GetByIdAsync(primjerakId);
            if (primjerak == null) return NotFound();

            var sva = await _zaduzenjeRepo.GetByPrimjerakAsync(primjerakId);
            var model = sva.Select(MapToViewModel).ToList();

            ViewBag.PrimjerakId = primjerakId;
            ViewBag.InventarniBroj = primjerak.InventarniBroj;
            ViewBag.KnjigaNaslov = primjerak.Knjiga?.Naslov ?? string.Empty;
            ViewBag.KnjigaId = primjerak.KnjigaId;

            return View(model);
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static ZaduzenjeViewModel MapToViewModel(Zaduzenje z)
        {
            var now = DateTime.UtcNow;
            var zakasnilo = z.DatumPlaniranogVracanja < now;
            return new ZaduzenjeViewModel
            {
                Id = z.Id,
                KnjigaId = z.Primjerak?.KnjigaId ?? 0,
                KorisnikIme = z.Korisnik != null ? $"{z.Korisnik.Ime} {z.Korisnik.Prezime}" : "-",
                KorisnikEmail = z.Korisnik?.Email ?? "-",
                KnjigaNaslov = z.Primjerak?.Knjiga?.Naslov ?? "-",
                InventarniBroj = z.Primjerak?.InventarniBroj ?? "-",
                DatumZaduzivanja = z.DatumZaduzivanja,
                DatumPlaniranogVracanja = z.DatumPlaniranogVracanja,
                DatumStvarnogVracanja = z.DatumStvarnogVracanja,
                Status = z.Status,
                JeZakasnilo = zakasnilo,
                RokSeBliži = !zakasnilo && z.DatumPlaniranogVracanja <= now.AddDays(3)
            };
        }

        private async Task PopulateCreateDropdowns(ZaduzenjeCreateDto? selected = null)
        {
            var sviKorisnici = await _korisnikRepo.GetAllAsync();
            var clanovi = sviKorisnici
                .Where(k => k.Uloga?.Naziv == RoleNames.Clan)
                .ToList();

            ViewBag.ClanDataJson = JsonSerializer.Serialize(
                clanovi.Select(k => new { id = k.Id, text = $"{k.Ime} {k.Prezime} ({k.Email})" }));

            var sveknjige = await _knjigaRepo.SearchAsync(null, null);
            var knjigeSaDostupnim = sveknjige
                .Where(k => k.Primjerci.Any(p => p.Status == "dostupan"))
                .ToList();

            ViewBag.KnjigaDataJson = JsonSerializer.Serialize(
                knjigeSaDostupnim.Select(k => new { id = k.Id, text = $"{k.Naslov} - {k.Autor}" }));

            ViewBag.PrimjerakDataJson = JsonSerializer.Serialize(
                knjigeSaDostupnim.SelectMany(k => k.Primjerci
                    .Where(p => p.Status == "dostupan")
                    .Select(p => new { id = p.Id, knjigaId = p.KnjigaId, text = p.InventarniBroj })));

            ViewBag.SelectedKorisnikId = selected?.KorisnikId ?? 0;
            ViewBag.SelectedKnjigaId = selected?.KnjigaId ?? 0;
            ViewBag.SelectedPrimjerakId = selected?.PrimjerakId ?? 0;
        }
    }
}