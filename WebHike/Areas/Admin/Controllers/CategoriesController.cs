using Microsoft.AspNetCore.Mvc;
using WebHike.Areas.Admin.Models.Category;
using WebHike.Data;
using WebHike.Data.Entities;

namespace WebHike.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoriesController(HikeDbContext hikeDbContext) : Controller
{
    public IActionResult Index()
    {
        List<CategoryItemVM> model = hikeDbContext.Categories
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.Id)
            .Select(x => new CategoryItemVM
            {
                Id = x.Id,
                Name = x.Name,
                Slug = x.Slug,
                Image = x.Image,
                ItemsCount = hikeDbContext.Items.Count(y => y.CategoryId == x.Id && !y.IsDeleted)
            })
            .ToList();

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CategoryCreateVM model)
    {
        if (!ModelState.IsValid)
            return View(model);

        bool slugExists = hikeDbContext.Categories
            .Any(x => x.Slug == model.Slug && !x.IsDeleted);

        if (slugExists)
        {
            ModelState.AddModelError(nameof(model.Slug), "Такий slug вже існує");
            return View(model);
        }

        string imageName = "default.jpg";

        if (model.Image != null)
            imageName = await SaveImageAsync(model.Image);

        var category = new CategoryEntity
        {
            Name = model.Name,
            Slug = model.Slug,
            Image = imageName
        };

        hikeDbContext.Categories.Add(category);
        await hikeDbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var category = hikeDbContext.Categories
            .SingleOrDefault(x => x.Id == id && !x.IsDeleted);

        if (category == null)
            return RedirectToAction(nameof(Index));

        var model = new CategoryEditVM
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            CurrentImage = category.Image
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(CategoryEditVM model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var category = hikeDbContext.Categories
            .SingleOrDefault(x => x.Id == model.Id && !x.IsDeleted);

        if (category == null)
            return RedirectToAction(nameof(Index));

        bool slugExists = hikeDbContext.Categories
            .Any(x => x.Id != model.Id && x.Slug == model.Slug && !x.IsDeleted);

        if (slugExists)
        {
            ModelState.AddModelError(nameof(model.Slug), "Такий slug вже існує");
            model.CurrentImage = category.Image;
            return View(model);
        }

        category.Name = model.Name;
        category.Slug = model.Slug;

        if (model.Image != null)
        {
            if (category.Image != "default.jpg")
                DeleteImage(category.Image);

            category.Image = await SaveImageAsync(model.Image);
        }

        await hikeDbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var category = hikeDbContext.Categories
            .SingleOrDefault(x => x.Id == id && !x.IsDeleted);

        if (category == null)
            return RedirectToAction(nameof(Index));

        bool hasItems = hikeDbContext.Items
            .Any(x => x.CategoryId == id && !x.IsDeleted);

        if (hasItems)
        {
            TempData["ErrorMessage"] = "Категорію не можна видалити, бо в ній є товари";
            return RedirectToAction(nameof(Index));
        }

        category.IsDeleted = true;

        await hikeDbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private async Task<string> SaveImageAsync(IFormFile image)
    {
        string extension = Path.GetExtension(image.FileName).ToLower();
        string fileName = Guid.NewGuid() + extension;
        string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
        string filePath = Path.Combine(folderPath, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await image.CopyToAsync(stream);

        return fileName;
    }

    private void DeleteImage(string imageName)
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", imageName);

        if (System.IO.File.Exists(filePath))
            System.IO.File.Delete(filePath);
    }
}