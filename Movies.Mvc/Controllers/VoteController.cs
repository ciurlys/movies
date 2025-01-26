using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Mvc.Models;
using Microsoft.EntityFrameworkCore;
using Movies.EntityModels;
using Microsoft.AspNetCore.Identity;
using Movies.Chat.Models;

namespace Movies.Mvc.Controllers;
[Authorize]
public class VoteController : Controller
{
    private readonly MoviesDataContext _db;
    private readonly UserManager<IdentityUser> _userManager;
    public VoteController(MoviesDataContext db, UserManager<IdentityUser> userManager)
    {
	_userManager = userManager;
	_db = db;
    }

    public IActionResult Movie()
    {
	var dates = _db.Dates
	    .OrderByDescending(d => d.Votes)
	    .ThenBy(d => d.ProposedDate)
	    .ToList();

	var movies = _db.Movies
	    .OrderBy(m => m.Votes)
	    .ThenBy(m => m.Title)
	    .ToList();
	
	var model = new
	{
	    Dates = dates,
	    Movies = movies
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
