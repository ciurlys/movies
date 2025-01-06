using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Mvc.Models;
using Microsoft.EntityFrameworkCore;
using Movies.EntityModels;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;

namespace Movies.Mvc.Controllers;
[Authorize]
public class HomeController : Controller
{
    private int itemsPerPage = 5;
    private readonly ILogger<HomeController> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IHttpClientFactory _httpClient;
    private readonly MoviesDataContext _db;

    public HomeController(ILogger<HomeController> logger, MoviesDataContext db, IWebHostEnvironment webHostEnvironment, IHttpClientFactory httpClient)
    {
        _httpClient = httpClient;
        _webHostEnvironment = webHostEnvironment;
        _db = db;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }


    public IActionResult MovieDetail(int? id)
    {
        if (!id.HasValue)
        {
            return BadRequest("You must pass a movie ID in the route, for example, /Home/MovieDetail/2");
        }

        Movie? model = _db.Movies.SingleOrDefault(movie => movie.MovieId == id);

        if (model is null)
        {
            return NotFound($"MovieId {id} not found.");
        }

        return View(model);
    }

    //GET: /home/movies/{title:string}{onlySeen:string}
    public IActionResult Movies(string? title, string? onlySeen, int? page)
    {

        if (!string.IsNullOrEmpty(onlySeen))
        {
            HttpContext.Session.SetString("onlySeen", onlySeen);
        }
        else
        {
            onlySeen = HttpContext.Session.GetString("onlySeen") ?? "f";
        }



        int movieCount;
        int skipAmount = (page ?? 0) * itemsPerPage;
        ViewData["onlySeen"] = HttpContext.Session.GetString("onlySeen");


        HomeMoviesViewModel model;

        if (!string.IsNullOrWhiteSpace(title))
        {
            title = title.ToLower();
            if (onlySeen == "t")
            {
                model = new(_db.Movies
                    .Where(m => m.Title.ToLower().StartsWith(title) && m.Seen)
                    .OrderBy(m => m.Title)
                    .ThenBy(m => m.ReleaseDate)
                    .Skip(skipAmount)
                    .Take(itemsPerPage)
                    .ToList());

                movieCount = _db.Movies
                    .Where(m => m.Title.ToLower().StartsWith(title) && m.Seen)
                    .Count();

            }
            else
            {
                model = new(_db.Movies
                    .Where(m => m.Title.ToLower().StartsWith(title))
                    .OrderBy(m => m.Title)
                    .ThenBy(m => m.ReleaseDate)
                    .Skip(skipAmount)
                    .Take(itemsPerPage)
                    .ToList());

                movieCount = _db.Movies
                    .Where(m => m.Title.ToLower().StartsWith(title))
                    .Count();
            }
        }
        else
        {
            if (onlySeen == "t")
            {
                model = new(_db.Movies
                    .Where(m => m.Seen)
                    .OrderBy(m => m.Title)
                    .ThenByDescending(m => m.ReleaseDate)
                    .Skip(skipAmount)
                    .Take(itemsPerPage)
                    .ToList());
                movieCount = _db.Movies
                    .Where(m => m.Seen)
                    .Count(); 
            }
            else
            {
                model = new(_db.Movies
                    .OrderBy(m => m.Title)
                    .ThenByDescending(m => m.ReleaseDate)
                    .Skip(skipAmount)
                    .Take(itemsPerPage)
                    .ToList());
                movieCount = _db.Movies
                    .Count();
            }
        }
        ViewData["PageCount"] = (movieCount + itemsPerPage - 1) / itemsPerPage;
        return View(model);
    }

    //GET: /home/editmove/{id}
    [Authorize(Roles = "Administrators")]
    public IActionResult EditMovie(int? id)
    {
        Movie? movieInDb = _db.Movies.Find(id);

        HomeMovieViewModel model = new(movieInDb is null ? 0 : 1, movieInDb);

        //Views/Home/EditMovie.cshtml
        return View(model);
    }

    //POST: /home/editmovie
    //BODY: JSON Movie
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditMovie(Movie movie, IFormFile? ImageFile)
    {
        int affected = 0;

        if (ModelState.IsValid)
        {
            Movie? movieInDb = _db.Movies.Find(movie.MovieId);

            if (movieInDb is not null)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    if (!string.IsNullOrWhiteSpace(movieInDb.ImagePath))
                    {
                        string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "covers", movieInDb.ImagePath);
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
                        movieInDb.ImagePath = uniqueFileName;
                    }
                }
                movieInDb.Title = movie.Title;
                movieInDb.Director = movie.Director;
                movieInDb.ReleaseDate = movie.ReleaseDate;
                movieInDb.Description = movie.Description;
                affected = _db.SaveChanges();
            }
        }

        HomeMovieViewModel model = new(affected, movie);
        if (affected == 0)
        {
            //Views/Home/EditMovie.cshtml
            return View(model);
        }
        else
        {
            return RedirectToAction("Movies");
        }
    }
    [HttpDelete]
    [Authorize(Roles = "Administrators")]
    public IActionResult DeleteMovie(int? id)
    {
        int affected = 0;
        Movie? movieInDb = _db.Movies.Find(id);

        if (movieInDb is not null)
        {
            _db.Movies.Remove(movieInDb);
            affected = _db.SaveChanges();
        }

        HomeMovieViewModel model = new(affected, movieInDb);

        if (affected == 0)
        {
            // Views/Home/DeleteMovie.cshtml
            return View(model);
        }
        else
        {
            return RedirectToAction("Movies");
        }
    }

    //GET: /home/addmovie
    [Authorize(Roles = "Administrators")]
    public IActionResult AddMovie()
    {
        HomeMovieViewModel model = new(0, new Movie());

        return View(model);
    }

    //POST: /home/addmovie
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddMovie(Movie movie, IFormFile? ImageFile)
    {

        int affected = 0;

        if (ModelState.IsValid)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "covers");
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(fileStream);
                }
                movie.ImagePath = uniqueFileName;
            }

            _db.Movies.Add(movie);
            affected = _db.SaveChanges();
        }
        HomeMovieViewModel model = new(affected, movie);

        if (affected == 0)
        {
            // Views/Home/AddMovie.cshtml
            return View(model);
        }
        else
        {
            return RedirectToAction("Movies");
        }
    }

    //POST: /home/find/{title}
    public async Task<IActionResult> Find(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            ViewData["activeQuery"] = false;
            return View();
        }

        ViewData["activeQuery"] = true;

        string apiKey = Environment.GetEnvironmentVariable("API_KEY");
        string apiUrl = $"https://www.omdbapi.com/?t={Uri.EscapeDataString(title)}&apikey={apiKey}";

        try
        {
            HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Error fetching movie data");
            }
            string jsonResponse = await response.Content.ReadAsStringAsync();

            var movieData = JsonSerializer.Deserialize<OmdbApiResponse>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (movieData is null || movieData.Response == "false")
            {
                return NotFound("Movie not found");
            }

            var movie = new Movie
            {
                Title = movieData.Title,
                Director = movieData.Director,
                Description = movieData.Plot,
                ReleaseDate = new DateOnly(int.Parse(movieData.Year), 1, 1),
                ImagePath = movieData.Poster
            };

            HomeMovieViewModel model = new(0, movie);

            return View(model);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
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
            return RedirectToAction("Movies");
        }
    }




    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}

public class OmdbApiResponse
{
    public string Title { get; set; }
    public string Director { get; set; }
    public string Plot { get; set; }
    public string Year { get; set; }
    public string Poster { get; set; }
    public string Response { get; set; }
}
