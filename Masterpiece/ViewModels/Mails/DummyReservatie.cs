namespace Restaurant.ViewModels
{
    public class DummyReservatie
    {
        public string Voornaam { get; set; } = "Jan";
        public string Achternaam { get; set; } = "Janssens";
        public DateTime Datum { get; set; } = DateTime.Today;
        public TimeSpan Tijd { get; set; } = new TimeSpan(19, 0, 0);
        public int TijdSlotId { get; set; } = 1;
        public int AantalPersonen { get; set; } = 4;
        public string TafelNummer { get; set; } = "12";
        
        public string Email { get; set; } = "klant.chezbistro@outlook.com";
        public string KlantId { get; set; } = "1e238d5a-1328-403d-8db8-b59ca508be9c";

    }
}
