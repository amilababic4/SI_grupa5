namespace SmartLib.Core.Models
{
    public class ListaKolekcijaStavka
    {
        public int Id { get; set; }
        public int ListaKolekcijaId { get; set; }
        public int KnjigaId { get; set; }
        public int Redoslijed { get; set; }
        public DateTime DatumDodavanja { get; set; } = DateTime.UtcNow;

        public ListaKolekcija? ListaKolekcija { get; set; }
        public Knjiga? Knjiga { get; set; }
    }
}
