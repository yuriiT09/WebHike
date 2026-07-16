using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebHike.Models.Item;

public class ItemCreateViewModel
{
    [Display(Name = "Назва")]
    [Required(ErrorMessage = "Enter item name")]
    public string Name { get; set; } = null!;

    [Display(Name = "Опис")]
    [Required(ErrorMessage = "Enter description")]
    public string Description { get; set; } = null!;

    [Display(Name = "Категорія")]
    [Required(ErrorMessage = "Choose category")]
    public int CategoryId { get; set; }

    [Display(Name = "Фото")]
    public List<IFormFile> Images { get; set; } = new();

    public List<SelectListItem> Categories { get; set; } = new();
}