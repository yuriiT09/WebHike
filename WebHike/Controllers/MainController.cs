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
        EnsureCategories();

        var list = hikeDbContext.Categories
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.Id)
            .ToList();

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

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var category = hikeDbContext.Categories.FirstOrDefault(x => x.Id == id && !x.IsDeleted);

        if (category == null)
            return RedirectToAction(nameof(Index));

        var model = new CategoryEditViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            CurrentImage = category.Image
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(CategoryEditViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var category = hikeDbContext.Categories.FirstOrDefault(x => x.Id == model.Id && !x.IsDeleted);

        if (category == null)
            return RedirectToAction(nameof(Index));

        category.Name = model.Name;
        category.Slug = model.Slug;

        if (model.Image != null && model.Image.Length > 0)
        {
            if (!imageService.IsCorrectImage(model.Image))
            {
                ModelState.AddModelError(nameof(model.Image), "Оберіть правильне зображення");
                return View(model);
            }

            try
            {
                var oldImage = category.Image;
                category.Image = await imageService.SaveCategoryImageAsync(model.Image);
                imageService.DeleteImage(oldImage);
            }
            catch
            {
                ModelState.AddModelError(nameof(model.Image), "Не вдалося обробити фото");
                return View(model);
            }
        }

        hikeDbContext.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    private void EnsureCategories()
    {
        if (hikeDbContext.Categories.Any())
            return;

        var categories = new List<CategoryEntity>
        {
            new CategoryEntity
            {
                Name = "Гірські походи",
                Slug = "mountain-hikes",
                Image = "https://picsum.photos/seed/mountain-hikes/1000/700"
            },
            new CategoryEntity
            {
                Name = "Лісові стежки",
                Slug = "forest-trails",
                Image = "https://picsum.photos/seed/forest-trails/1000/700"
            },
            new CategoryEntity
            {
                Name = "Озера та річки",
                Slug = "lake-routes",
                Image = "https://picsum.photos/seed/lake-routes/1000/700"
            },
            new CategoryEntity
            {
                Name = "Сімейні маршрути",
                Slug = "family-routes",
                Image = "https://picsum.photos/seed/family-routes/1000/700"
            },
            new CategoryEntity
            {
                Name = "Кемпінг",
                Slug = "camping",
                Image = "https://picsum.photos/seed/camping-trip/1000/700"
            },
            new CategoryEntity
            {
                Name = "Зимові пригоди",
                Slug = "winter-adventures",
                Image = "https://picsum.photos/seed/winter-adventures/1000/700"
            },
            new CategoryEntity
            {
                Name = "Сонячні прогулянки",
                Slug = "sunny-walks",
                Image = "https://picsum.photos/seed/sunny-walks/1000/700"
            },
            new CategoryEntity
            {
                Name = "Туристичне спорядження",
                Slug = "hiking-gear",
                Image = "https://picsum.photos/seed/hiking-gear/1000/700"
            }
        };

        hikeDbContext.Categories.AddRange(categories);
        hikeDbContext.SaveChanges();
    }
}