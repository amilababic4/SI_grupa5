using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Web.Models;
using System.Security.Claims;

namespace SmartLib.Web.Controllers
{
    [Authorize(Roles = RoleNames.Clan)]
    public class ListaKolekcijaController : Controller
    {
        private readonly IListaKolekcijaRepository _listaKolekcijaRepository;

        public ListaKolekcijaController(IListaKolekcijaRepository listaKolekcijaRepository)
        {
            _listaKolekcijaRepository = listaKolekcijaRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? q = null, string? visibility = "all", string? sort = "updated")
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Forbid();
            await _listaKolekcijaRepository.EnsureWishlistCollectionAsync(userId.Value);
            var lists = await _listaKolekcijaRepository.GetByKorisnikAsync(userId.Value);
            var totalCount = lists.Count;

            var filtered = ApplyFilters(lists, q, visibility);
            var mapped = filtered.Select(l => new ListaKolekcijaCardViewModel
            {
                Id = l.Id,
                Naziv = l.Naziv,
                Opis = l.Opis,
                Javna = l.Javna,
                BrojStavki = l.Stavke?.Count ?? 0,
                DatumKreiranja = l.DatumKreiranja,
                DatumAzuriranja = l.DatumAzuriranja,
                IsWishlist = string.Equals(l.Naziv, "Lista želja", StringComparison.Ordinal)
            }).ToList();

            var sorted = ApplySort(mapped, sort);

            var model = new ListaKolekcijaListViewModel
            {
                Kolekcije = sorted,
                Query = q,
                Visibility = string.IsNullOrWhiteSpace(visibility) ? "all" : visibility,
                Sort = string.IsNullOrWhiteSpace(sort) ? "updated" : sort,
                TotalCount = totalCount
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ListaKolekcijaListViewModel model, int? knjigaId = null, string? returnUrl = null)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Forbid();

            if (string.IsNullOrWhiteSpace(model.NewNaziv))
            {
                TempData["ErrorMessage"] = "Naziv kolekcije je obavezan.";
                return RedirectToAction(nameof(Index));
            }

            var lista = new ListaKolekcija
            {
                KorisnikId = userId.Value,
                Naziv = model.NewNaziv.Trim(),
                Opis = model.NewOpis?.Trim(),
                Javna = model.NewJavna,
                DatumKreiranja = DateTime.UtcNow
            };

            await _listaKolekcijaRepository.CreateAsync(lista);
            if (knjigaId.HasValue)
            {
                await _listaKolekcijaRepository.AddItemAsync(lista.Id, knjigaId.Value);
            }
            TempData["SuccessMessage"] = "Kolekcija je kreirana.";
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Forbid();

            var lista = await _listaKolekcijaRepository.GetByIdAsync(id, includeItems: true);
            if (lista == null || lista.KorisnikId != userId.Value) return NotFound();

