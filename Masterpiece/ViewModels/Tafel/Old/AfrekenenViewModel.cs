namespace Restaurant.ViewModels
{
    public class AfrekenenViewModel
    {
        public string TafelNummer { get; set; }
        public int TafelId { get; set; }
        public List<ReservatieItemViewModel> Reservaties { get; set; }
    }
}
