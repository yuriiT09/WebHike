using System.ComponentModel.DataAnnotations;

namespace WebHike.Models.Category;

/// <summary>
/// Model
/// </summary>
public class CategoryCreateViewModel
{
    [Display(Name = "Назва категорііі")]
    [Required(ErrorMessage = "Enter name of category")]
    public string Name { get; set; } = null!;
    [Display(Name = "Slug category")]
    [Required(ErrorMessage = "Enter slug of category")]
    public string Slug { get; set; } = null!;
    [Display(Name = "Photo for category")]
    [DataType(DataType.Upload)]
    public IFormFile? Image { get; set; } = null!;
}