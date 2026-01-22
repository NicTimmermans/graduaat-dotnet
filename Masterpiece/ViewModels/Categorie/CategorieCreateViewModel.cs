namespace Restaurant.ViewModels
{
    public class CategorieCreateViewModel
    {
        [Required]
        [StringLength(100)]
        public string Naam { get; set; }

        [Required]
        public int TypeId { get; set; }
        public bool Actief { get; set; }
    }
}
