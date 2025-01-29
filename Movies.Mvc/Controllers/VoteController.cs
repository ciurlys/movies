using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Mvc.Models;
using Microsoft.EntityFrameworkCore;
using Movies.EntityModels;
using Microsoft.AspNetCore.Identity;
using Movies.Chat.Models;
using Microsoft.AspNetCore.RateLimiting;


namespace Movies.Mvc.Controllers;
[Authorize]
public class VoteController : Controller
{
    private readonly MoviesDataContext _db;
    private readonly UserManager<IdentityUser> _userManager;
    public VoteController(MoviesDataContext db,
			  UserManager<IdentityUser> userManager)
    {
	_userManager = userManager;
	_db = db;
    }

    public async Task<IActionResult> Movie()
    {
	var dates = await _db.Dates
	    .OrderByDescending(d => d.Votes)
	    .ThenBy(d => d.ProposedDate)
	    .ToListAsync();

	var movies = await _db.Movies
	    .OrderByDescending(m => m.Votes)
	    .ThenBy(m => m.Title)
	    .Where(m => !m.Seen)
	    .ToListAsync();
	
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
	var topDate = await _db.Dates
	    .OrderByDescending(d => d.Votes)
	    .ThenBy(d => d.ProposedDate)
	    .FirstOrDefaultAsync();
	
	var topMovie = await _db.Movies
	    .OrderByDescending(m => m.Votes)
	    .ThenBy(m => m.Title)
	    .Where(m => !m.Seen)
	    .FirstOrDefaultAsync();

	var model = new
	{
	    Date = topDate.ProposedDate,
	    MovieTitle = topMovie.Title,
	    MovieImagePath = topMovie.ImagePath
	};

	return View(model);
    }

    //Will return 1 on HasVoted == true, else will return 0
    [HttpGet]
    public async Task<JsonResult> CheckUserVoteDate(int voteDateId)
    {
	using var scopedDb = new MoviesDataContext();
	var currentUserId = _userManager.GetUserId(User);
	var userHasVoted = await scopedDb.UserVotesDate
	    .AnyAsync(uvd =>
		      uvd.DateId == voteDateId &&
		      uvd.UserId == currentUserId &&
		      uvd.HasVoted);
	return Json(userHasVoted ? 1 : 0);
    }

    //Will return 1 on HasVoted == true, else will return 0
    [HttpGet]
    public async Task<JsonResult> CheckUserVoteMovie(int voteMovieId)
    {
	using var scopedDb = new MoviesDataContext();
	var currentUserId = _userManager.GetUserId(User);
	var userHasVoted = await scopedDb.UserVotesMovie
	    .AnyAsync(uvm =>
		      uvm.MovieId == voteMovieId &&
		      uvm.UserId == currentUserId &&
		      uvm.HasVoted);

	return Json(userHasVoted ? 1 : 0);
    }
    
}
