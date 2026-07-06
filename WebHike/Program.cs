using Microsoft.EntityFrameworkCore;
using WebHike.Data;
using WebHike.Services;

var builder = WebApplication.CreateBuilder(args);

string strConn = builder.Configuration
    .GetConnectionString("MyWebHikeConnection") ?? "";

builder.Services.AddDbContext<HikeDbContext>(opt =>
    opt.UseNpgsql(strConn));

builder.Services.AddScoped<ImageService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

var dirName = "images";
var dirCurrent = Directory.GetCurrentDirectory();
var path = Path.Combine(dirCurrent, "wwwroot", dirName);
Directory.CreateDirectory(path);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Main}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();