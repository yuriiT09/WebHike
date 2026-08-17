using Microsoft.EntityFrameworkCore;
using WebHike.Data;
using WebHike.Services;

var builder = WebApplication.CreateBuilder(args);

string strConn = builder.Configuration
    .GetConnectionString("MyWebHikeConnection") ?? "";

builder.Services.AddDbContext<HikeDbContext>(opt =>
    opt.UseNpgsql(strConn));

builder.Services.AddScoped<ImageService>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

string imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
Directory.CreateDirectory(imagesPath);

string userImagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "users");
Directory.CreateDirectory(userImagesPath);

string itemImagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "items");
Directory.CreateDirectory(itemImagesPath);

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

app.UseEndpoints(endpoints =>
{
    endpoints.MapAreaControllerRoute(
        name: "admin_area",
        areaName: "Admin",
        pattern: "admin/{controller=Categories}/{action=Index}/{id?}"
    );

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Main}/{action=Index}/{id?}"
    );
});

app.Run();