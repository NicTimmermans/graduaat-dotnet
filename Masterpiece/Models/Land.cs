namespace Restaurant.Models
{
    public class Land
    {
        [Key]
        public int Id { get; set; }
        public string? Naam { get; set; }
        public bool Actief { get; set; }

        // Navigation properties
        public virtual ICollection<CustomUser> Gebruikers { get; set; } = new List<CustomUser>();

    }
}
