using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Mvc.Models;
using Microsoft.EntityFrameworkCore;
using Movies.EntityModels;
using System.Text.Json;
using System.Threading.Tasks;
using Movies.Repositories;

namespace Movies.Mvc.Controllers;
[Authorize]
public class HomeController : Controller
{
    private const int ITEMS_PER_PAGE = 5;
    private readonly ILogger<HomeController> _logger;
    private readonly IMovieRepository _movieRepository;
    public HomeController(ILogger<HomeController> logger,
			  IMovieRepository movieRepository)
    {
        _logger = logger;
	_movieRepository = movieRepository;
    }

    public IActionResult Index()
    {
        return View();
    }
    //GET: /home/movies/{title:string}{onlySeen:string}{page:int}
    public async Task<IActionResult> Movies(string? title, string? onlySeen, int? page)
    {

	if (!string.IsNullOrEmpty(onlySeen))
	{
	    HttpContext.Session.SetString("onlySeen", onlySeen);
	}
	else
	{
	    onlySeen = HttpContext.Session.GetString("onlySeen") ?? "f";
	}
	
        ViewData["onlySeen"] = HttpContext.Session.GetString("onlySeen");

	HomeMoviesViewModel model = new HomeMoviesViewModel {
	    Movies =
	    (!string.IsNullOrWhiteSpace(title)) ?
	    await _movieRepository.GetByTitleAsync(title, onlySeen, page) :
	    await _movieRepository.GetAllAsync(onlySeen, page)
	};

        ViewData["PageCount"] = (await _movieRepository.CountAsync() + ITEMS_PER_PAGE - 1) / ITEMS_PER_PAGE;
        return View(model);
    }

    //GET: /home/editmove/{id}
    [Authorize(Roles = "Administrators")]
    public async Task<IActionResult> EditMovie(int? id)
    {
        HomeMovieViewModel model = new(await _movieRepository.GetByIdAsync(id));
	
        //Views/Home/EditMovie.cshtml
        return View(model);
    }

    //POST: /home/editmovie
    //BODY: JSON Movie
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditMovie(Movie movie, IFormFile? ImageFile)
    {
	int affected = 0;
        if (ModelState.IsValid)
        {
	    affected = await _movieRepository.UpdateAsync(movie, ImageFile);
        }

	if (affected == 0)
        {
	    HomeMovieViewModel model = new(movie);
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
    public async Task<IActionResult> DeleteMovie(int? id)
    {
        int affected = 0;
        Movie? movie = await _movieRepository.GetByIdAsync(id);

        if (movie is not null)
        {
            affected = await _movieRepository.RemoveAsync(movie);
        }

        if (affected == 0)
        {
	    HomeMovieViewModel model = new(movie);
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
	var model = new HomeMovieViewModel(new Movie()); 
        return View(model);
    }

    //POST: /home/addmovie
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddMovie(Movie movie, IFormFile? ImageFile)
    {
	int affected = 0;
        if (ModelState.IsValid)
        {
	    affected = await _movieRepository.AddAsync(movie, ImageFile);
        }

        if (affected == 0)
        {
	    HomeMovieViewModel model = new(movie);
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

