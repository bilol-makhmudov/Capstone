using System.ComponentModel.DataAnnotations;

namespace Capstone.ViewModels.Account;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public required string Email { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
    [Display(Name = "Remember Me")]
    public bool RememberMe { get; set; } = false;
}