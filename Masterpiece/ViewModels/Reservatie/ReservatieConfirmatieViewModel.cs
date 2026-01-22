namespace Restaurant.ViewModels
{
    public class ReservatieConfirmatieViewModel
    {

        public DateTime Datum { get; set; }
        public string TijdslotNaam { get; set; } = string.Empty;
        public int AantalPersonen { get; set; }
        public string? Opmerking { get; set; }

        public string KlantNaam { get; set; } = string.Empty;
        public string KlantEmail { get; set; } = string.Empty;
    }
}