            var model = BuildDetailsModel(lista, true);
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> PublicIndex(string? q = null, string? sort = "updated")
        {
            var lists = await _listaKolekcijaRepository.GetAllPublicAsync(q, sort);

            var model = new JavneKolekcijeListViewModel
            {
                Query = q,
                Sort = string.IsNullOrWhiteSpace(sort) ? "updated" : sort,
                TotalCount = lists.Count,
                Kolekcije = lists.Select(l => new JavneKolekcijeCardViewModel
                {
                    Id = l.Id,
                    Naziv = l.Naziv,
                    Opis = l.Opis,
                    BrojStavki = l.Stavke?.Count ?? 0,
                    DatumKreiranja = l.DatumKreiranja,
                    DatumAzuriranja = l.DatumAzuriranja,
                    KorisnikId = l.KorisnikId,
                    KorisnikIme = l.Korisnik != null
                        ? $"{l.Korisnik.Ime} {l.Korisnik.Prezime}"
                        : "—"
                }).ToList()
            };

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Public(int id)
        {
            var lista = await _listaKolekcijaRepository.GetByIdAsync(id, includeItems: true);
            if (lista == null || !lista.Javna) return NotFound();

            var isOwner = false;
            var userId = GetUserId();
            if (userId.HasValue && userId.Value == lista.KorisnikId)
            {
                isOwner = true;
            }

            var model = BuildDetailsModel(lista, isOwner);
            return View("Details", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePublic(int id)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Forbid();

            var lista = await _listaKolekcijaRepository.GetByIdAsync(id);
            if (lista == null || lista.KorisnikId != userId.Value) return NotFound();

            lista.Javna = !lista.Javna;
            lista.DatumAzuriranja = DateTime.UtcNow;
            await _listaKolekcijaRepository.UpdateAsync(lista);

            TempData["SuccessMessage"] = lista.Javna
                ? "Kolekcija je sada javna."
                : "Kolekcija je sada privatna.";

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Forbid();

            var lista = await _listaKolekcijaRepository.GetByIdAsync(id);
            if (lista != null && string.Equals(lista.Naziv, "Lista želja", StringComparison.Ordinal))
            {
                TempData["ErrorMessage"] = "Lista želja se ne može obrisati.";
                return RedirectToAction(nameof(Index));
            }

            var ok = await _listaKolekcijaRepository.DeleteAsync(id, userId.Value);
            TempData["SuccessMessage"] = ok ? "Kolekcija je obrisana." : "Kolekcija nije pronađena.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(int listaId, int knjigaId, string? returnUrl = null)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Forbid();

            var lista = await _listaKolekcijaRepository.GetByIdAsync(listaId);
            if (lista == null || lista.KorisnikId != userId.Value) return NotFound();

            await _listaKolekcijaRepository.AddItemAsync(listaId, knjigaId);
            TempData["SuccessMessage"] = "Knjiga je dodana u kolekciju.";

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Details), new { id = listaId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItemAjax(int listaId, int knjigaId)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Forbid();

            var lista = await _listaKolekcijaRepository.GetByIdAsync(listaId);
            if (lista == null || lista.KorisnikId != userId.Value) return NotFound();

            var alreadyAdded = await _listaKolekcijaRepository.HasItemAsync(listaId, knjigaId);
            if (!alreadyAdded)
            {
                await _listaKolekcijaRepository.AddItemAsync(listaId, knjigaId);
            }

            var message = alreadyAdded
                ? "Knjiga je već u kolekciji."
                : "Knjiga je dodana u kolekciju.";

            return Json(new { added = !alreadyAdded, message });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int listaId, int knjigaId)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Forbid();

            var lista = await _listaKolekcijaRepository.GetByIdAsync(listaId);
            if (lista == null || lista.KorisnikId != userId.Value) return NotFound();

            await _listaKolekcijaRepository.RemoveItemAsync(listaId, knjigaId);
            TempData["SuccessMessage"] = "Knjiga je uklonjena iz kolekcije.";
            return RedirectToAction(nameof(Details), new { id = listaId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reorder([FromBody] ReorderRequest request)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Forbid();

            var lista = await _listaKolekcijaRepository.GetByIdAsync(request.ListaId);
            if (lista == null || lista.KorisnikId != userId.Value) return NotFound();

            if (request.StavkaIds == null || request.StavkaIds.Count == 0)
            {
                return BadRequest();
            }

            await _listaKolekcijaRepository.UpdateOrderAsync(request.ListaId, request.StavkaIds);
            return Ok();
        }

        private static ListaKolekcijaDetailsViewModel BuildDetailsModel(ListaKolekcija lista, bool isOwner)
        {
            return new ListaKolekcijaDetailsViewModel
            {
                Id = lista.Id,
                Naziv = lista.Naziv,
                Opis = lista.Opis,
                Javna = lista.Javna,
                IsOwner = isOwner,
                Stavke = lista.Stavke
                    .OrderBy(s => s.Redoslijed)
                    .Select(s => new ListaKolekcijaItemViewModel
                    {
                        StavkaId = s.Id,
                        KnjigaId = s.KnjigaId,
                        Naslov = s.Knjiga?.Naslov ?? "—",
                        Autor = s.Knjiga?.Autor ?? "—",
                        Isbn = s.Knjiga?.Isbn,
                        GodinaIzdanja = s.Knjiga?.GodinaIzdanja,
                        DatumDodavanja = s.DatumDodavanja,
                        SlikaUrl = s.Knjiga?.SlikaUrl,
                        BrojDostupnih = s.Knjiga?.Primjerci?.Count(p => p.Status == "dostupan") ?? 0
                    })
                    .ToList()
            };
        }

        private int? GetUserId()
        {
            var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(idValue, out var id)) return id;
            return null;
        }

        private static List<ListaKolekcija> ApplyFilters(List<ListaKolekcija> lists, string? query, string? visibility)
        {
            var result = lists.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.Trim().ToLowerInvariant();
                result = result.Where(l =>
                    (!string.IsNullOrWhiteSpace(l.Naziv) && l.Naziv.ToLowerInvariant().Contains(q)) ||
                    (!string.IsNullOrWhiteSpace(l.Opis) && l.Opis.ToLowerInvariant().Contains(q)));
            }

            if (!string.IsNullOrWhiteSpace(visibility))
            {
                var normalized = visibility.Trim().ToLowerInvariant();
                if (normalized == "public")
                {
                    result = result.Where(l => l.Javna);
                }
                else if (normalized == "private")
                {
                    result = result.Where(l => !l.Javna);
                }
            }

            return result.ToList();
        }

        private static List<ListaKolekcijaCardViewModel> ApplySort(List<ListaKolekcijaCardViewModel> lists, string? sort)
        {
            var sorted = (sort ?? string.Empty).Trim().ToLowerInvariant() switch
            {
                "name" => lists.OrderBy(l => l.Naziv).ToList(),
                "items" => lists.OrderByDescending(l => l.BrojStavki).ThenBy(l => l.Naziv).ToList(),
                "created" => lists.OrderByDescending(l => l.DatumKreiranja).ToList(),
                _ => lists.OrderByDescending(l => l.DatumAzuriranja ?? l.DatumKreiranja).ToList()
            };

            return sorted
                .OrderByDescending(l => l.IsWishlist)
                .ThenBy(l => l.Naziv)
                .ToList();
        }

        public class ReorderRequest
        {
            public int ListaId { get; set; }
            public List<int> StavkaIds { get; set; } = new();
        }
    }
}
