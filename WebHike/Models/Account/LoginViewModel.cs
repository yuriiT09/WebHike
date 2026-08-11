using System.ComponentModel.DataAnnotations;

namespace WebHike.Models.Account;

public class LoginViewModel
{
    [Display(Name = "Email")]
    [Required(ErrorMessage = "Enter email")]
    [EmailAddress(ErrorMessage = "Enter correct email")]
    public string Email { get; set; } = null!;

    [Display(Name = "Password")]
    [Required(ErrorMessage = "Enter password")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 symbols")]
    public string Password { get; set; } = null!;
}