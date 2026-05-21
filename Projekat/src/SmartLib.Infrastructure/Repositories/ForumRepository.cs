using Microsoft.EntityFrameworkCore;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;

namespace SmartLib.Infrastructure.Repositories
{
    public class ForumRepository : IForumRepository
    {
        private readonly ApplicationDbContext _db;

        private static readonly IReadOnlyList<string> _kategorije = new List<string>
        {
            "Opšta diskusija",
            "Preporuke knjiga",
            "Pitanja",
            "Recenzije"
        }.AsReadOnly();

        public ForumRepository(ApplicationDbContext db) => _db = db;

        public IReadOnlyList<string> GetKategorije() => _kategorije;

        // PB-59: filtrirano po kategoriji
        public async Task<IEnumerable<ForumObjava>> GetAllAsync(string? kategorija = null)
        {
            var query = _db.ForumObjave
                .Include(o => o.Korisnik).ThenInclude(k => k!.Uloga)
                .Include(o => o.Komentari)
                .Include(o => o.Reakcije)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(kategorija))
                query = query.Where(o => o.Kategorija == kategorija);

            return await query.OrderByDescending(o => o.DatumKreiranja).ToListAsync();
        }

        // PB-57: detalji objave sa komentarima i reakcijama
        public async Task<ForumObjava?> GetByIdAsync(int id)
        {
            return await _db.ForumObjave
                .Include(o => o.Korisnik).ThenInclude(k => k!.Uloga)
                .Include(o => o.Komentari)
                    .ThenInclude(k => k.Korisnik)
                    .ThenInclude(k => k!.Uloga)
                .Include(o => o.Reakcije)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        // PB-57: kreiranje objave
        public async Task<ForumObjava> CreateAsync(ForumObjava objava)
        {
            _db.ForumObjave.Add(objava);
            await _db.SaveChangesAsync();
            return objava;
        }

        // PB-58: dodaj komentar
        public async Task<ForumKomentar> AddKomentarAsync(ForumKomentar komentar)
        {
            _db.ForumKomentari.Add(komentar);
            await _db.SaveChangesAsync();
            return komentar;
        }

        public async Task<bool> DeleteKomentarAsync(int komentarId)
        {
            var komentar = await _db.ForumKomentari.FirstOrDefaultAsync(k => k.Id == komentarId);
            if (komentar == null)
            {
                return false;
            }

            _db.ForumKomentari.Remove(komentar);
            await _db.SaveChangesAsync();
            return true;
        }

        // PB-60: toggle korisno reakcija
        public async Task<bool> ToggleReakcijaAsync(int objavaId, int korisnikId, string tip = "korisno")
        {
            var existing = await _db.ForumReakcije
                .FirstOrDefaultAsync(r => r.ObjavaId == objavaId && r.KorisnikId == korisnikId && r.Tip == tip);

            if (existing != null)
            {
                _db.ForumReakcije.Remove(existing);
                await _db.SaveChangesAsync();
                return false; // uklonjena
            }

            _db.ForumReakcije.Add(new ForumReakcija
            {
                ObjavaId = objavaId,
                KorisnikId = korisnikId,
                Tip = tip,
                DatumKreiranja = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
            return true; // dodana
        }

        // PB-60: provjera postojeće reakcije
        public async Task<bool> HasReakcijaAsync(int objavaId, int korisnikId, string tip = "korisno")
        {
            return await _db.ForumReakcije
                .AnyAsync(r => r.ObjavaId == objavaId && r.KorisnikId == korisnikId && r.Tip == tip);
        }
    }
}
