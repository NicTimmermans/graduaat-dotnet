namespace Restaurant.ViewModels
{
    public class ReservatieKiesViewModel
    {

        public int ReservatieId { get; set; }
        public List<string> TafelNummers { get; set; } = new List<string>();
        public DateTime Datum { get; set; }

        public bool MagBestellen { get; set; } = false;

        public string TijdslotNaam { get; set; } = "";
    }
}
