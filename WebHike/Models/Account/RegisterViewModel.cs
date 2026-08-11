using System.ComponentModel.DataAnnotations;

namespace WebHike.Models.Account;

public class RegisterViewModel
{
    [Display(Name = "Email")]
    [Required(ErrorMessage = "Enter email")]
    [EmailAddress(ErrorMessage = "Enter correct email")]
    public string Email { get; set; } = null!;

    [Display(Name = "First name")]
    [Required(ErrorMessage = "Enter first name")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "First name must be at least 2 symbols")]
    public string FirstName { get; set; } = null!;

    [Display(Name = "Last name")]
    [Required(ErrorMessage = "Enter last name")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Last name must be at least 2 symbols")]
    public string LastName { get; set; } = null!;

    [Display(Name = "Password")]
    [Required(ErrorMessage = "Enter password")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 symbols")]
    public string Password { get; set; } = null!;

    [Display(Name = "Confirm password")]
    [Required(ErrorMessage = "Confirm password")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords are different")]
    public string PasswordConfirm { get; set; } = null!;

    [Display(Name = "Photo")]
    [DataType(DataType.Upload)]
    public IFormFile? Image { get; set; }
}