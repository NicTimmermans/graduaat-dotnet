//using System.ComponentModel.DataAnnotations;

//namespace Restaurant.ViewModels
//{
//    public class TafelEditViewModel
//    {
//        public int Id { get; set; }

//        [Required]
//        [Display(Name = "Tafelnummer")]
//        public string? TafelNummer { get; set; }

//        [Required]
//        [Range(1, 50)]
//        [Display(Name = "Minimum aantal personen")]
//        public int MinAantalPersonen { get; set; }

//        [Required]
//        [Range(1, 50)]
//        [Display(Name = "Maximum aantal personen")]
//        public int AantalPersonen { get; set; }

//        [Display(Name = "Actief")]
//        public bool Actief { get; set; }

//        [Display(Name = "Barcode / QR-code")]
//        public string? QrBarcode { get; set; }
//    }
//}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Restaurant.ViewModels
{
    public class TafelEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tafelnummer")]
        public string? TafelNummer { get; set; }

        [Required]
        [Range(1, 50)]
        [Display(Name = "Minimum aantal personen")]
        public int MinAantalPersonen { get; set; }

        [Required]
        [Range(1, 50)]
        [Display(Name = "Maximum aantal personen")]
        public int AantalPersonen { get; set; }

        [Display(Name = "Actief")]
        public bool Actief { get; set; }

        [Display(Name = "Barcode / QR-code")]
        public string? QrBarcode { get; set; }

        // ✅ zelfde als Create: lijst met beschikbare nummers (T01–T20)
        public List<string> BeschikbareNummers { get; set; } = new();
    }
}
