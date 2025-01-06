using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Mvc.Models;
using Microsoft.EntityFrameworkCore;
using Movies.EntityModels;


namespace Movies.Mvc.Controllers;
[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;

    private readonly MoviesDataContext _db;

    public HomeController(ILogger<HomeController> logger, MoviesDataContext db, IWebHostEnvironment webHostEnvironment)
    {
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

    public IActionResult Movies()
    {
        HomeMoviesViewModel model = new(_db.Movies
            .OrderBy(m => m.Title)
            .ThenByDescending(m => m.ReleaseDate));

        return View(model);
    }

    //GET: /home/editmove/{id}

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

    // //GET: /home/deletemovie/{id}
    // public IActionResult DeleteMovie(int? id)
    // {
    //     Movie? movieInDb = _db.Movies.Find(id);

    //     HomeMovieViewModel model = new(movieInDb is null ? 0 : 1, movieInDb);

    //     return View(model);
    // }

    //POST: /home/deletemovie/{id}
    [HttpDelete]
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
    public IActionResult AddMovie()
    {
        HomeMovieViewModel model = new(0, new Movie());

        return View(model);
    }

    //POST: /home/addmovie
    [HttpPost]
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
