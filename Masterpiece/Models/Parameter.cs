namespace Restaurant.Models
{
    public class Parameter
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Naam is verplicht")]
        public string? Naam { get; set; }

        [Required(ErrorMessage = "Waarde is verplicht")]
        public string? Waarde { get; set; }
    }
}
