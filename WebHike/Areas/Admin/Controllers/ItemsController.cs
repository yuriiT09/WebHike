using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebHike.Areas.Admin.Models.Item;
using WebHike.Data;
using WebHike.Data.Entities;

using WebHike.Security;

namespace WebHike.Areas.Admin.Controllers;

[Area("Admin")]
[AdminOnly]
public class ItemsController(HikeDbContext hikeDbContext) : Controller
{
    public IActionResult Index()
    {
        var model = hikeDbContext.Items
            .Include(x => x.Category)
            .Include(x => x.Images)
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.Id)
            .Select(x => new AdminItemVM
            {
                Id = x.Id,
                Name = x.Name,
                CategoryName = x.Category.Name,
                Image = x.Images.OrderBy(y => y.Priority).Select(y => y.Image).FirstOrDefault() ?? "default.jpg",
                ImagesCount = x.Images.Count
            })
            .ToList();

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        LoadCategories();

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(AdminItemCreateVM model)
    {
        LoadCategories();

        if (!ModelState.IsValid)
            return View(model);

        bool categoryExists = hikeDbContext.Categories
            .Any(x => x.Id == model.CategoryId && !x.IsDeleted);

        if (!categoryExists)
        {
            ModelState.AddModelError(nameof(model.CategoryId), "Оберіть категорію");
            return View(model);
        }

        var item = new ItemEntity
        {
            Name = model.Name,
            Description = model.Description,
            CategoryId = model.CategoryId
        };

        hikeDbContext.Items.Add(item);
        await hikeDbContext.SaveChangesAsync();

        if (model.Images.Count > 0)
        {
            for (int i = 0; i < model.Images.Count; i++)
            {
                IFormFile image = model.Images[i];

                if (!IsImage(image))
                    continue;

                string imageName = await SaveItemImageAsync(image);

                var itemImage = new ItemImageEntity
                {
                    ItemId = item.Id,
                    Image = imageName,
                    Priority = i + 1
                };

                hikeDbContext.ItemImages.Add(itemImage);
            }

            await hikeDbContext.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var item = hikeDbContext.Items
            .Include(x => x.Images)
            .SingleOrDefault(x => x.Id == id && !x.IsDeleted);

        if (item == null)
            return RedirectToAction(nameof(Index));

        LoadCategories();

        var model = new AdminItemEditVM
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            CategoryId = item.CategoryId,
            CurrentImages = item.Images
                .OrderBy(x => x.Priority)
                .Select(x => x.Image)
                .ToList()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(AdminItemEditVM model)
    {
        LoadCategories();

        var item = hikeDbContext.Items
            .Include(x => x.Images)
            .SingleOrDefault(x => x.Id == model.Id && !x.IsDeleted);

        if (item == null)
            return RedirectToAction(nameof(Index));

        if (!ModelState.IsValid)
        {
            model.CurrentImages = item.Images
                .OrderBy(x => x.Priority)
                .Select(x => x.Image)
                .ToList();

            return View(model);
        }

        bool categoryExists = hikeDbContext.Categories
            .Any(x => x.Id == model.CategoryId && !x.IsDeleted);

        if (!categoryExists)
        {
            ModelState.AddModelError(nameof(model.CategoryId), "Оберіть категорію");

            model.CurrentImages = item.Images
                .OrderBy(x => x.Priority)
                .Select(x => x.Image)
                .ToList();

            return View(model);
        }

        item.Name = model.Name;
        item.Description = model.Description;
        item.CategoryId = model.CategoryId;

        if (model.Images.Count > 0)
        {
            foreach (var image in item.Images)
            {
                DeleteItemImage(image.Image);
            }

            hikeDbContext.ItemImages.RemoveRange(item.Images);

            for (int i = 0; i < model.Images.Count; i++)
            {
                IFormFile image = model.Images[i];

                if (!IsImage(image))
                    continue;

                string imageName = await SaveItemImageAsync(image);

                var itemImage = new ItemImageEntity
                {
                    ItemId = item.Id,
                    Image = imageName,
                    Priority = i + 1
                };

                hikeDbContext.ItemImages.Add(itemImage);
            }
        }

        await hikeDbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var item = hikeDbContext.Items
            .Include(x => x.Images)
            .SingleOrDefault(x => x.Id == id && !x.IsDeleted);

        if (item == null)
            return RedirectToAction(nameof(Index));

        item.IsDeleted = true;

        await hikeDbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private void LoadCategories()
    {
        ViewBag.Categories = hikeDbContext.Categories
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            })
            .ToList();
    }

    private bool IsImage(IFormFile image)
    {
        string extension = Path.GetExtension(image.FileName).ToLower();

        return extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".webp";
    }

    private async Task<string> SaveItemImageAsync(IFormFile image)
    {
        string extension = Path.GetExtension(image.FileName).ToLower();
        string fileName = Guid.NewGuid() + extension;
        string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "items");
        string filePath = Path.Combine(folderPath, fileName);

        Directory.CreateDirectory(folderPath);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await image.CopyToAsync(stream);

        return fileName;
    }

    private void DeleteItemImage(string imageName)
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "items", imageName);

        if (System.IO.File.Exists(filePath))
            System.IO.File.Delete(filePath);
    }
}