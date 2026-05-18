using System.ComponentModel.DataAnnotations.Schema;

namespace SmartLib.Core.Models
{
    /// <summary>
    /// Registrovani korisnik sistema (Član, Bibliotekar, Administrator)
    /// Status: aktivan / deaktiviran (soft delete)
    /// </summary>
    public class Korisnik
    {
        public int Id { get; set; }
        public string Ime { get; set; } = string.Empty;
        public string Prezime { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;   // UNIQUE
        public string LozinkaHash { get; set; } = string.Empty;
        public int UlogaId { get; set; }
        public string Status { get; set; } = "aktivan";     // aktivan / deaktiviran
        public DateTime DatumKreiranja { get; set; } = DateTime.UtcNow;
        public DateTime? DatumDeaktivacije { get; set; }

        // Navigaciona svojstva
        public Uloga? Uloga { get; set; }
        public ICollection<Zaduzenje> Zaduzenja { get; set; } = new List<Zaduzenje>();
        public ICollection<Rezervacija> Rezervacije { get; set; } = new List<Rezervacija>();

        public ICollection<Clanarina> Clanarine { get; set; } = new List<Clanarina>();

        // Password Reset Flow
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }

        // TODO: Book Recommendation System
        // Add navigation properties for user history/ratings when models are implemented
        // public ICollection<Ocjena> Ocjene { get; set; } = new List<Ocjena>();
        // public ICollection<IstorijaCitanja> IstorijaCitanja { get; set; } = new List<IstorijaCitanja>();
    }
}
