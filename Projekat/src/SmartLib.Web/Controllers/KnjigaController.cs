using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;

namespace SmartLib.Web.Controllers
{
    [Authorize]
    public class KnjigaController : Controller
    {
        private readonly IKnjigaRepository _knjigaRepository;
        private readonly IPrimjerakRepository _primjerakRepository;
        private readonly IKategorijaRepository _kategorijaRepository;

        public KnjigaController(
            IKnjigaRepository knjigaRepository,
            IPrimjerakRepository primjerakRepository,
            IKategorijaRepository kategorijaRepository)
        {
            _knjigaRepository = knjigaRepository;
            _primjerakRepository = primjerakRepository;
            _kategorijaRepository = kategorijaRepository;
        }

        public async Task<IActionResult> Index(string? naslov, string? autor, int page = 1)
        {
            const int pageSize = 10;
            if (page < 1) page = 1;

            var (knjige, ukupno) = await _knjigaRepository.GetPagedAsync(naslov, autor, page, pageSize);

            var dtos = knjige.Select(k => new KnjigaDto
            {
                Id = k.Id,
                Naslov = k.Naslov,
                Autor = k.Autor,
                Isbn = k.Isbn,
                Kategorija = k.Kategorija?.Naziv,
                Izdavac = k.Izdavac,
                GodinaIzdanja = k.GodinaIzdanja,
                BrojPrimjeraka = k.Primjerci.Count,
                BrojDostupnih = k.Primjerci.Count(p => p.Status == "dostupan")
            }).ToList();

            int ukupnoStrana = ukupno == 0 ? 1 : (int)Math.Ceiling((double)ukupno / pageSize);

            var model = new KatalogViewModel
            {
                Knjige = dtos,
                TrenutnaStrana = page,
                UkupnoStrana = ukupnoStrana,
                UkupnoStavki = ukupno,
                VelicinaStrane = pageSize,
                Naslov = naslov,
                Autor = autor
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var knjiga = await _knjigaRepository.GetByIdAsync(id);
            if (knjiga == null) return NotFound();

            var dto = new KnjigaDto
            {
                Id = knjiga.Id,
                Naslov = knjiga.Naslov,
                Autor = knjiga.Autor,
                Isbn = knjiga.Isbn,
                Kategorija = knjiga.Kategorija?.Naziv,
                Izdavac = knjiga.Izdavac,
                GodinaIzdanja = knjiga.GodinaIzdanja,
                BrojPrimjeraka = knjiga.Primjerci.Count,
                BrojDostupnih = knjiga.Primjerci.Count(p => p.Status == "dostupan")
            };

            ViewBag.Primjerci = knjiga.Primjerci.OrderBy(p => p.InventarniBroj).ToList();

            return View(dto);
        }

        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateKategorije();
            return View(new KnjigaCreateDto());
        }

        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        [HttpPost]
        public async Task<IActionResult> Create(KnjigaCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateKategorije(model.KategorijaId);
                return View(model);
            }

            var normalizedIsbn = NormalizeIsbn(model.Isbn);
            if (!IsValidIsbn(normalizedIsbn))
            {
                ModelState.AddModelError(nameof(model.Isbn), "ISBN nije u ispravnom formatu. Unesite 10 ili 13 cifara (sa crticama ili bez).");
                await PopulateKategorije(model.KategorijaId);
                return View(model);
            }

            var existing = await _knjigaRepository.GetByIsbnAsync(normalizedIsbn);
            if (existing != null)
            {
                ModelState.AddModelError(nameof(model.Isbn), "Knjiga sa ovim ISBN-om već postoji u katalogu.");
                await PopulateKategorije(model.KategorijaId);
                return View(model);
            }

            var kategorija = await _kategorijaRepository.GetByIdAsync(model.KategorijaId);
            if (kategorija == null)
            {
                ModelState.AddModelError(nameof(model.KategorijaId), "Odabrana kategorija nije validna.");
                await PopulateKategorije(model.KategorijaId);
                return View(model);
            }

