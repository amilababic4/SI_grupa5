namespace SmartLib.Core.Models
{
    /// <summary>
    /// Kategorija za klasifikaciju knjiga. Naziv je jedinstven.
    /// </summary>
    public class Kategorija
    {
        public int Id { get; set; }
        public string Naziv { get; set; } = string.Empty;   // UNIQUE
        public string? Opis { get; set; }

        // Navigaciono svojstvo
        public ICollection<Knjiga> Knjige { get; set; } = new List<Knjiga>();
    }
}
