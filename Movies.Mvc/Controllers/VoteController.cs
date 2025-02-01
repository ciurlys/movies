using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.EntityModels;
using Microsoft.AspNetCore.Identity;
using Movies.Repositories;

namespace Movies.Mvc.Controllers;
[Authorize]
public class VoteController : Controller
{
    private readonly MoviesDataContext _db;
	private readonly ILogger<VoteController> _logger;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IVoteRepository _voteRepository;
    public VoteController(MoviesDataContext db,
			  UserManager<IdentityUser> userManager,
			  IVoteRepository voteRepository,
			  ILogger<VoteController> logger
			  )
    {
	_userManager = userManager;
	_db = db;
	_voteRepository = voteRepository;
	_logger = logger;
    }

    public async Task<IActionResult> Movie()
    {
	var userId = _userManager.GetUserId(User);
	if (userId is null)
	{
		_logger.LogWarning("User Id {UserId} not found", userId);
		return NotFound();
	}

	var dates = await _voteRepository.GetDates(userId);

	var movies = await _voteRepository.GetMovies(userId);
	
	var model = new
	{
	    Dates = dates,
	    Movies = movies
	};
	
	return View(model);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Plan()
    {

	var topDate = await _voteRepository.GetTopDate();
	
	if (topDate is null)
	{
		_logger.LogWarning("Top date {TopDate} not found", topDate);
		return NotFound();
	}

	var topMovie = await _voteRepository.GetTopMovie();
	
	if (topMovie is null)
	{
		_logger.LogWarning("Top movie {TopMovie} not found", topMovie);
		return NotFound();
	}


	var model = new
	{
	    Date = topDate.ProposedDate,
	    MovieTitle = topMovie.Title,
	    MovieImagePath = topMovie.ImagePath
	};

	return View(model);
    }
    
}
