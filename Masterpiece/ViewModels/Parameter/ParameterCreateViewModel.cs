namespace Restaurant.ViewModels
{
    public class ParameterCreateViewModel
    {
        [Required(ErrorMessage = "Naam is verplicht")]
        [MinLength(1, ErrorMessage = "Naam mag niet leeg zijn")]
        public string Naam { get; set; }

        [Required(ErrorMessage = "Waarde is verplicht")]
        [MinLength(1, ErrorMessage = "Waarde mag niet leeg zijn")]
        public string Waarde { get; set; }
    }
}
