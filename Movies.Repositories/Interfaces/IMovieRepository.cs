using Movies.EntityModels;
using Microsoft.AspNetCore.Http;

namespace Movies.Repositories;

public interface IMovieRepository
{
    Task<IEnumerable<Movie>> GetByTitleAsync(string? title, string? onlySeen, int? page);
    Task<IEnumerable<Movie>> GetAllAsync(string? onlySeen, int? page);
    Task<Movie?> GetByIdAsync(int? id);
    Task<int> CountAsync();
    Task<int> UpdateAsync(Movie movie, IFormFile? imageFile);
    Task<int> AddAsync(Movie movie, IFormFile? imageFile);
    Task<int> RemoveAsync(Movie? movie);
}