            var knjiga = new Knjiga
            {
                Naslov = model.Naslov.Trim(),
                Autor = model.Autor.Trim(),
                Isbn = normalizedIsbn,
                KategorijaId = model.KategorijaId,
                Izdavac = model.Izdavac?.Trim(),
                GodinaIzdanja = model.GodinaIzdanja
            };

            var savedKnjiga = await _knjigaRepository.CreateAsync(knjiga);

            for (int i = 0; i < model.BrojPrimjeraka; i++)
            {
                await _primjerakRepository.CreateAsync(new Primjerak
                {
                    KnjigaId = savedKnjiga.Id,
                    InventarniBroj = $"INV-{savedKnjiga.Id}-{i + 1:D3}",
                    Status = "dostupan",
                    DatumNabave = DateTime.UtcNow
                });
            }

            TempData["SuccessMessage"] = $"Knjiga \"{savedKnjiga.Naslov}\" je uspješno dodana u katalog.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var knjiga = await _knjigaRepository.GetByIdAsync(id);
            if (knjiga == null) return NotFound();

            var model = new KnjigaEditDto
            {
                Id = knjiga.Id,
                Naslov = knjiga.Naslov,
                Autor = knjiga.Autor,
                KategorijaId = knjiga.KategorijaId,
                Izdavac = knjiga.Izdavac,
                GodinaIzdanja = knjiga.GodinaIzdanja
            };

            ViewBag.Isbn = knjiga.Isbn;
            await PopulateKategorije(knjiga.KategorijaId);
            return View(model);
        }

        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        [HttpPost]
        public async Task<IActionResult> Edit(KnjigaEditDto model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateKategorije(model.KategorijaId);
                return View(model);
            }

            var knjiga = await _knjigaRepository.GetByIdAsync(model.Id);
            if (knjiga == null) return NotFound();

            var kategorija = await _kategorijaRepository.GetByIdAsync(model.KategorijaId);
            if (kategorija == null)
            {
                ModelState.AddModelError(nameof(model.KategorijaId), "Odabrana kategorija nije validna.");
                await PopulateKategorije(model.KategorijaId);
                return View(model);
            }

            knjiga.Naslov = model.Naslov.Trim();
            knjiga.Autor = model.Autor.Trim();
            knjiga.KategorijaId = model.KategorijaId;
            knjiga.Izdavac = model.Izdavac?.Trim();
            knjiga.GodinaIzdanja = model.GodinaIzdanja;

            await _knjigaRepository.UpdateAsync(knjiga);

            TempData["SuccessMessage"] = $"Podaci knjige \"{knjiga.Naslov}\" su uspješno ažurirani.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _knjigaRepository.HasActiveLoansAsync(id))
            {
                TempData["ErrorMessage"] = "Knjiga ima aktivna zaduženja i ne može biti obrisana.";
                return RedirectToAction(nameof(Index));
            }

            await _knjigaRepository.DeleteAsync(id);
            TempData["SuccessMessage"] = "Knjiga je obrisana iz kataloga.";
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateKategorije(int? selectedId = null)
        {
            var kategorije = await _kategorijaRepository.GetAllAsync();
            ViewBag.Kategorije = new SelectList(kategorije, "Id", "Naziv", selectedId);
        }

        private static string NormalizeIsbn(string isbn)
            => isbn.Replace("-", "").Replace(" ", "").Trim();

        private static bool IsValidIsbn(string isbn)
        {
            if (string.IsNullOrEmpty(isbn)) return false;
            if (isbn.Length == 13) return isbn.All(char.IsDigit);
            if (isbn.Length == 10) return isbn[..9].All(char.IsDigit) && (char.IsDigit(isbn[9]) || isbn[9] == 'X');
            return false;
        }
    }
}
