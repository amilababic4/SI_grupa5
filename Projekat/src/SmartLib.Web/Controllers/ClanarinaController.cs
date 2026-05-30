// Lokacija: SmartLib.Web/Controllers/ClanarinaController.cs
// ZAMIJENI cijeli postojeći fajl ovim sadržajem.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;

namespace SmartLib.Web.Controllers
{
    public class ClanarinaController : Controller
    {
        private readonly IClanarinaRepository _clanarinaRepo;
        private readonly IKorisnikRepository _korisnikRepository;
        private readonly IZahtjevProduzenjaRepository _zahtjevRepo;

        public ClanarinaController(
            IClanarinaRepository clanarinaRepo,
            IKorisnikRepository korisnikRepository,
            IZahtjevProduzenjaRepository zahtjevRepo)
        {
            _clanarinaRepo = clanarinaRepo;
            _korisnikRepository = korisnikRepository;
            _zahtjevRepo = zahtjevRepo;
        }

        // ═════════════════════════════════════════════════════════════════
        // BIBLIOTEKAR / ADMINISTRATOR — postojeće akcije (PB-33)
        // ═════════════════════════════════════════════════════════════════

        // GET: /Clanarina/Upsert?korisnikId=5
        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        [HttpGet]
        public async Task<IActionResult> Upsert(int korisnikId)
        {
            var postojeca = await _clanarinaRepo.GetByKorisnikAsync(korisnikId);

            var dto = postojeca is not null
                ? new ClanarinaUpsertDto
                {
                    KorisnikId = korisnikId,
                    DatumPocetka = postojeca.DatumPocetka,
                    DatumIsteka = postojeca.DatumIsteka
                }
                : new ClanarinaUpsertDto
                {
                    KorisnikId = korisnikId,
                    DatumPocetka = DateTime.Today,
                    DatumIsteka = DateTime.Today.AddYears(1)
                };

            ViewBag.Postoji = postojeca is not null;
            return View(dto);
        }

        // POST: /Clanarina/Upsert
        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ClanarinaUpsertDto dto)
        {
            if (dto.DatumIsteka <= dto.DatumPocetka)
                ModelState.AddModelError(nameof(dto.DatumIsteka),
                    "Datum isteka mora biti nakon datuma početka.");

            if (!ModelState.IsValid)
            {
                ViewBag.Postoji = await _clanarinaRepo.GetByKorisnikAsync(dto.KorisnikId) is not null;
                return View(dto);
            }

            var korisnik = await _korisnikRepository.GetByIdAsync(dto.KorisnikId);
            if (korisnik == null) return NotFound();

            var postojeca = await _clanarinaRepo.GetByKorisnikAsync(dto.KorisnikId);

            if (postojeca is null)
            {
                await _clanarinaRepo.CreateAsync(new Clanarina
                {
                    KorisnikId = dto.KorisnikId,
                    DatumPocetka = dto.DatumPocetka,
                    DatumIsteka = dto.DatumIsteka
                });
            }
            else
            {
                postojeca.DatumPocetka = dto.DatumPocetka;
                postojeca.DatumIsteka = dto.DatumIsteka;
                await _clanarinaRepo.UpdateAsync(postojeca);
            }

            if (!string.Equals(korisnik.Status, "aktivan", StringComparison.OrdinalIgnoreCase))
            {
                korisnik.Status = "aktivan";
                korisnik.DatumDeaktivacije = null;
                await _korisnikRepository.UpdateAsync(korisnik);
            }

            TempData["Uspjeh"] = "Članarina je uspješno spremljena.";
            return RedirectToAction("ProfilClana", "Korisnik", new { id = dto.KorisnikId });
        }

        // ═════════════════════════════════════════════════════════════════
        // BIBLIOTEKAR / ADMINISTRATOR — PB-48: Obrada zahtjeva za produženje
        // ═════════════════════════════════════════════════════════════════

        // GET: /Clanarina/ZahtjeviProduzenja
        // Pregled svih zahtjeva na čekanju (US-97)
        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        [HttpGet]
        public async Task<IActionResult> ZahtjeviProduzenja()
        {
            var zahtjeviEntiteti = await _zahtjevRepo.GetNaCekanjuAsync();

            var dtos = new List<ZahtjevProduzenjaDto>();
            foreach (var z in zahtjeviEntiteti)
            {
                var clanarina = await _clanarinaRepo.GetByKorisnikAsync(z.KorisnikId);

                // Izračunaj procijenjeni novi datum isteka
                var bazaDatum = (clanarina?.DatumIsteka ?? DateTime.UtcNow).Date;
                if (bazaDatum < DateTime.UtcNow.Date) bazaDatum = DateTime.UtcNow.Date;
                var procijenjeni = bazaDatum.AddMonths(z.TrajanjeMjeseci);

                dtos.Add(new ZahtjevProduzenjaDto
                {
                    Id = z.Id,
                    KorisnikId = z.KorisnikId,
                    ImeClana = z.Korisnik?.Ime ?? "",
                    PrezimeClana = z.Korisnik?.Prezime ?? "",
                    EmailClana = z.Korisnik?.Email ?? "",
                    TrajanjeMjeseci = z.TrajanjeMjeseci,
                    Napomena = z.Napomena,
                    Status = z.Status,
                    DatumPodnosenja = z.DatumPodnosenja,
                    TrenutniDatumIsteka = clanarina?.DatumIsteka,
                    TrenutniStatus = clanarina?.Status,
                    ProcijenjeniNoviDatumIsteka = procijenjeni
                });
            }

            var vm = new ZahtjeviProduzenjaViewModel { NaCekanju = dtos };
            return View(vm);
        }

