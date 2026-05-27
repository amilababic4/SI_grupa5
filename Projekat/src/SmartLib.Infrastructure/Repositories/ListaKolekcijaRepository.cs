using Microsoft.EntityFrameworkCore;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;

namespace SmartLib.Infrastructure.Repositories
{
    public class ListaKolekcijaRepository : IListaKolekcijaRepository
    {
        private readonly ApplicationDbContext _db;
        private const string WishlistName = "Lista želja";

        public ListaKolekcijaRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ListaKolekcija> EnsureWishlistCollectionAsync(int korisnikId)
        {
            var wishlist = await _db.ListaKolekcije
                .FirstOrDefaultAsync(l => l.KorisnikId == korisnikId && l.Naziv == WishlistName);

            if (wishlist == null)
            {
                var korisnik = await _db.Korisnici.FirstOrDefaultAsync(k => k.Id == korisnikId);
                wishlist = new ListaKolekcija
                {
                    KorisnikId = korisnikId,
                    Naziv = WishlistName,
                    Opis = "Podrazumijevana lista želja",
                    Javna = korisnik?.ListaZeljaJavna ?? false,
                    DatumKreiranja = DateTime.UtcNow
                };
                _db.ListaKolekcije.Add(wishlist);
                await _db.SaveChangesAsync();
            }

            var oldItems = await _db.ListaZeljaStavke
                .Where(l => l.KorisnikId == korisnikId)
                .OrderBy(l => l.DatumDodavanja)
                .ToListAsync();

            if (oldItems.Count > 0)
            {
                var existingBookIds = await _db.ListaKolekcijaStavke
                    .Where(s => s.ListaKolekcijaId == wishlist.Id)
                    .Select(s => s.KnjigaId)
                    .ToListAsync();

                var nextOrder = await _db.ListaKolekcijaStavke
                    .Where(s => s.ListaKolekcijaId == wishlist.Id)
                    .CountAsync();

                var newItems = new List<ListaKolekcijaStavka>();
                foreach (var item in oldItems)
                {
                    if (existingBookIds.Contains(item.KnjigaId)) continue;
                    nextOrder += 1;
                    newItems.Add(new ListaKolekcijaStavka
                    {
                        ListaKolekcijaId = wishlist.Id,
                        KnjigaId = item.KnjigaId,
                        Redoslijed = nextOrder,
                        DatumDodavanja = item.DatumDodavanja
                    });
                }

                if (newItems.Count > 0)
                {
                    _db.ListaKolekcijaStavke.AddRange(newItems);
                }

                _db.ListaZeljaStavke.RemoveRange(oldItems);
                await _db.SaveChangesAsync();
            }

            return wishlist;
        }

        public async Task<List<ListaKolekcija>> GetByKorisnikAsync(int korisnikId)
        {
            return await _db.ListaKolekcije
                .Include(l => l.Stavke)
                .Where(l => l.KorisnikId == korisnikId)
                .OrderByDescending(l => l.DatumKreiranja)
                .ToListAsync();
        }

        public async Task<ListaKolekcija?> GetByIdAsync(int id, bool includeItems = false)
        {
            var query = _db.ListaKolekcije
                .Include(l => l.Korisnik)
                .AsQueryable();

            if (includeItems)
            {
                query = query
                    .Include(l => l.Stavke)
                        .ThenInclude(s => s.Knjiga)
                            .ThenInclude(k => k!.Primjerci);
            }

            return await query.FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<ListaKolekcija> CreateAsync(ListaKolekcija lista)
        {
            _db.ListaKolekcije.Add(lista);
            await _db.SaveChangesAsync();
            return lista;
        }

        public async Task UpdateAsync(ListaKolekcija lista)
        {
            _db.ListaKolekcije.Update(lista);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id, int korisnikId)
        {
            var lista = await _db.ListaKolekcije
                .FirstOrDefaultAsync(l => l.Id == id && l.KorisnikId == korisnikId);
            if (lista == null) return false;

            _db.ListaKolekcije.Remove(lista);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasItemAsync(int listaId, int knjigaId)
        {
            return await _db.ListaKolekcijaStavke
                .AnyAsync(s => s.ListaKolekcijaId == listaId && s.KnjigaId == knjigaId);
        }

        public async Task AddItemAsync(int listaId, int knjigaId)
        {
            if (await HasItemAsync(listaId, knjigaId)) return;

            var redoslijed = await _db.ListaKolekcijaStavke
                .Where(s => s.ListaKolekcijaId == listaId)
                .CountAsync();

            _db.ListaKolekcijaStavke.Add(new ListaKolekcijaStavka
            {
                ListaKolekcijaId = listaId,
                KnjigaId = knjigaId,
                Redoslijed = redoslijed + 1,
                DatumDodavanja = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();
        }

        public async Task RemoveItemAsync(int listaId, int knjigaId)
        {
            var entity = await _db.ListaKolekcijaStavke
                .FirstOrDefaultAsync(s => s.ListaKolekcijaId == listaId && s.KnjigaId == knjigaId);
            if (entity == null) return;

            _db.ListaKolekcijaStavke.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateOrderAsync(int listaId, IReadOnlyList<int> stavkaIds)
        {
            var items = await _db.ListaKolekcijaStavke
                .Where(s => s.ListaKolekcijaId == listaId)
                .ToListAsync();

            var map = items.ToDictionary(s => s.Id);
            for (int i = 0; i < stavkaIds.Count; i++)
            {
                if (map.TryGetValue(stavkaIds[i], out var item))
                {
                    item.Redoslijed = i + 1;
                }
            }

            await _db.SaveChangesAsync();
        }
    }
}
