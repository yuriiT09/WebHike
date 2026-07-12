using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace WebHike.Services;

public class ImageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

    public ImageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public bool IsCorrectImage(IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return false;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        return _allowedExtensions.Contains(extension);
    }

    public async Task<string> SaveCategoryImageAsync(IFormFile file)
    {
        var dirPath = Path.Combine(_environment.WebRootPath, "images");
        Directory.CreateDirectory(dirPath);

        var fileName = Guid.NewGuid().ToString() + ".jpg";
        var filePath = Path.Combine(dirPath, fileName);

        using var inputStream = file.OpenReadStream();
        using var image = await Image.LoadAsync(inputStream);

        image.Mutate(x => x.AutoOrient());

        if (image.Width > 1200 || image.Height > 1200)
        {
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(1200, 1200),
                Mode = ResizeMode.Max
            }));
        }

        await image.SaveAsJpegAsync(filePath, new JpegEncoder
        {
            Quality = 75
        });

        return fileName;
    }

    public void DeleteImage(string? imageName)
    {
        if (string.IsNullOrWhiteSpace(imageName))
            return;

        if (imageName.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            return;

        if (imageName == "default.jpg")
            return;

        var filePath = Path.Combine(_environment.WebRootPath, "images", imageName);

        if (File.Exists(filePath))
            File.Delete(filePath);
    }
}