namespace Restaurant.ViewModels
{
    public class ProductEditViewModel
    {
        public int Id { get; set; }
        [Required]
        public string? Naam { get; set; }
        public string? Beschrijving { get; set; }
        public string? AllergenenInfo { get; set; }
        public string? AfbeeldingUrl { get; set; }
        public IFormFile? Afbeelding { get; set; }
        [Required]
        public bool Actief { get; set; }
        [Required]
        public bool IsSuggestie { get; set; }
        [Required]
        public int CategorieId { get; set; }
        [Required]
        public decimal Prijs { get; set; }

        public SelectList? CategorieList { get; set; }
        public bool ExistsInBestelling { get; set; }
    }
}
