using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebHike.Data;

namespace WebHike.Controllers;

public class ItemController(HikeDbContext hikeDbContext) : Controller
{
    public IActionResult Index()
    {
        var items = hikeDbContext.Items
            .Include(x => x.Category)
            .Include(x => x.Images)
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.Id)
            .ToList();

        return View(items);
    }

    public IActionResult Details(int id)
    {
        var item = hikeDbContext.Items
            .Include(x => x.Category)
            .Include(x => x.Images)
            .FirstOrDefault(x => x.Id == id && !x.IsDeleted);

        if (item == null)
            return RedirectToAction(nameof(Index));

        return View(item);
    }
}