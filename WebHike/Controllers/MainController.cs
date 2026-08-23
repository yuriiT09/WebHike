using Microsoft.AspNetCore.Mvc;
using WebHike.Data;
using WebHike.Data.Entities;

namespace WebHike.Controllers;

public class MainController(HikeDbContext hikeDbContext) : Controller
{
    public IActionResult Index()
    {
        EnsureCategories();

        var model = hikeDbContext.Categories
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Id)
            .ToList();

        return View(model);
    }

    private void EnsureCategories()
    {
        if (hikeDbContext.Categories.Any())
            return;

        var categories = new List<CategoryEntity>
        {
            new CategoryEntity { Name = "Гірські походи", Slug = "mountain-hikes", Image = "https://picsum.photos/seed/mountain-hikes/900/650" },
            new CategoryEntity { Name = "Лісові стежки", Slug = "forest-trails", Image = "https://picsum.photos/seed/forest-trails/900/650" },
            new CategoryEntity { Name = "Озера та річки", Slug = "lake-routes", Image = "https://picsum.photos/seed/lake-routes/900/650" },
            new CategoryEntity { Name = "Сімейні маршрути", Slug = "family-routes", Image = "https://picsum.photos/seed/family-routes/900/650" },
            new CategoryEntity { Name = "Кемпінг", Slug = "camping", Image = "https://picsum.photos/seed/camping/900/650" },
            new CategoryEntity { Name = "Зимові пригоди", Slug = "winter-adventures", Image = "https://picsum.photos/seed/winter-adventures/900/650" },
            new CategoryEntity { Name = "Сонячні прогулянки", Slug = "sunny-walks", Image = "https://picsum.photos/seed/sunny-walks/900/650" },
            new CategoryEntity { Name = "Туристичне спорядження", Slug = "hiking-gear", Image = "https://picsum.photos/seed/hiking-gear/900/650" }
        };

        hikeDbContext.Categories.AddRange(categories);
        hikeDbContext.SaveChanges();
    }
}