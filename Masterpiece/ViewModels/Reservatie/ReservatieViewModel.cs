namespace Restaurant.ViewModels
{
    public class ReservatieViewModel
    {
        [Required(ErrorMessage = "Gelieve het aantal personen in te geven.")]
        [Range(1, 24, ErrorMessage = "Het aantal personen moet tussen 1 en 24 zijn.")]
        public int AantalPersonen { get; set; } = 1;

        [Required]
        public DateTime? GekozenDatum { get; set; }

        public string? Opmerking { get; set; }

        [Required]
        public int? GekozenTijdslotId { get; set; }

        public List<TijdslotBeschikbaarheidHelperViewModel> Tijdsloten { get; set; } = new();

        [DataType(DataType.Date)]
        public List<DateTime> BeschikbareDatums { get; set; } = new();

        // Nieuwe property om succesvolle reservatie te tonen
        public bool ReserveringVoltooid { get; set; } = false;

        public string? Foutmelding { get; set; } // Voor server-side validatie
    }

    public class TijdslotBeschikbaarheidHelperViewModel
    {
        public int TijdslotId { get; set; }
        public string Naam { get; set; } = string.Empty;
        public bool IsBeschikbaar { get; set; }
        public int BeschikbarePlaatsen { get; set; }
    }
}

   

