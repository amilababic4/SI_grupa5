using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Web.Models;
using System.Security.Claims;

namespace SmartLib.Web.Controllers
{
    [Authorize(Roles = RoleNames.Clan)]
    public class ListaZeljaController : Controller
    {
        private readonly IKorisnikRepository _korisnikRepository;
        private readonly IListaKolekcijaRepository _listaKolekcijaRepository;

        public ListaZeljaController(
            IKorisnikRepository korisnikRepository,
            IListaKolekcijaRepository listaKolekcijaRepository)
        {
            _korisnikRepository = korisnikRepository;
            _listaKolekcijaRepository = listaKolekcijaRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string sort = "added")
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Forbid();
            var wishlist = await _listaKolekcijaRepository.EnsureWishlistCollectionAsync(userId.Value);
            return RedirectToAction("Details", "ListaKolekcija", new { id = wishlist.Id });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Public(int id, string sort = "added")
        {
            var korisnik = await _korisnikRepository.GetByIdAsync(id);
            if (korisnik == null) return NotFound();

            var wishlist = await _listaKolekcijaRepository.EnsureWishlistCollectionAsync(id);
            if (!wishlist.Javna) return NotFound();

            return RedirectToAction("Public", "ListaKolekcija", new { id = wishlist.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dodaj(int knjigaId, string? returnUrl = null)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Forbid();

            var wishlist = await _listaKolekcijaRepository.EnsureWishlistCollectionAsync(userId.Value);
            await _listaKolekcijaRepository.AddItemAsync(wishlist.Id, knjigaId);
            TempData["SuccessMessage"] = "Knjiga je dodana u listu želja.";

            return SafeRedirect(returnUrl, knjigaId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ukloni(int knjigaId, string? returnUrl = null)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Forbid();

            var wishlist = await _listaKolekcijaRepository.EnsureWishlistCollectionAsync(userId.Value);
            await _listaKolekcijaRepository.RemoveItemAsync(wishlist.Id, knjigaId);
            TempData["SuccessMessage"] = "Knjiga je uklonjena sa liste želja.";

            return SafeRedirect(returnUrl, knjigaId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePublic()
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Forbid();

            var korisnik = await _korisnikRepository.GetByIdAsync(userId.Value);
            if (korisnik == null) return NotFound();

            korisnik.ListaZeljaJavna = !korisnik.ListaZeljaJavna;
            await _korisnikRepository.UpdateAsync(korisnik);

            var wishlist = await _listaKolekcijaRepository.EnsureWishlistCollectionAsync(userId.Value);
            wishlist.Javna = korisnik.ListaZeljaJavna;
            wishlist.DatumAzuriranja = DateTime.UtcNow;
            await _listaKolekcijaRepository.UpdateAsync(wishlist);

            TempData["SuccessMessage"] = korisnik.ListaZeljaJavna
                ? "Lista želja je sada javna."
                : "Lista želja je sada privatna.";

            return RedirectToAction("Details", "ListaKolekcija", new { id = wishlist.Id });
        }

        private ListaZeljaViewModel BuildViewModel(
            Korisnik korisnik,
            List<Core.Models.ListaZeljaStavka> items,
            string sort,
            bool isOwner)
        {
            var sorted = SortItems(items, sort);

            return new ListaZeljaViewModel
            {
                OwnerId = korisnik.Id,
                OwnerName = $"{korisnik.Ime} {korisnik.Prezime}",
                IsOwner = isOwner,
                IsPublic = korisnik.ListaZeljaJavna,
                Sort = sort,
                Items = sorted.Select(i => new ListaZeljaItemViewModel
                {
                    KnjigaId = i.KnjigaId,
                    Naslov = i.Knjiga?.Naslov ?? "—",
                    Autor = i.Knjiga?.Autor ?? "—",
                    Isbn = i.Knjiga?.Isbn,
                    GodinaIzdanja = i.Knjiga?.GodinaIzdanja,
                    DatumDodavanja = i.DatumDodavanja,
                    SlikaUrl = i.Knjiga?.SlikaUrl,
                    BrojDostupnih = i.Knjiga?.Primjerci?.Count(p => p.Status == "dostupan") ?? 0
                }).ToList()
            };
        }

        private static List<Core.Models.ListaZeljaStavka> SortItems(
            List<Core.Models.ListaZeljaStavka> items,
            string sort)
        {
            return sort switch
            {
                "random" => items
                    .OrderBy(_ => Random.Shared.Next())
                    .ToList(),
                "year" => items
                    .OrderByDescending(i => i.Knjiga?.GodinaIzdanja ?? 0)
                    .ThenByDescending(i => i.DatumDodavanja)
                    .ToList(),
                _ => items
                    .OrderByDescending(i => i.DatumDodavanja)
                    .ToList()
            };
        }

        private int? GetUserId()
        {
            var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(idValue, out var id)) return id;
            return null;
        }

        private IActionResult SafeRedirect(string? returnUrl, int knjigaId)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Details", "Knjiga", new { id = knjigaId });
        }
    }
}
