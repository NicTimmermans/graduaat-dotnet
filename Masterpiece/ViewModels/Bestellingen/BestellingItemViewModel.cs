namespace Restaurant.ViewModels
{
    public class BestellingItemViewModel
    {
        public int BestellingId { get; set; }

        // Van Product
        public string ProductNaam { get; set; }
        public string AllergenenInfo { get; set; }
        public string CategorieType { get; set; } // bv. "Dranken" of "Eten"

        // Van Status
        public string StatusNaam { get; set; } // database status

        // Lokale status, alleen voor view (accordion-buttons)
        public string LokaleStatus { get; set; }
        public int StatusId { get; set; }

        // Optioneel: tafelnummers / reservatie info
        public string TafelNummer { get; set; }
        public DateTime? TijdstipBestelling { get; set; }
        public int Aantal { get; set; }
        public string Opmerking { get; set; }

        public bool ProductActief { get; set; } // nieuw: afkomstig van Product

        public bool Actief { get; set; }  // geeft aan of het product actief is
        public int ProductId { get; set; } // nodig voor ToggleProductBeschikbaarheid
    }
}
