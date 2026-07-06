using Microsoft.AspNetCore.Mvc;
using WebHike.Data;
using WebHike.Data.Entities;
using WebHike.Models.Category;
using WebHike.Services;

namespace WebHike.Controllers;

public class MainController(HikeDbContext hikeDbContext, ImageService imageService)
    : Controller
{
    public IActionResult Index()
    {
        var list = new List<CategoryEntity>();

        try
        {
            list = hikeDbContext.Categories
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.Id)
                .ToList();
        }
        catch
        {
            list = new List<CategoryEntity>();
        }

        if (list.Count < 8)
        {
            var fakeCategories = GetFakeCategories();
            list.AddRange(fakeCategories.Take(8 - list.Count));
        }

        return View(list);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CategoryCreateViewModel model)
    {
        if (ModelState.IsValid)
        {
            CategoryEntity categoryEntity = new CategoryEntity();
            categoryEntity.Name = model.Name;
            categoryEntity.Slug = model.Slug;
            categoryEntity.Image = "default.jpg";

            if (model.Image != null && model.Image.Length > 0)
            {
                if (!imageService.IsCorrectImage(model.Image))
                {
                    ModelState.AddModelError(nameof(model.Image), "Оберіть правильне зображення");
                    return View(model);
                }

                try
                {
                    categoryEntity.Image = await imageService.SaveCategoryImageAsync(model.Image);
                }
                catch
                {
                    ModelState.AddModelError(nameof(model.Image), "Не вдалося обробити фото");
                    return View(model);
                }
            }

            hikeDbContext.Categories.Add(categoryEntity);
            hikeDbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    private List<CategoryEntity> GetFakeCategories()
    {
        return new List<CategoryEntity>
        {
            new CategoryEntity
            {
                Id = 1,
                Name = "Гірські походи",
                Slug = "mountain-hikes",
                Image = "https://picsum.photos/seed/mountain-hikes/1000/700"
            },
            new CategoryEntity
            {
                Id = 2,
                Name = "Лісові стежки",
                Slug = "forest-trails",
                Image = "https://picsum.photos/seed/forest-trails/1000/700"
            },
            new CategoryEntity
            {
                Id = 3,
                Name = "Озера та річки",
                Slug = "lake-routes",
                Image = "https://picsum.photos/seed/lake-routes/1000/700"
            },
            new CategoryEntity
            {
                Id = 4,
                Name = "Сімейні маршрути",
                Slug = "family-routes",
                Image = "https://picsum.photos/seed/family-routes/1000/700"
            },
            new CategoryEntity
            {
                Id = 5,
                Name = "Кемпінг",
                Slug = "camping",
                Image = "https://picsum.photos/seed/camping-trip/1000/700"
            },
            new CategoryEntity
            {
                Id = 6,
                Name = "Зимові пригоди",
                Slug = "winter-adventures",
                Image = "https://picsum.photos/seed/winter-adventures/1000/700"
            },
            new CategoryEntity
            {
                Id = 7,
                Name = "Сонячні прогулянки",
                Slug = "sunny-walks",
                Image = "https://picsum.photos/seed/sunny-walks/1000/700"
            },
            new CategoryEntity
            {
                Id = 8,
                Name = "Туристичне спорядження",
                Slug = "hiking-gear",
                Image = "https://picsum.photos/seed/hiking-gear/1000/700"
            }
        };
    }
}