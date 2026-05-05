using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;
using SmartLib.Infrastructure.Security; 

namespace SmartLib.Tests.TestData
{
    public static class TestSeeder
    {
        public static void Seed(ApplicationDbContext db)
        {
            db.Korisnici.RemoveRange(db.Korisnici);
            db.Primjerci.RemoveRange(db.Primjerci);
            db.Knjige.RemoveRange(db.Knjige);
            db.Kategorije.RemoveRange(db.Kategorije);
            db.Uloge.RemoveRange(db.Uloge);
            db.SaveChanges();
           
            var ulogaClan = new Uloga { Naziv = "Član" };
            db.Uloge.Add(ulogaClan);
            db.SaveChanges();

            var ulogaBib = new Uloga { Naziv = "Bibliotekar" };
            db.Uloge.Add(ulogaBib);
            db.SaveChanges();
            
            var hash = PasswordHasher.HashPassword("Test123!");

            db.Korisnici.AddRange(
                new Korisnik
                {
                    Ime = "Bibliotekar",
                    Prezime = "Test",
                    Email = "bibliotekar@smartlib.ba",
                    LozinkaHash = hash,
                    UlogaId = ulogaBib.Id,
                    Status = "aktivan",
                    DatumKreiranja = DateTime.UtcNow
                },
                new Korisnik
                {
                    Ime = "Clan",
                    Prezime = "Test",
                    Email = "clan@smartlib.ba",
                    LozinkaHash = hash,
                    UlogaId = ulogaClan.Id,
                    Status = "aktivan",
                    DatumKreiranja = DateTime.UtcNow
                },
                new Korisnik
                {
                    Ime = "Deakt",
                    Prezime = "Test",
                    Email = "deaktiviran@smartlib.ba",
                    LozinkaHash = hash,
                    UlogaId = ulogaClan.Id,
                    Status = "deaktiviran",
                    DatumKreiranja = DateTime.UtcNow
                }
            );
            db.SaveChanges();

            var kategorija = new Kategorija { Naziv = "Test Kat" };
            db.Kategorije.Add(kategorija);
            db.SaveChanges();

            var knjiga = new Knjiga
            {
                Naslov = "Test Knjiga",
                Autor = "Autor",
                Isbn = "1234567890123",
                KategorijaId = kategorija.Id,
                GodinaIzdanja = 2024
            };
            db.Knjige.Add(knjiga);
            db.SaveChanges();

            db.Primjerci.Add(new Primjerak
            {
                KnjigaId = knjiga.Id,
                InventarniBroj = $"INV-{knjiga.Id}-001",
                Status = "dostupan",
                DatumNabave = DateTime.UtcNow
            });
            db.SaveChanges();
        }
    }
}