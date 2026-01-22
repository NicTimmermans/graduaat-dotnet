namespace Restaurant.ViewModels.Account;

public class ResetPasswordViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Code { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6)]
    public string NewPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Wachtwoorden komen niet overeen")]
    public string ConfirmPassword { get; set; }

    public bool IsValidated { get; set; }
    
}