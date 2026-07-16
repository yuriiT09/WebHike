using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebHike.Data;
using WebHike.Data.Entities;
using WebHike.Models.Item;
using WebHike.Services;

namespace WebHike.Controllers;

public class ItemController(HikeDbContext hikeDbContext, ImageService imageService)
    : Controller
{
    [HttpGet]
    public IActionResult Create()
    {
        var model = new ItemCreateViewModel
        {
            Categories = GetCategories()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ItemCreateViewModel model)
    {
        model.Categories = GetCategories();

        if (!ModelState.IsValid)
            return View(model);

        if (!hikeDbContext.Categories.Any(x => x.Id == model.CategoryId && !x.IsDeleted))
        {
            ModelState.AddModelError(nameof(model.CategoryId), "Choose category");
            return View(model);
        }

        var item = new ItemEntity
        {
            Name = model.Name,
            Description = model.Description,
            CategoryId = model.CategoryId
        };

        hikeDbContext.Items.Add(item);
        hikeDbContext.SaveChanges();

        if (model.Images != null && model.Images.Count > 0)
        {
            for (int i = 0; i < model.Images.Count; i++)
            {
                var image = model.Images[i];

                if (!imageService.IsCorrectImage(image))
                    continue;

                var fileName = await imageService.SaveItemImageAsync(image);

                var itemImage = new ItemImageEntity
                {
                    Image = fileName,
                    ItemId = item.Id,
                    Priority = i + 1
                };

                hikeDbContext.ItemImages.Add(itemImage);
            }

            hikeDbContext.SaveChanges();
        }

        return RedirectToAction("Index", "Main");
    }

    private List<SelectListItem> GetCategories()
    {
        return hikeDbContext.Categories
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            })
            .ToList();
    }
}