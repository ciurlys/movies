using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Mvc.Models;
using Microsoft.EntityFrameworkCore;
using Movies.EntityModels;
using Microsoft.AspNetCore.Identity;
using Movies.Services;

namespace Movies.Mvc.Controllers;
[Authorize]
[Route("Find")]
public class FindController : Controller
{
    private readonly MoviesDataContext _db;
    private readonly ILogger<FindController> _logger;
    private readonly MovieService _movieService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public FindController(MoviesDataContext db, ILogger<FindController> logger,
	 MovieService movieService, IWebHostEnvironment webHostEnvironment)
    {
	_db = db;
	_logger = logger;
	_movieService = movieService;
	_webHostEnvironment = webHostEnvironment;
    }
    
    //POST: /find/{title}
    [Route("")]
    public async Task<IActionResult> Find(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            ViewData["activeQuery"] = false;
            return View();
        }

        ViewData["activeQuery"] = true;
        try
        {
            var movie = await _movieService.GetMovieAsync(title);

	    if (movie is null)
	    {
		_logger.LogInformation("Movie not found - Title: {Title}", title);
		return NotFound("Movie not found");
	    }
	    
	    _logger.LogInformation("Successfully found movie - Title: {Title}", title);
	    
            HomeMovieViewModel model = new(0, movie);

            return View(model);
        }
        catch (Exception ex)
        {
	    _logger.LogError(ex, "Error searching for movie - Title: {Title}", title);
            return StatusCode(500, "Internal server error");
        }
    }

    //POST: /home/addfound
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddFound(Movie movie)
    {
        int affected = 0;

        if (ModelState.IsValid)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "covers");
            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(movie.ImagePath);

            HttpClient client = new();
            var imageBytes = await client.GetByteArrayAsync(movie.ImagePath);

            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);
	    
            movie.ImagePath = uniqueFileName;

            _db.Movies.Add(movie);
            affected = _db.SaveChanges();
        }
        HomeMovieViewModel model = new(affected, movie);

        if (affected == 0)
        {
            return StatusCode(500, "Error encountered");
        }
        else
        {
            return RedirectToAction("Movies", "Home");
        }
    }
     
}
