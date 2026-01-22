namespace Restaurant.ViewModels
{
    public class AccountCreateViewModel
    {
        [Required]
        public string? Voornaam { get; set; }
        [Required]
        public string? Achternaam { get; set; }
        [Required]
        public string? Adres { get; set; }
        [Required]
        public string? Huisnummer { get; set; }
        [Required]
        public string? Postcode { get; set; }
        [Required]
        public string? Gemeente { get; set; }


        public virtual SelectList? Landen { get; set; }
        public int LandId { get; set; }


        [Required]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }
        public SelectList? Rollen { get; set; }
        public string RolId { get; set; }
    }
}
