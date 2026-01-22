using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Restaurant.ViewModels
{
    public class TafelCreateViewModel
    {
        [Required]
        [Display(Name = "Tafelnummer")]
        public string? TafelNummer { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Aantal personen moet minstens 1 zijn.")]
        [Display(Name = "Maximum aantal personen")]
        public int AantalPersonen { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Min. aantal personen moet minstens 1 zijn.")]
        [Display(Name = "Minimum aantal personen")]
        public int MinAantalPersonen { get; set; }

        [Display(Name = "Actief")]
        public bool Actief { get; set; } = true;

        [Display(Name = "Barcode / QR-code")]
        public string? QrBarcode { get; set; }

        // lijst met beschikbare nummers (T01–T20 zonder reeds gebruikte)
        public List<string> BeschikbareNummers { get; set; } = new();
    }
}
