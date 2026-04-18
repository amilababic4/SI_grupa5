namespace SmartLib.Core.Models
{
    /// <summary>
    /// Evidencija rezervacija nedostupnih knjiga.
    /// Status: aktivna / realizovana / otkazana / istekla
    /// </summary>
    public class Rezervacija
    {
        public int Id { get; set; }
        public int KorisnikId { get; set; }
        public int KnjigaId { get; set; }
        public DateTime DatumRezervacije { get; set; }
        public DateTime? DatumIsteka { get; set; }
        public string Status { get; set; } = "aktivna";   // aktivna / realizovana / otkazana / istekla

        // Navigaciona svojstva
        public Korisnik? Korisnik { get; set; }
        public Knjiga? Knjiga { get; set; }
    }
}
