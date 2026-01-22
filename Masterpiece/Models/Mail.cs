namespace Restaurant.Models
{
    public class Mail
    {
        [Key]
        public int Id { get; set; }
        public string? Naam { get; set; }
        public string? Onderwerp { get; set; }
        public string? Body { get; set; }
    }
}
