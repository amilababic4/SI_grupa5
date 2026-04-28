using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;

namespace SmartLib.Tests.TestData
{
    public static class TestSeeder
    {
        public static void Seed(ApplicationDbContext db)
        {
            db.Korisnici.RemoveRange(db.Korisnici);
            db.SaveChanges();

            db.Korisnici.AddRange(
                new Korisnik
                {
                    Id = 2,
                    Ime = "Bibliotekar",
                    Prezime = "Test",
                    Email = "bibliotekar@smartlib.ba",
                    LozinkaHash = "R+QQe1m/6nmEN3ZEN4gXFw==:X5nJN7Js0COVp4hLI8OT/IXLK1GT8EBHa9QqNQNalh0=",
                    UlogaId = 2,
                    Status = "aktivan",
                    DatumKreiranja = DateTime.UtcNow
                },
                new Korisnik
                {
                    Id = 3,
                    Ime = "Clan",
                    Prezime = "Test",
                    Email = "clan@smartlib.ba",
                    LozinkaHash = "R+QQe1m/6nmEN3ZEN4gXFw==:X5nJN7Js0COVp4hLI8OT/IXLK1GT8EBHa9QqNQNalh0=",
                    UlogaId = 1,
                    Status = "aktivan",
                    DatumKreiranja = DateTime.UtcNow
                }
            );

            db.SaveChanges();
        }
    }
}