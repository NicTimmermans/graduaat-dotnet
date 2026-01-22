using System;
using System.Collections.Generic;

namespace Restaurant.ViewModels
{
    public class OldTafelToewijzenReservatieViewModel
    {
        // Reservatie-info
        public int ReservatieId { get; set; }
        public string KlantNaam { get; set; } = string.Empty;
        public string? KlantEmail { get; set; }
        public DateTime? Datum { get; set; }
        public string TijdslotNaam { get; set; } = string.Empty;
        public int AantalPersonen { get; set; }
        public string? Opmerking { get; set; }

        // Huidige tafels (voor overzicht)
        public List<string> TafelNummers { get; set; } = new();
        public bool HeeftTafel => TafelNummers != null && TafelNummers.Count > 0;

        // Status van reservatie
        public bool IsAanwezig { get; set; }   // check-in
        public bool IsActief { get; set; }     // voor eventuele filters

        // Detail voor "tafels toewijzen"
        public List<TafelSelectItemViewModel> BeschikbareTafels { get; set; } = new();

        // Id’s van geselecteerde tafels in de POST
        public int[] GeselecteerdeTafelIds { get; set; } = Array.Empty<int>();

        // Gebruikt als alle tafels bezet zijn
        public string? WachttijdMelding { get; set; }
    }

    public class TafelSelectItemViewModel
    {
        public int Id { get; set; }
        public string TafelNummer { get; set; } = string.Empty;
        public int AantalPersonen { get; set; }
    }
}
