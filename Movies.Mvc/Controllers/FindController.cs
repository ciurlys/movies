using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Models;
using Movies.EntityModels;
using Movies.Services;
using Movies.Repositories;
namespace Movies.Mvc.Controllers;

[Authorize]
[Route("Find")]
public class FindController : Controller
{
    private readonly ILogger<FindController> _logger;
    private readonly IMovieService _movieService;
    private readonly IMovieRepository _movieRepository;
    public FindController(ILogger<FindController> logger,
              IMovieService movieService,
              IMovieRepository movieRepository)
    {
        _logger = logger;
        _movieService = movieService;
        _movieRepository = movieRepository;
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

            HomeMovieViewModel model = new(movie);

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for movie - Title: {Title}", title);
            ViewData["FailStatus"] = "Did not find a movie with such a title";
            ViewData["activeQuery"] = false;
            return View();
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
            HttpClient client = new();
            var imageBytes = await client.GetByteArrayAsync(movie.ImagePath);
            using var stream = new MemoryStream(imageBytes);
            IFormFile ImageFile =
            new FormFile(baseStream: stream,
                     baseStreamOffset: 0,
                     length: imageBytes.Length,
                     name: "ImagePath",
                     fileName: Path.GetFileName(movie.ImagePath)!)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/png"
            };

            affected = await _movieRepository.AddAsync(movie, ImageFile);
        }

        HomeMovieViewModel model = new(movie);

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
