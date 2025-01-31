using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Movies.Services;


public interface IImageService
{

    Task<string> SaveImageAsync(IFormFile imageFile, string? existingPath = "");
    void DeleteImage(string imagePath);
}
