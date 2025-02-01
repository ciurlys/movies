using Movies.EntityModels;
using Microsoft.AspNetCore.Http;

namespace Movies.Repositories;

public interface IMovieRepository
{
    Task<IEnumerable<Movie>> GetByTitleAsync(string? title, bool? onlySeen, int? page, int movieCount, string userId);
    Task<IEnumerable<Movie>> GetAllAsync(bool? onlySeen, int? page, int movieCount, string userId);
    Task<Movie?> GetByIdAsync(int? id);
    Task<int> CountAsync(string? title, bool? onlySeen);
    Task<int> UpdateAsync(Movie movie, IFormFile? imageFile);
    Task<int> AddAsync(Movie movie, IFormFile? imageFile);
    Task<int> RemoveAsync(Movie? movie);
}
