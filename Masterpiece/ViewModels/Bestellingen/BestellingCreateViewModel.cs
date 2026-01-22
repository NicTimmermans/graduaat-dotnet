namespace Restaurant.ViewModels
{
    public class BestellingCreateViewModel
    {   
        public int ReservatieId { get; set; }
        public int ProductId { get; set; }
        public int Aantal { get; set; }
        public string? Opmerking { get; set; }
        public int ViewMode { get; set; }
    }
}
