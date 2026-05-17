// Lokacija: SmartLib.Core/DTOs/ClanarniaDtos.cs

using System.ComponentModel.DataAnnotations;

namespace SmartLib.Core.DTOs
{
    /// <summary>
    /// DTO za prikaz članarine na profilu člana.
    /// </summary>
    public class ClanarinaDto
    {
        public int Id { get; set; }
        public DateTime DatumPocetka { get; set; }
        public DateTime DatumIsteka { get; set; }

        /// <summary>
        /// Derivirani status — aktivna ako datum isteka nije prošao.
        /// </summary>
        public bool JeAktivna => DateTime.UtcNow.Date <= DatumIsteka.Date;

        public string StatusTekst => JeAktivna ? "aktivna" : "istekla";
    }

    /// <summary>
    /// DTO za kreiranje ili ažuriranje (upsert) članarine.
    /// Koristi se i za Create i za Update formu.
    /// </summary>
    public class ClanarinaUpsertDto
    {
        public int KorisnikId { get; set; }

        [Required(ErrorMessage = "Datum početka je obavezan.")]
        [DataType(DataType.Date)]
        [Display(Name = "Datum početka")]
        public DateTime DatumPocetka { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Datum isteka je obavezan.")]
        [DataType(DataType.Date)]
        [Display(Name = "Datum isteka")]
        public DateTime DatumIsteka { get; set; } = DateTime.Today.AddYears(1);
    }
}