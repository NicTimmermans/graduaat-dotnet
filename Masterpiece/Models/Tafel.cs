namespace Restaurant.Models
{
    public class Tafel
    {
        [Key]
        public int Id { get; set; }
        public string? TafelNummer { get; set; }
        public int AantalPersonen { get; set; }

        
        public int MinAantalPersonen { get; set; }
        public bool Actief { get; set; }
        public string? QrBarcode { get; set; }

        public virtual ICollection<TafelLijst> TafelLijsten { get; set; } = new List<TafelLijst>();

    }
}
