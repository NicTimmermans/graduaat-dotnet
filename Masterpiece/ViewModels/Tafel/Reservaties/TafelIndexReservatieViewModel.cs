namespace Restaurant.ViewModels
{
    /// <summary>
    /// Landing voor /Tafel/Reservaties/... + lokale tabs (Toewijzen/Overzicht).
    /// </summary>
    public class TafelIndexReservatieViewModel : TafelBaseViewModel
    {
        // "Toewijzen" of "Overzicht"
        public string ActiveSub { get; set; } = "Toewijzen";

        public TafelToewijzenReservatieViewModel Toewijzen { get; set; } = new();
        public TafelOverzichtReservatieViewModel Overzicht { get; set; } = new();
    }
}
