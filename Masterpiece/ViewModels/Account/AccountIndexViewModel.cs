namespace Restaurant.ViewModels
{
    public class AccountIndexViewModel
    {
        public string Id { get; set; }
        public string? Email { get; set; }
        public string? Voornaam { get; set; }
        public string? Achternaam { get; set; }
        public string? Adres { get; set; }
        public string? Huisnummer { get; set; }
        public string? Postcode { get; set; }
        public string? Gemeente { get; set; }
        public bool Actief { get; set; }
        public int LandId { get; set; }

        // Navigation properties
        public virtual Land Land { get; set; }
        public IdentityRole Rol { get; set; }
    }
}
