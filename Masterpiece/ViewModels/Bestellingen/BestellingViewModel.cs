namespace Restaurant.ViewModels
{
    public class BestellingViewModel
    {
        public int Id { get; set; }
        public int ReservatieId { get; set; }
        public int ProductId { get; set; }
        public int StatusId { get; set; }

        public string ProductNaam { get; set; }
        public decimal HuidigePrijs { get; set; }

        public int Aantal { get; set; }
        public string? Opmerking { get; set; }

        public string StatusNaam { get; set; }

        public decimal LijnTotaal => Aantal * HuidigePrijs;
    }
}
