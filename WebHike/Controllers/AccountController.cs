using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebHike.Data;
using WebHike.Data.Entities;
using WebHike.Models.Account;

namespace WebHike.Controllers;

public class AccountController(HikeDbContext db, IPasswordHasher<UserEntity> passwordHasher) : Controller
{
    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        string email = model.Email.Trim().ToLowerInvariant();
        var user = await db.Users.SingleOrDefaultAsync(x => x.Email == email);
        if (user is null || !VerifyPassword(user, model.Password, out bool upgrade))
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password");
            return View(model);
        }

        if (upgrade)
        {
            user.PasswordHash = passwordHasher.HashPassword(user, model.Password);
            await db.SaveChangesAsync();
        }

        HttpContext.Session.Clear();
        HttpContext.Session.SetInt32("UserId", user.Id);
        return RedirectToAction("Index", "Main");
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        string email = model.Email.Trim().ToLowerInvariant();
        if (await db.Users.AnyAsync(x => x.Email == email))
        {
            ModelState.AddModelError(nameof(model.Email), "Email is already registered");
            return View(model);
        }

        string imageName = "default.jpg";
        if (model.Image is { Length: > 0 } image)
        {
            string extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (image.Length > 5 * 1024 * 1024 || extension is not (".jpg" or ".jpeg" or ".png" or ".webp"))
            {
                ModelState.AddModelError(nameof(model.Image), "Upload a JPG, PNG or WEBP under 5 MB");
                return View(model);
            }

            imageName = await SaveUserImageAsync(image);
        }

        var user = new UserEntity
        {
            Email = email,
            FirstName = model.FirstName.Trim(),
            LastName = model.LastName.Trim(),
            Image = imageName
        };
        user.PasswordHash = passwordHasher.HashPassword(user, model.Password);
        db.Users.Add(user);
        await db.SaveChangesAsync();

        HttpContext.Session.Clear();
        HttpContext.Session.SetInt32("UserId", user.Id);
        return RedirectToAction("Index", "Main");
    }

    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Main");
    }

    private bool VerifyPassword(UserEntity user, string password, out bool upgrade)
    {
        upgrade = false;
        if (user.PasswordHash.Length == 44)
        {
            byte[] expected;
            try { expected = Convert.FromBase64String(user.PasswordHash); }
            catch (FormatException) { return false; }

            byte[] actual = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            upgrade = expected.Length == actual.Length && CryptographicOperations.FixedTimeEquals(expected, actual);
            return upgrade;
        }

        var status = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        upgrade = status == PasswordVerificationResult.SuccessRehashNeeded;
        return status != PasswordVerificationResult.Failed;
    }

    private static async Task<string> SaveUserImageAsync(IFormFile image)
    {
        string extension = Path.GetExtension(image.FileName).ToLowerInvariant();
        string fileName = Guid.NewGuid().ToString("N") + extension;
        string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "users");
        Directory.CreateDirectory(folder);
        await using var stream = System.IO.File.Create(Path.Combine(folder, fileName));
        await image.CopyToAsync(stream);
        return fileName;
    }
}
