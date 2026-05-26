using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Services;
using SmartLib.Web.Models;

namespace SmartLib.Web.Controllers
{
    [Authorize(Roles = "Bibliotekar,Administrator")]
    public class PrijaveController : Controller
    {
        private readonly IRecenzijaPrijavaRepository _recenzijaPrijavaRepo;
        private readonly IForumRepository _forumRepo;
        private readonly IKorisnikRepository _korisnikRepo;
        private readonly INotifikacijaRepository _notifikacijaRepo;
        private readonly CacheVersionStore _cacheVersions;

        public PrijaveController(
            IRecenzijaPrijavaRepository recenzijaPrijavaRepo,
            IForumRepository forumRepo,
            IKorisnikRepository korisnikRepo,
            INotifikacijaRepository notifikacijaRepo,
            CacheVersionStore cacheVersions)
        {
            _recenzijaPrijavaRepo = recenzijaPrijavaRepo;
            _forumRepo = forumRepo;
            _korisnikRepo = korisnikRepo;
            _notifikacijaRepo = notifikacijaRepo;
            _cacheVersions = cacheVersions;
        }

        public async Task<IActionResult> Index()
        {
            var stavke = new List<PrijavaListItem>();

            var recenzije = await _recenzijaPrijavaRepo.GetOpenAsync();
            foreach (var p in recenzije)
            {
                stavke.Add(new PrijavaListItem
                {
                    Tip = "Recenzija",
                    Naslov = p.Recenzija?.Knjiga?.Naslov ?? "Knjiga",
                    Autor = $"{p.Recenzija?.Korisnik?.Ime} {p.Recenzija?.Korisnik?.Prezime}".Trim(),
                    Prijavio = $"{p.PrijavioKorisnik?.Ime} {p.PrijavioKorisnik?.Prezime}".Trim(),
                    Razlog = string.IsNullOrWhiteSpace(p.Razlog) ? "—" : p.Razlog,
                    Datum = p.DatumKreiranja,
                    LinkUrl = p.Recenzija?.KnjigaId is int knjigaId
                        ? Url.Action("Details", "Knjiga", new { id = knjigaId })
                        : null,
                    PrijavaId = p.Id,
                    Source = "recenzija"
                });
            }

            var komentarPrijave = await _forumRepo.GetOpenKomentarPrijaveAsync();
            foreach (var p in komentarPrijave)
            {
                stavke.Add(new PrijavaListItem
                {
                    Tip = "Forum komentar",
                    Naslov = p.Komentar?.Objava?.Naslov ?? "Forum objava",
                    Autor = $"{p.Komentar?.Korisnik?.Ime} {p.Komentar?.Korisnik?.Prezime}".Trim(),
                    Prijavio = $"{p.PrijavioKorisnik?.Ime} {p.PrijavioKorisnik?.Prezime}".Trim(),
                    Razlog = string.IsNullOrWhiteSpace(p.Razlog) ? "—" : p.Razlog,
                    Datum = p.DatumKreiranja,
                    LinkUrl = p.Komentar?.ObjavaId is int objavaId
                        ? Url.Action("Details", "Forum", new { id = objavaId })
                        : null,
                    PrijavaId = p.Id,
                    Source = "forum-komentar"
                });
            }

            var objavaPrijave = await _forumRepo.GetOpenObjavaPrijaveAsync();
            foreach (var p in objavaPrijave)
            {
                stavke.Add(new PrijavaListItem
                {
                    Tip = "Forum objava",
                    Naslov = p.Objava?.Naslov ?? "Forum objava",
                    Autor = $"{p.Objava?.Korisnik?.Ime} {p.Objava?.Korisnik?.Prezime}".Trim(),
                    Prijavio = $"{p.PrijavioKorisnik?.Ime} {p.PrijavioKorisnik?.Prezime}".Trim(),
                    Razlog = string.IsNullOrWhiteSpace(p.Razlog) ? "—" : p.Razlog,
                    Datum = p.DatumKreiranja,
                    LinkUrl = p.Objava?.Id is int objavaId
                        ? Url.Action("Details", "Forum", new { id = objavaId })
                        : null,
                    PrijavaId = p.Id,
                    Source = "forum-objava"
                });
            }

            var vm = new PrijaveViewModel
            {
                Stavke = stavke.OrderByDescending(s => s.Datum).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Razrijesi(int prijavaId, string source)
        {
            var uId = GetUserId();
            if (!uId.HasValue) return Challenge();

            var resolved = false;
            string? resolvedBy = null;
            string tip = "Prijava";
            string naslov = "";

            if (source == "recenzija")
            {
                resolved = await _recenzijaPrijavaRepo.TryResolveAsync(prijavaId, uId.Value);
                var prijava = await _recenzijaPrijavaRepo.GetByIdAsync(prijavaId);
                resolvedBy = prijava?.RazrijesioKorisnik != null
                    ? $"{prijava.RazrijesioKorisnik.Ime} {prijava.RazrijesioKorisnik.Prezime}"
                    : null;
                tip = "Recenzija";
                naslov = prijava?.Recenzija?.Knjiga?.Naslov ?? "Knjiga";
            }
            else if (source == "forum-komentar")
            {
                resolved = await _forumRepo.TryResolveKomentarPrijavaAsync(prijavaId, uId.Value);
                var prijava = await _forumRepo.GetKomentarPrijavaByIdAsync(prijavaId);
                resolvedBy = prijava?.RazrijesioKorisnik != null
                    ? $"{prijava.RazrijesioKorisnik.Ime} {prijava.RazrijesioKorisnik.Prezime}"
                    : null;
                tip = "Forum komentar";
                naslov = prijava?.Komentar?.Objava?.Naslov ?? "Forum objava";
            }
            else if (source == "forum-objava")
            {
                resolved = await _forumRepo.TryResolveObjavaPrijavaAsync(prijavaId, uId.Value);
                var prijava = await _forumRepo.GetObjavaPrijavaByIdAsync(prijavaId);
                resolvedBy = prijava?.RazrijesioKorisnik != null
                    ? $"{prijava.RazrijesioKorisnik.Ime} {prijava.RazrijesioKorisnik.Prezime}"
                    : null;
                tip = "Forum objava";
                naslov = prijava?.Objava?.Naslov ?? "Forum objava";
            }

            var staff = (await _korisnikRepo.GetAllAsync())
                .Where(k => string.Equals(k.Uloga?.Naziv, RoleNames.Administrator, StringComparison.OrdinalIgnoreCase)
                         || string.Equals(k.Uloga?.Naziv, RoleNames.Bibliotekar, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var resolver = staff.FirstOrDefault(k => k.Id == uId.Value);
            var resolverDisplay = resolver != null ? $"{resolver.Ime} {resolver.Prezime}" : "osoblje";
            var notifLink = Url.Action(nameof(Index)) ?? "/Prijave";

            if (!resolved)
            {
                var who = string.IsNullOrWhiteSpace(resolvedBy) ? "drugi korisnik" : resolvedBy;
                await _notifikacijaRepo.AddAsync(new Notifikacija
                {
                    KorisnikId = uId.Value,
                    Naslov = "Prijava već razriješena",
                    Poruka = $"Prijava je već razriješena od: {who}.",
                    Tip = "Prijave",
                    LinkUrl = notifLink,
                    DatumKreiranja = DateTime.UtcNow
                });

                TempData["ErrorMessage"] = "Prijava je već razriješena od drugog korisnika.";
                return RedirectToAction(nameof(Index));
            }

            var notifList = staff.Where(s => s.Id != uId.Value).Select(s => new Notifikacija
            {
                KorisnikId = s.Id,
                Naslov = "Prijava razriješena",
                Poruka = $"{tip} prijava za {naslov} je razriješena od: {resolverDisplay}.",
                Tip = "Prijave",
                LinkUrl = notifLink,
                DatumKreiranja = DateTime.UtcNow
            });

            await _notifikacijaRepo.AddRangeAsync(notifList);

            TempData["SuccessMessage"] = "Prijava je razriješena.";
            return RedirectToAction(nameof(Index));
        }

        private int? GetUserId()
        {
            var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idValue, out var id) ? id : null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UkloniSadrzaj(int prijavaId, string source)
        {
            var uId = GetUserId();
            if (!uId.HasValue) return Challenge();

            bool deleted = false;
            string tip = "";
            string naslov = "";
            int? authorId = null;

            if (source == "forum-komentar")
            {
                var prijava = await _forumRepo.GetKomentarPrijavaByIdAsync(prijavaId);
                if (prijava?.Komentar != null)
                {
                    naslov = prijava.Komentar.Objava?.Naslov ?? "Forum objava";
                    authorId = prijava.Komentar.KorisnikId;
                    deleted = await _forumRepo.DeleteKomentarAsync(prijava.KomentarId);
                    tip = "Forum komentar";
                }
            }
            else if (source == "forum-objava")
            {
                var prijava = await _forumRepo.GetObjavaPrijavaByIdAsync(prijavaId);
                if (prijava?.Objava != null)
                {
                    naslov = prijava.Objava.Naslov;
                    authorId = prijava.Objava.KorisnikId;
                    deleted = await _forumRepo.DeleteObjavaAsync(prijava.ObjavaId);
                    tip = "Forum objava";
                }
            }

            if (!deleted)
            {
                TempData["ErrorMessage"] = "Sadržaj nije moguće ukloniti.";
                return RedirectToAction(nameof(Index));
            }

            if (authorId.HasValue)
            {
                var author = await _korisnikRepo.GetByIdAsync(authorId.Value);
                if (author != null)
                {
                    author.BrojUklonjenihSadrzaja++;
                    if (author.BrojUklonjenihSadrzaja >= 3)
                    {
                        author.DatumZabraneDo = DateTime.UtcNow.AddDays(7);
                        author.BrojUklonjenihSadrzaja = 0;
                    }
                    await _korisnikRepo.UpdateAsync(author);
                }
            }

            _cacheVersions.BumpForumVersion();

            // Auto-resolve the report after deleting the content
            if (source == "forum-komentar")
                await _forumRepo.TryResolveKomentarPrijavaAsync(prijavaId, uId.Value);
            else if (source == "forum-objava")
                await _forumRepo.TryResolveObjavaPrijavaAsync(prijavaId, uId.Value);

            var staff = (await _korisnikRepo.GetAllAsync())
                .Where(k => string.Equals(k.Uloga?.Naziv, RoleNames.Administrator, StringComparison.OrdinalIgnoreCase)
                         || string.Equals(k.Uloga?.Naziv, RoleNames.Bibliotekar, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var resolver = staff.FirstOrDefault(k => k.Id == uId.Value);
            var resolverDisplay = resolver != null ? $"{resolver.Ime} {resolver.Prezime}" : "osoblje";
            var notifLink = Url.Action(nameof(Index)) ?? "/Prijave";

            var notifList = staff.Where(s => s.Id != uId.Value).Select(s => new Notifikacija
            {
                KorisnikId = s.Id,
                Naslov = "Sadržaj uklonjen",
                Poruka = $"{tip} \"{naslov}\" je uklonjen/a od: {resolverDisplay}.",
                Tip = "Prijave",
                LinkUrl = notifLink,
                DatumKreiranja = DateTime.UtcNow
            });

            await _notifikacijaRepo.AddRangeAsync(notifList);

            TempData["SuccessMessage"] = $"{tip} je uspješno uklonjen/a.";
            return RedirectToAction(nameof(Index));
        }
    }
}
