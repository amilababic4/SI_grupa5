using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Services;

namespace SmartLib.Web.Controllers
{
    [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
    public class KategorijaController : Controller
    {
        private readonly IKategorijaRepository _kategorijaRepository;
        private readonly IDistributedCache _cache;
        private readonly CacheVersionStore _cacheVersions;
        private static readonly TimeSpan CategoriesCacheTtl = TimeSpan.FromMinutes(5);

        public KategorijaController(
            IKategorijaRepository kategorijaRepository,
            IDistributedCache cache,
            CacheVersionStore cacheVersions)
        {
            _kategorijaRepository = kategorijaRepository;
            _cache = cache;
            _cacheVersions = cacheVersions;
        }

        // US-31: Prikaz liste svih kategorija
        public async Task<IActionResult> Index()
        {
            var version = _cacheVersions.CategoriesVersion;
            var cacheKey = CacheKeyBuilder.CatalogCategoriesKey(version);
            Response.Headers.Append("X-Cache-Version", version.ToString());

            var cached = await _cache.GetRecordAsync<List<Kategorija>>(cacheKey);
            if (cached != null)
                return View(cached);

            var kategorije = (await _kategorijaRepository.GetAllAsync()).ToList();
            await _cache.SetRecordAsync(cacheKey, kategorije, CategoriesCacheTtl);
            return View(kategorije);
        }

        // US-30: Dodavanje kategorije (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string naziv, string? opis)
        {
            if (string.IsNullOrWhiteSpace(naziv))
            {
                TempData["ErrorMessage"] = "Naziv kategorije je obavezan.";
                return RedirectToAction(nameof(Index));
            }

            naziv = naziv.Trim();

            // US-30 AC: Kada kategorija već postoji, prikazuje se poruka o grešci
            var sve = await _kategorijaRepository.GetAllAsync();
            if (sve.Any(k => k.Naziv.Equals(naziv, StringComparison.OrdinalIgnoreCase)))
            {
                TempData["ErrorMessage"] = $"Kategorija \"{naziv}\" već postoji u sistemu.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _kategorijaRepository.CreateAsync(new Kategorija
                {
                    Naziv = naziv,
                    Opis = string.IsNullOrWhiteSpace(opis) ? null : opis.Trim()
                });

                TempData["SuccessMessage"] = $"Kategorija \"{naziv}\" je uspješno dodana.";
                _cacheVersions.BumpCategoriesVersion();
                _cacheVersions.BumpBooksVersion();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[KategorijaController.Create] Greška: {ex.Message}");
                TempData["ErrorMessage"] = "Greška pri dodavanju kategorije.";
            }

            return RedirectToAction(nameof(Index));
        }

        // US-33: Uređivanje kategorije (GET) — vraća JSON za inline formu
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var kategorija = await _kategorijaRepository.GetByIdAsync(id);
            if (kategorija == null) return NotFound();
            return Json(new { kategorija.Id, kategorija.Naziv, kategorija.Opis });
        }

        // US-33: Uređivanje kategorije (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string naziv, string? opis)
        {
            // US-33 AC: Kada naziv je prazan, prikazuje se greška
            if (string.IsNullOrWhiteSpace(naziv))
            {
                TempData["ErrorMessage"] = "Naziv kategorije ne smije biti prazan.";
                return RedirectToAction(nameof(Index));
            }

            naziv = naziv.Trim();

            var kategorija = await _kategorijaRepository.GetByIdAsync(id);
            if (kategorija == null) return NotFound();

            // US-33 AC: Kada naziv već postoji, sistem odbija izmjenu
            var sve = await _kategorijaRepository.GetAllAsync();
            if (sve.Any(k => k.Id != id && k.Naziv.Equals(naziv, StringComparison.OrdinalIgnoreCase)))
            {
                TempData["ErrorMessage"] = $"Kategorija \"{naziv}\" već postoji u sistemu.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                kategorija.Naziv = naziv;
                kategorija.Opis = string.IsNullOrWhiteSpace(opis) ? null : opis.Trim();
                await _kategorijaRepository.UpdateAsync(kategorija);

                TempData["SuccessMessage"] = $"Kategorija \"{naziv}\" je uspješno ažurirana.";
                _cacheVersions.BumpCategoriesVersion();
                _cacheVersions.BumpBooksVersion();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[KategorijaController.Edit] Greška: {ex.Message}");
                TempData["ErrorMessage"] = "Greška pri ažuriranju kategorije.";
            }

            return RedirectToAction(nameof(Index));
        }

        // US-34 + US-32: Brisanje kategorije (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var kategorija = await _kategorijaRepository.GetByIdAsync(id);
            if (kategorija == null)
            {
                TempData["ErrorMessage"] = "Kategorija nije pronađena.";
                return RedirectToAction(nameof(Index));
            }

            // US-32 AC: Zabrana brisanja kategorije koja ima povezane knjige
            if (await _kategorijaRepository.HasBooksAsync(id))
            {
                TempData["ErrorMessage"] =
                    $"Kategorija \"{kategorija.Naziv}\" ima povezane knjige i ne može biti obrisana. " +
                    "Prvo premjestite knjige u drugu kategoriju.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _kategorijaRepository.DeleteAsync(id);
                TempData["SuccessMessage"] = $"Kategorija \"{kategorija.Naziv}\" je uspješno obrisana.";
                _cacheVersions.BumpCategoriesVersion();
                _cacheVersions.BumpBooksVersion();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[KategorijaController.Delete] Greška: {ex.Message}");
                TempData["ErrorMessage"] = "Greška pri brisanju kategorije.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}