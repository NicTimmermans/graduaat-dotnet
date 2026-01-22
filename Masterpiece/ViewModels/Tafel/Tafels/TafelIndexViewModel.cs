namespace Restaurant.ViewModels
{
    /// <summary>
    /// Landing voor /Tafel/Tafels/... + lokale tabs (Grondplan/Overzicht).
    /// </summary>
    public class TafelIndexViewModel : TafelBaseViewModel
    {
        // "Grondplan" of "Overzicht"
        public string ActiveSub { get; set; } = "Grondplan";

        public TafelGrondplanViewModel Grondplan { get; set; } = new();
        public TafelOverzichtViewModel Overzicht { get; set; } = new();
    }
}
