using SmartLib.Infrastructure.Data;
using SmartLib.Infrastructure.Security;

namespace SmartLib.Tests.Integration
{
    /// <summary>
    /// Postavlja lozinke testnih naloga na Test123! u bazi koja već sadrži HasData seed.
    /// </summary>
    internal static class IntegrationTestAccountSeeder
    {
        public static void EnsureTestAccounts(ApplicationDbContext db)
        {
            var hash = PasswordHasher.HashPassword("Test123!");

            var accounts = new (string Email, int UlogaId, string Status)[]
            {
                ("bibliotekar@smartlib.ba", 2, "aktivan"),
                ("clan@smartlib.ba", 1, "aktivan"),
                ("admin@smartlib.ba", 3, "aktivan"),
                ("deaktiviran@smartlib.ba", 1, "deaktiviran")
            };

            foreach (var (email, ulogaId, status) in accounts)
            {
                var korisnik = db.Korisnici.FirstOrDefault(k => k.Email == email);
                if (korisnik != null)
                {
                    korisnik.LozinkaHash = hash;
                    korisnik.UlogaId = ulogaId;
                    korisnik.Status = status;
                }
                else
                {
                    db.Korisnici.Add(new SmartLib.Core.Models.Korisnik
                    {
                        Ime = "Test",
                        Prezime = email.Split('@')[0],
                        Email = email,
                        LozinkaHash = hash,
                        UlogaId = ulogaId,
                        Status = status,
                        DatumKreiranja = DateTime.UtcNow
                    });
                }
            }

            db.SaveChanges();
        }
    }
}
