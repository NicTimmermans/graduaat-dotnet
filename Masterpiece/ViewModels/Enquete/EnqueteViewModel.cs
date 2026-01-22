namespace Restaurant.ViewModels
{
    public class EnqueteViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Gelieve een score te kiezen.")]
        [Range(1, 5, ErrorMessage = "De score moet tussen 1 en 5 liggen.")]
        public int Sterren { get; set; }

        [Display(Name = "Opmerkingen")]
        [StringLength(1000, ErrorMessage = "Maximaal 1000 tekens toegestaan.")]
        public string? Opmerkingen { get; set; }
    }
}
