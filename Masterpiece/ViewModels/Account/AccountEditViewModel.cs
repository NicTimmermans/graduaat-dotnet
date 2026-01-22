using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Restaurant.ViewModels
{
    public class AccountEditViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string? Voornaam { get; set; }

        [Required]
        public string? Achternaam { get; set; }

        [Required]
        public string? Adres { get; set; }

        [Required]
        public string? Huisnummer { get; set; }

        [Required]
        public string? Postcode { get; set; }

        [Required]
        public string? Gemeente { get; set; }

      
        public virtual SelectList? Landen { get; set; }

        [Required]
        public int LandId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public SelectList? Rollen { get; set; }
        public string? RolId { get; set; }

        public bool IsSelf { get; set; }
    }
}
