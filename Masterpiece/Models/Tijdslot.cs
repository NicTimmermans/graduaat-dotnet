namespace Restaurant.Models
{
    public class Tijdslot
    {
        [Key]
        public int Id { get; set; }
        public string? Naam { get; set; }
        public bool Actief { get; set; }

        // Navigation properties
        public virtual ICollection<Reservatie> Reservaties { get; set; } = new List<Reservatie>();

    }
}
