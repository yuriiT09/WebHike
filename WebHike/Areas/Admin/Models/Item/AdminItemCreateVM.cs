using System.ComponentModel.DataAnnotations;

namespace WebHike.Areas.Admin.Models.Item;

public class AdminItemCreateVM
{
    [Display(Name = "Назва")]
    [Required(ErrorMessage = "Вкажіть назву")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Назва має містити мінімум 2 символи")]
    public string Name { get; set; } = null!;

    [Display(Name = "Опис")]
    [Required(ErrorMessage = "Вкажіть опис")]
    public string Description { get; set; } = null!;

    [Display(Name = "Категорія")]
    [Required(ErrorMessage = "Оберіть категорію")]
    public int CategoryId { get; set; }

    [Display(Name = "Фото")]
    public List<IFormFile> Images { get; set; } = new();
}