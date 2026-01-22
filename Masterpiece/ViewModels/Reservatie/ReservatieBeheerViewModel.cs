

namespace Restaurant.ViewModels
{
    public class ReservatieBeheerViewModel
    {
        public int Id { get; set; }

        public string KlantNaam { get; set; } = string.Empty;

        [Required]
        [Range(1, 24)]
        public int AantalPersonen { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Datum { get; set; }

        [Required]
        public int TijdslotId { get; set; }

        public string TijdslotNaam { get; set; } = string.Empty;

        public bool IsAanwezig { get; set; }



    }
}
