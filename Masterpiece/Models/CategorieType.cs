namespace Restaurant.Models
{
    public class CategorieType
    {
        [Key]
        public int Id { get; set; }
        public string? Naam { get; set; }
        public bool Actief { get; set; }

        // Navigation properties
        public virtual ICollection<Categorie> Categorieen { get; set; } = new List<Categorie>();

    }
}
