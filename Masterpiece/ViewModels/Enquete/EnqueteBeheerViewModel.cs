namespace Restaurant.ViewModels
{
    public class EnqueteBeheerViewModel
    {
        public int Id { get; set; }

        public string KlantNaam { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime Datum { get; set; }

        [Required(ErrorMessage = "Gelieve een score te kiezen.")]
        [Range(0, 5, ErrorMessage = "De score moet tussen 0 en 5 liggen.")]
        public int EvaluatieAantalSterren { get; set; } = 0;

        [Display(Name = "Opmerkingen")]
        [StringLength(1000, ErrorMessage = "Maximaal 1000 tekens toegestaan.")]
        public string? EvaluatieOpmerkingen { get; set; } = null;

        public int MinStars { get; set; }
    }
}