        // POST: /Clanarina/ObradiZahtjev
        // Odobrava ili odbija zahtjev (US-97)
        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ObradiZahtjev(ZahtjevProduzenjaObradiDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Greska"] = "Neispravan zahtjev.";
                return RedirectToAction(nameof(ZahtjeviProduzenja));
            }

            var zahtjev = await _zahtjevRepo.GetByIdAsync(dto.ZahtjevId);
            if (zahtjev is null || zahtjev.Status != "na_cekanju")
            {
                TempData["Greska"] = "Zahtjev nije pronađen ili je već obrađen.";
                return RedirectToAction(nameof(ZahtjeviProduzenja));
            }

            // Čitamo ID bibliotekara iz claims
            var bibliotekarbIdStr = User.FindFirst("korisnikId")?.Value
                                 ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(bibliotekarbIdStr, out var bibliotekarbId);

            if (dto.Akcija == "odobreno")
            {
                // Ažuriraj ili kreiraj članarinu
                var clanarina = await _clanarinaRepo.GetByKorisnikAsync(zahtjev.KorisnikId);

                DateTime bazaDatum;
                if (clanarina is null)
                {
                    // Nema članarine — kreiraj novu od danas
                    bazaDatum = DateTime.UtcNow.Date;
                }
                else
                {
                    // Produžavamo od datuma isteka (ili od danas ako je istekla)
                    bazaDatum = clanarina.DatumIsteka.Date < DateTime.UtcNow.Date
                        ? DateTime.UtcNow.Date
                        : clanarina.DatumIsteka.Date;
                }

                var noviDatumIsteka = bazaDatum.AddMonths(zahtjev.TrajanjeMjeseci);

                if (clanarina is null)
                {
                    await _clanarinaRepo.CreateAsync(new Clanarina
                    {
                        KorisnikId = zahtjev.KorisnikId,
                        DatumPocetka = DateTime.UtcNow.Date,
                        DatumIsteka = noviDatumIsteka
                    });
                }
                else
                {
                    clanarina.DatumIsteka = noviDatumIsteka;
                    if (clanarina.DatumPocetka > noviDatumIsteka)
                        clanarina.DatumPocetka = DateTime.UtcNow.Date;
                    await _clanarinaRepo.UpdateAsync(clanarina);
                }

                // Ažuriraj zahtjev
                zahtjev.Status = "odobreno";
                zahtjev.NoviDatumIsteka = noviDatumIsteka;
                zahtjev.DatumObrade = DateTime.UtcNow;
                zahtjev.ObradioKorisnikId = bibliotekarbId > 0 ? bibliotekarbId : null;

                await _zahtjevRepo.UpdateAsync(zahtjev);
                TempData["Uspjeh"] = $"Zahtjev je odobren. Nova članarina važi do {noviDatumIsteka:dd.MM.yyyy}.";
            }
            else if (dto.Akcija == "odbijeno")
            {
                if (string.IsNullOrWhiteSpace(dto.RazlogOdbijanja))
                {
                    TempData["Greska"] = "Razlog odbijanja je obavezan.";
                    return RedirectToAction(nameof(ZahtjeviProduzenja));
                }

                zahtjev.Status = "odbijeno";
                zahtjev.RazlogOdbijanja = dto.RazlogOdbijanja?.Trim();
                zahtjev.DatumObrade = DateTime.UtcNow;
                zahtjev.ObradioKorisnikId = bibliotekarbId > 0 ? bibliotekarbId : null;

                await _zahtjevRepo.UpdateAsync(zahtjev);
                TempData["Uspjeh"] = "Zahtjev je odbijen.";
            }
            else
            {
                TempData["Greska"] = "Nepoznata akcija.";
            }

