using Movies.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace Movies.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly MoviesDataContext _db;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly int ITEMS_PER_PAGE = 20;
    public MovieRepository(MoviesDataContext db,
			   IWebHostEnvironment webHostEnvironment)
    {
	_db = db;
	_webHostEnvironment = webHostEnvironment;
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

            if (movieInDb is not null)
            {
                if (ImageFile != null &&
		    ImageFile.Length > 0 &&
		    !string.IsNullOrWhiteSpace(movieInDb.ImagePath))
                {
		    movieInDb.ImagePath = HandleImageSaving(ImageFile,
							    movieInDb.ImagePath);
                }

                movieInDb.Title = movie.Title;
                movieInDb.Director = movie.Director;
                movieInDb.ReleaseDate = movie.ReleaseDate;
                movieInDb.Description = movie.Description;               
	    }
	    return await _db.SaveChangesAsync();	
    }
    //Returns the int of rows affected
    public async Task<int> AddAsync(Movie movie, IFormFile? ImageFile)
    {
	if (ImageFile != null && ImageFile.Length > 0)
	{       
	    movie.ImagePath = HandleImageSaving(ImageFile, movie.ImagePath);
	}
	_db.Movies.Add(movie);
	return await _db.SaveChangesAsync();
    }
    //Returns the int of rows affected
    public async Task<int> RemoveAsync(Movie? movie)
    {
	_db.Movies.Remove(movie);
	return await _db.SaveChangesAsync();
    }    
    //Returns the path to the saved image
    private string HandleImageSaving(IFormFile? ImageFile, string? imagePath = "")
    {
	string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath,
					  "covers",
					  imagePath);
	if (System.IO.File.Exists(oldFilePath))
	{
	    System.IO.File.Delete(oldFilePath);
	}
	
	string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
	string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "covers");
	string filePath = Path.Combine(uploadsFolder, uniqueFileName);

	using (var fileStream = new FileStream(filePath, FileMode.Create))
	{
	    ImageFile.CopyTo(fileStream);
	}
	
	return uniqueFileName;
    }
}
