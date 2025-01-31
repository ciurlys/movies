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
    private readonly int ITEMS_PER_PAGE = 5;
    private readonly IImageService _imageService;
    public MovieRepository(MoviesDataContext db,
			   IWebHostEnvironment webHostEnvironment,
			   IImageService imageService)
    {
	_db = db;
	_webHostEnvironment = webHostEnvironment;
	_imageService = imageService;
    }

    public async Task<IEnumerable<Movie>> GetByTitleAsync(string? title,
							  string? onlySeen,
							  int? page)
    {
	title = title.ToLower();
	int skipAmount = (page ?? 0) * ITEMS_PER_PAGE;
	
	return await _db.Movies
            .Where(m => m.Title.ToLower().StartsWith(title) &&
		   (onlySeen == "t" ? m.Seen : true)) 
            .OrderBy(m => m.Title)
            .ThenBy(m => m.ReleaseDate)
            .Skip(skipAmount)
            .Take(ITEMS_PER_PAGE)
            .ToListAsync();
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(string? onlySeen, int? page)
    {
	int skipAmount = (page ?? 0) * ITEMS_PER_PAGE;
	
	return await _db.Movies
	   .Where(m => (onlySeen == "t" ? m.Seen : true))
           .OrderBy(m => m.Title)
           .ThenByDescending(m => m.ReleaseDate)
           .Skip(skipAmount)
           .Take(ITEMS_PER_PAGE)
	   .ToListAsync();
    }

    public async Task<Movie?> GetByIdAsync(int? id)
    {
	return await _db.Movies.FindAsync(id);
    }
    
    //Returns the int of rows affected
    public async Task<int> UpdateAsync(Movie movie, IFormFile? ImageFile)
    {
	Movie? movieInDb = await _db.Movies.FindAsync(movie.MovieId);
	if (movieInDb is null) return 0;
	
	movieInDb.ImagePath = await _imageService.SaveImageAsync(
	    ImageFile,
	    movieInDb.ImagePath
	);
	movieInDb.Title = movie.Title;
	movieInDb.Director = movie.Director;
	movieInDb.ReleaseDate = movie.ReleaseDate;
	movieInDb.Description = movie.Description;               
 
	return await _db.SaveChangesAsync();	
    }

    public async Task<int> CountAsync()
    {
	return await _db.Movies.CountAsync();
    }
    
    //Returns the int of rows affected
    public async Task<int> AddAsync(Movie movie, IFormFile? ImageFile)
    {
	movie.ImagePath = await _imageService.SaveImageAsync(ImageFile, movie.ImagePath);
	_db.Movies.Add(movie);
	return await _db.SaveChangesAsync();
    }
    //Returns the int of rows affected
    public async Task<int> RemoveAsync(Movie? movie)
    {
	_db.Movies.Remove(movie);
	return await _db.SaveChangesAsync();
    }    
}
