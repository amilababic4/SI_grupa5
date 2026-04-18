namespace SmartLib.Core.Models
{
    /// <summary>
    /// Članstvo korisnika. Status se derivira iz datum_isteka.
    /// Aktivna ako datum isteka nije prošao.
    /// </summary>
    public class Clanarina
    {
        public int Id { get; set; }
        public int KorisnikId { get; set; }
        public string Status { get; set; } = "aktivna";   // aktivna / istekla (derivirano)
        public DateTime DatumPocetka { get; set; }
        public DateTime DatumIsteka { get; set; }

        // Navigaciono svojstvo
        public Korisnik? Korisnik { get; set; }
    }
}
