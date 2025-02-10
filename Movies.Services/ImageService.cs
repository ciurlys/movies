using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Movies.Services;

public class ImageService : IImageService
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private const string UPLOADS_FOLDER = "covers";

    public ImageService(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<string> SaveImageAsync(IFormFile imageFile,
                         string? existingPath = "")
    {
        if (imageFile == null || imageFile.Length == 0)
            return existingPath!;

        if (!string.IsNullOrEmpty(existingPath))
            DeleteImage(existingPath);

        var uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath,
                           UPLOADS_FOLDER);
        Directory.CreateDirectory(uploadsPath);

        var uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
        var filePath = Path.Combine(uploadsPath, uniqueFileName);


        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(fileStream);
        }
        return uniqueFileName;
    }

    public void DeleteImage(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
        {
            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath,
                        UPLOADS_FOLDER,
                        imagePath);
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

    }
}
