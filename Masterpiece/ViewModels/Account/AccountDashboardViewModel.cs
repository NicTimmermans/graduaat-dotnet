using System.ComponentModel.DataAnnotations;

namespace Restaurant.ViewModels
{
    public class AccountDashboardViewModel
    {
        public string Id { get; set; }

        [Required]
        public string? Voornaam { get; set; }

        [Required]
        public string? Achternaam { get; set; }

        [Required, EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Adres { get; set; }

        [Required]
        public string? Huisnummer { get; set; }

        [Required]
        public string? Postcode { get; set; }

        [Required]
        public string? Gemeente { get; set; }

        public string? LandNaam { get; set; }
        public string? RolNaam { get; set; }

        public IList<ReservatieDashboardViewModel> Reservaties { get; set; }
            = new List<ReservatieDashboardViewModel>();


    }

    public class ReservatieDashboardViewModel
    {
        public int Id { get; set; }
        public DateTime? Datum { get; set; }
        public string TijdslotNaam { get; set; } = string.Empty;
        public int AantalPersonen { get; set; }
        public bool Betaald { get; set; }

        public bool MagBestellen { get; set; }
        public int ReservatieId { get; set; }

        public List<string> TafelNummers { get; set; } = new List<string>();
    }
}
