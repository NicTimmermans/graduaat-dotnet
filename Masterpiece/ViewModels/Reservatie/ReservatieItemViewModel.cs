namespace Restaurant.ViewModels
{
    public class ReservatieItemViewModel
    {
        public int Id { get; set; }
        public string Voornaam { get; set; }
        public string Naam { get; set; }
        public DateTime? Datum { get; set; }
        public bool IsAanwezig { get; set; }

        public bool IsBetaald {  get; set; }
        public decimal Totaal {  get; set; }

    }
}
