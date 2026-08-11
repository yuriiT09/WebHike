using Microsoft.AspNetCore.Mvc;
using WebHike.Models.Account;

namespace WebHike.Controllers;

public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        TempData["AccountMessage"] = "Вхід виконано успішно";

        return RedirectToAction("Index", "Main");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        TempData["AccountMessage"] = "Реєстрація виконана успішно";

        return RedirectToAction(nameof(Login));
    }
}