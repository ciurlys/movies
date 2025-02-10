using Movies.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Movies.Services;

namespace Movies.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly MoviesDataContext _db;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IImageService _imageService;
    public MovieRepository(MoviesDataContext db,
               IWebHostEnvironment webHostEnvironment,
               IImageService imageService)
    {
        _db = db;
        _webHostEnvironment = webHostEnvironment;
        _imageService = imageService;
    }

    public async Task<IEnumerable<Movie>> GetByTitleAsync(string title,
                              bool? onlySeen,
                              int page,
                              int movieCount,
                              string userId)
    {
        title = title.ToLower();
        int skipAmount = page * movieCount;

        return await _db.Movies
            .Where(m => m.Title.ToLower().Contains(title ?? "") &&
               ((onlySeen ?? false) ? m.Seen : true))
            .SelectMovieWithUnreadComments(_db, userId)
            .OrderBy(m => m.Title)
            .ThenBy(m => m.ReleaseDate)
            .Skip(skipAmount)
            .Take(movieCount)
            .ToListAsync();

    }

    public async Task<IEnumerable<Movie>> GetAllAsync(bool? onlySeen,
                              int page,
                              int movieCount,
                              string userId)
    {
        int skipAmount = page * movieCount;

        return await _db.Movies
            .Where(m => ((onlySeen ?? false) ? m.Seen : true))
            .SelectMovieWithUnreadComments(_db, userId)
            .OrderBy(m => m.Title)
            .ThenByDescending(m => m.ReleaseDate)
            .Skip(skipAmount)
            .Take(movieCount)
            .ToListAsync();
    }

    public async Task<Movie?> GetByIdAsync(int id)
    {
        return await _db.Movies.FindAsync(id);
    }

    //Returns the int of rows affected
    public async Task<int> UpdateAsync(Movie movie, IFormFile? ImageFile)
    {
        Movie? movieInDb = await _db.Movies.FindAsync(movie.MovieId);
        if (movieInDb is null) return 0;

        movieInDb.ImagePath = await _imageService.SaveImageAsync(
            ImageFile!,
            movieInDb.ImagePath
        );
        movieInDb.Title = movie.Title;
        movieInDb.Director = movie.Director;
        movieInDb.ReleaseDate = movie.ReleaseDate;
        movieInDb.Description = movie.Description;
        movieInDb.Seen = movie.Seen;
        return await _db.SaveChangesAsync();
    }

    public async Task<int> CountAsync(string? title, bool? onlySeen)
    {
        return await _db.Movies
            .Where(m => m.Title.ToLower().Contains(title ?? "") &&
               ((onlySeen ?? false) ? m.Seen : true))
            .CountAsync();
    }

    //Returns the int of rows affected
    public async Task<int> AddAsync(Movie movie, IFormFile? ImageFile)
    {
        movie.ImagePath = await _imageService.SaveImageAsync(ImageFile!, movie.ImagePath);
        _db.Movies.Add(movie);
        return await _db.SaveChangesAsync();
    }
    //Returns the int of rows affected
    public async Task<int> RemoveAsync(int movieId)
    {
        var movie = await _db.Movies.FindAsync(movieId);
        if (movie is null) return 0;

        _db.Movies.Remove(movie);
        return await _db.SaveChangesAsync();
    }
}
