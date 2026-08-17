using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebHike.Data;
using WebHike.Models.Cart;

namespace WebHike.Controllers;

public class CartController(HikeDbContext hikeDbContext) : Controller
{
    private const string CartKey = "Cart";

    public IActionResult Index()
    {
        Dictionary<int, int> cart = GetCart();
        List<int> ids = cart.Keys.ToList();

        var items = hikeDbContext.Items
            .Include(x => x.Category)
            .Include(x => x.Images)
            .Where(x => ids.Contains(x.Id) && !x.IsDeleted)
            .ToList();

        var model = items.Select(x =>
        {
            string image = x.Images
                .OrderBy(y => y.Priority)
                .Select(y => y.Image)
                .FirstOrDefault() ?? "default.jpg";

            return new CartItemViewModel
            {
                ItemId = x.Id,
                Name = x.Name,
                CategoryName = x.Category.Name,
                Image = image,
                Quantity = cart[x.Id]
            };
        }).ToList();

        return View(model);
    }

    [HttpPost]
    public IActionResult Add(int id)
    {
        bool exists = hikeDbContext.Items.Any(x => x.Id == id && !x.IsDeleted);

        if (!exists)
            return RedirectToAction("Index", "Item");

        Dictionary<int, int> cart = GetCart();

        if (cart.ContainsKey(id))
            cart[id]++;
        else
            cart[id] = 1;

        SaveCart(cart);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Plus(int id)
    {
        Dictionary<int, int> cart = GetCart();

        if (cart.ContainsKey(id))
            cart[id]++;

        SaveCart(cart);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Minus(int id)
    {
        Dictionary<int, int> cart = GetCart();

        if (cart.ContainsKey(id))
        {
            cart[id]--;

            if (cart[id] <= 0)
                cart.Remove(id);
        }

        SaveCart(cart);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        Dictionary<int, int> cart = GetCart();

        if (cart.ContainsKey(id))
            cart.Remove(id);

        SaveCart(cart);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Clear()
    {
        HttpContext.Session.Remove(CartKey);

        return RedirectToAction(nameof(Index));
    }

    private Dictionary<int, int> GetCart()
    {
        string? json = HttpContext.Session.GetString(CartKey);

        if (string.IsNullOrWhiteSpace(json))
            return new Dictionary<int, int>();

        return JsonSerializer.Deserialize<Dictionary<int, int>>(json) ?? new Dictionary<int, int>();
    }

    private void SaveCart(Dictionary<int, int> cart)
    {
        HttpContext.Session.SetString(CartKey, JsonSerializer.Serialize(cart));
    }
}