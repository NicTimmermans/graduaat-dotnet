// /ViewModels/Tafel/Shared/TafelTileViewModel.cs
using System;

namespace Restaurant.ViewModels
{
    public class TafelTileViewModel
    {
        public int Id { get; set; }

        public int TafelNummer { get; set; }

        public int MinAantalPersonen { get; set; }

        public int AantalPersonen { get; set; }

        public bool Actief { get; set; }

        /// <summary>
        /// Volgorde waarin de tiles worden weergegeven (wordt door JS / jouw logic gevuld).
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Korte label voor op de tegel, bv. "T3 (4)".
        /// </summary>
        public string DisplayLabel => $"T{TafelNummer} ({AantalPersonen})";

        /// <summary>
        /// Tekst voor de capaciteit, bv. "2–4 pers."
        /// </summary>
        public string CapacityLabel
            => $"{MinAantalPersonen}–{AantalPersonen} pers.";

        /// <summary>
        /// CSS-class voor de breedte van de tegel op basis van capaciteit.
        /// </summary>
        public string SizeCssClass
        {
            get
            {
                var max = AantalPersonen;

                if (max <= 2) return "size-s";    // klein
                if (max <= 4) return "size-m";    // normaal
                if (max <= 6) return "size-l";    // lang
                return "size-xl";                 // extra lang
            }
        }
    }
}
