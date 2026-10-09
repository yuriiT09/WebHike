using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebHike.Data;
using WebHike.Data.Entities;
using WebHike.Services;

var builder = WebApplication.CreateBuilder(args);
string connectionString = builder.Configuration.GetConnectionString("MyWebHikeConnection")
    ?? throw new InvalidOperationException("Set ConnectionStrings:MyWebHikeConnection using user secrets or environment variables.");

if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("Set ConnectionStrings:MyWebHikeConnection before starting WebHike.");

builder.Services.AddDbContext<HikeDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddScoped<IPasswordHasher<UserEntity>, PasswordHasher<UserEntity>>();
builder.Services.AddScoped<ImageService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});
builder.Services.AddControllersWithViews(options =>
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

var app = builder.Build();
foreach (string directory in new[] { "images", "images/users", "images/items" })
    Directory.CreateDirectory(Path.Combine(app.Environment.WebRootPath, directory.Replace("/", Path.DirectorySeparatorChar.ToString())));

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapAreaControllerRoute(
    name: "admin_area",
    areaName: "Admin",
    pattern: "admin/{controller=Categories}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Main}/{action=Index}/{id?}");

app.Run();
