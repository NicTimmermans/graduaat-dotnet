using System.ComponentModel.DataAnnotations;

namespace Restaurant.ViewModels
{
    public class AccountLoginViewModel
    {
        [Required(ErrorMessage = "E-mail is verplicht.")]
        [EmailAddress(ErrorMessage = "Geef een geldig e-mailadres in.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Wachtwoord is verplicht.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
