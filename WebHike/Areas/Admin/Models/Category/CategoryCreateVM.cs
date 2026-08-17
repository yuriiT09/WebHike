using System.ComponentModel.DataAnnotations;

namespace WebHike.Areas.Admin.Models.Category;

public class CategoryCreateVM
{
    [Display(Name = "Назва")]
    [Required(ErrorMessage = "Вкажіть назву")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Назва має містити мінімум 2 символи")]
    public string Name { get; set; } = null!;

    [Display(Name = "Slug")]
    [Required(ErrorMessage = "Вкажіть slug")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Slug має містити мінімум 2 символи")]
    public string Slug { get; set; } = null!;

    [Display(Name = "Фото")]
    public IFormFile? Image { get; set; }
}