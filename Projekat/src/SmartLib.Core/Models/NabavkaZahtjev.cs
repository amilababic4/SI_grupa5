namespace SmartLib.Core.Models;

public class NabavkaZahtjev
{
    public int Id { get; set; }
    public string NazivKnjige { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public string Izdavac { get; set; } = string.Empty;
    public int BrojPrimjeraka { get; set; }
    public string? Napomena { get; set; }
    public DateTime VrijemePodnosenja { get; set; } = DateTime.UtcNow;
    public bool EmailPoslan { get; set; }
    public int PodnosilacId { get; set; }
    public Korisnik? Podnosilac { get; set; }
}