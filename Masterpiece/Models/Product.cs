using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string? Naam { get; set; }
        public string? Beschrijving { get; set; }
        public string? AllergenenInfo { get; set; }
        public string? AfbeeldingNaam { get; set; }

        [ForeignKey("Categorie")]
        public int CategorieId { get; set; }

        public bool Actief { get; set; }
        public bool IsSuggestie { get; set; }

        // Navigation properties
        public virtual Categorie Categorie { get; set; }
        public virtual ICollection<PrijsProduct> PrijsProducten { get; set; } = new List<PrijsProduct>();
        public virtual ICollection<Bestelling> Bestellingen { get; set; } = new List<Bestelling>();

    }
}
