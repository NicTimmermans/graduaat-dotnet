using System.Collections.Generic;

namespace Restaurant.ViewModels
{
    /// <summary>
    /// Eén tafel + de gekoppelde reservaties (voor het grondplan).
    /// </summary>
    public class TafelReservatiesViewModel
    {
        public int Id { get; set; }

        public string TafelNummer { get; set; }
        public int AantalPersonen { get; set; }
        public int MinAantalPersonen { get; set; }
        public bool Actief { get; set; }

        public List<ReservatieItemViewModel> Reservaties { get; set; } = new();
    }
}