            return RedirectToAction(nameof(ZahtjeviProduzenja));
        }

        // ═════════════════════════════════════════════════════════════════
        // ČLAN — PB-48: Podnošenje i pregled zahtjeva
        // ═════════════════════════════════════════════════════════════════

        // GET: /Clanarina/ProduzenjeClanarine
        // Stranica za produženje sa strane člana (US-95, US-96, US-98)
        [Authorize(Roles = RoleNames.Clan)]
        [HttpGet]
        public async Task<IActionResult> ProduzenjeClanarine()
        {
            var korisnikId = DohvatiTrenutniKorisnikId();
            if (korisnikId is null) return Unauthorized();

            var clanarina = await _clanarinaRepo.GetByKorisnikAsync(korisnikId.Value);
            var aktivniZahtjev = await _zahtjevRepo.GetAktivniZahtjevAsync(korisnikId.Value);
            var historija = await _zahtjevRepo.GetZahtjeviByKorisnikAsync(korisnikId.Value);

            // Datum od kojeg se računa produženje (za preview)
            DateTime bazaDatum;
            if (clanarina is null)
                bazaDatum = DateTime.UtcNow.Date;
            else
                bazaDatum = clanarina.DatumIsteka.Date < DateTime.UtcNow.Date
                    ? DateTime.UtcNow.Date
                    : clanarina.DatumIsteka.Date;

            var vm = new ProduzenjeViewModel
            {
                KorisnikId = korisnikId.Value,
                TrenutniDatumIsteka = clanarina?.DatumIsteka,
                TrenutniStatus = clanarina?.Status,
                ImaClanarinu = clanarina is not null,
                AktivniZahtjev = aktivniZahtjev is null ? null : new ZahtjevProduzenjaDto
                {
                    Id = aktivniZahtjev.Id,
                    KorisnikId = aktivniZahtjev.KorisnikId,
                    TrajanjeMjeseci = aktivniZahtjev.TrajanjeMjeseci,
                    Napomena = aktivniZahtjev.Napomena,
                    Status = aktivniZahtjev.Status,
                    DatumPodnosenja = aktivniZahtjev.DatumPodnosenja,
                    ProcijenjeniNoviDatumIsteka = bazaDatum.AddMonths(aktivniZahtjev.TrajanjeMjeseci)
                },
                Historija = historija.Select(z => new ZahtjevProduzenjaDto
                {
                    Id = z.Id,
                    KorisnikId = z.KorisnikId,
                    TrajanjeMjeseci = z.TrajanjeMjeseci,
                    Napomena = z.Napomena,
                    Status = z.Status,
                    DatumPodnosenja = z.DatumPodnosenja,
                    DatumObrade = z.DatumObrade,
                    RazlogOdbijanja = z.RazlogOdbijanja,
                    NoviDatumIsteka = z.NoviDatumIsteka,
                    ProcijenjeniNoviDatumIsteka = bazaDatum.AddMonths(z.TrajanjeMjeseci)
                })
            };

            return View(vm);
        }

        // POST: /Clanarina/PodnesizahtjevProduzenja
        // Podnošenje novog zahtjeva (US-96)
        [Authorize(Roles = RoleNames.Clan)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PodnesizahtjevProduzenja(ZahtjevProduzenjaCreateDto dto)
        {
            var korisnikId = DohvatiTrenutniKorisnikId();
            if (korisnikId is null) return Unauthorized();

            // Provjera: nema već aktivnog zahtjeva
            var postojeci = await _zahtjevRepo.GetAktivniZahtjevAsync(korisnikId.Value);
            if (postojeci is not null)
            {
                TempData["Greska"] = "Već imate zahtjev na čekanju. Pričekajte obradu.";
                return RedirectToAction(nameof(ProduzenjeClanarine));
            }

            // Validacija trajanja (1, 3, 6, 12)
            int[] dozvoljenaTrajanja = [1, 3, 6, 12];
            if (!dozvoljenaTrajanja.Contains(dto.TrajanjeMjeseci))
                ModelState.AddModelError(nameof(dto.TrajanjeMjeseci),
                    "Odaberite trajanje: 1, 3, 6 ili 12 mjeseci.");

            if (!ModelState.IsValid)
            {
                TempData["Greska"] = "Neispravan unos. Odaberite trajanje produženja.";
                return RedirectToAction(nameof(ProduzenjeClanarine));
            }

            await _zahtjevRepo.CreateAsync(new ZahtjevProduzenja
            {
                KorisnikId = korisnikId.Value,
                TrajanjeMjeseci = dto.TrajanjeMjeseci,
                Napomena = dto.Napomena?.Trim()
            });

            TempData["Uspjeh"] = "Zahtjev je uspješno poslan. Bibliotekar će ga obraditi.";
            return RedirectToAction(nameof(ProduzenjeClanarine));
        }

        // ── Private helper ────────────────────────────────────────────────
        private int? DohvatiTrenutniKorisnikId()
        {
            var claim = User.FindFirst("korisnikId")
                     ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (claim is null) return null;
            return int.TryParse(claim.Value, out var id) ? id : null;
        }
    }
}