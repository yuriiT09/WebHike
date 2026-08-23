namespace WebHike.Areas.Admin.Models.Item;

public class AdminItemVM
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string CategoryName { get; set; } = null!;

    public string Image { get; set; } = null!;

    public int ImagesCount { get; set; }
}