
namespace Restaurant.ViewModels
{
    public class BestellingListViewModel
    {
        public int ReservatieId { get; set; }
        public List<BestellingViewModel> Bestellingen { get; set; }
        public decimal Totaal { get; set; }

        // Voor recap-info welke klant en tafel in editing van een actieve bestelling in tafels beheren
        public string KlantNaam { get; set; }
        public string TafelNummer { get; set; }
        public DateTime? ReservatieDatum { get; set; }

        public DateTime? VolgendeReservatieDatum { get; set; }
    }
}