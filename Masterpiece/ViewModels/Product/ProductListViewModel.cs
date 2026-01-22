using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.ViewModels
{
    public class ProductListViewModel
    {
        public int Id { get; set; }
        public string? Naam { get; set; }
        public string? Beschrijving { get; set; }
        public string? AllergenenInfo { get; set; }
        public bool Actief { get; set; }
        public bool IsSuggestie { get; set; }
        public string? Categorie { get; set; }
        public decimal Prijs { get; set; }
        public List<int> ProductenInBestelling { get; set; } = new List<int>();
    }
}
