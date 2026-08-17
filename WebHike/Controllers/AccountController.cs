using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebHike.Data;
using WebHike.Data.Entities;
using WebHike.Models.Account;

namespace WebHike.Controllers;

public class AccountController(HikeDbContext hikeDbContext) : Controller
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

        string passwordHash = HashPassword(model.Password);

        UserEntity? user = hikeDbContext.Users
            .SingleOrDefault(x => x.Email == model.Email && x.PasswordHash == passwordHash);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Wrong email or password");
            return View(model);
        }

        HttpContext.Session.SetInt32("UserId", user.Id);

        return RedirectToAction("Index", "Main");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        bool emailExists = await hikeDbContext.Users
            .AnyAsync(x => x.Email == model.Email);

        if (emailExists)
        {
            ModelState.AddModelError(nameof(model.Email), "Email is already used");
            return View(model);
        }

        string imageName = "default.jpg";

        if (model.Image != null)
        {
            string extension = Path.GetExtension(model.Image.FileName).ToLower();

            if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".webp")
            {
                ModelState.AddModelError(nameof(model.Image), "Choose correct image");
                return View(model);
            }

            imageName = await SaveUserImageAsync(model.Image);
        }

        var user = new UserEntity
        {
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            PasswordHash = HashPassword(model.Password),
            Image = imageName
        };

        hikeDbContext.Users.Add(user);
        await hikeDbContext.SaveChangesAsync();

        HttpContext.Session.SetInt32("UserId", user.Id);

        return RedirectToAction("Index", "Main");
    }

    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Remove("UserId");

        return RedirectToAction("Index", "Main");
    }

    private async Task<string> SaveUserImageAsync(IFormFile image)
    {
        string extension = Path.GetExtension(image.FileName).ToLower();
        string fileName = Guid.NewGuid() + extension;
        string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "users");

        Directory.CreateDirectory(folderPath);

        string filePath = Path.Combine(folderPath, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await image.CopyToAsync(stream);

        return fileName;
    }

    private string HashPassword(string password)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));

        return Convert.ToBase64String(bytes);
    }
}