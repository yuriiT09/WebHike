using System.ComponentModel.DataAnnotations;

namespace WebHike.Models.Category;

public class CategoryEditViewModel
{
    public int Id { get; set; }

    [Display(Name = "Назва категорііі")]
    [Required(ErrorMessage = "Enter name of category")]
    public string Name { get; set; } = null!;

    [Display(Name = "Slug category")]
    [Required(ErrorMessage = "Enter slug of category")]
    public string Slug { get; set; } = null!;

    public string? CurrentImage { get; set; }

    [Display(Name = "New photo for category")]
    [DataType(DataType.Upload)]
    public IFormFile? Image { get; set; }
}