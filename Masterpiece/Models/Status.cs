namespace Restaurant.Models
{
    public class Status
    {
        [Key]
        public int Id { get; set; }
        public string? Naam { get; set; }
        public bool Actief { get; set; }

        // Navigation properties
        public virtual ICollection<Bestelling> Bestellingen { get; set; } = new List<Bestelling>();

    }
}
