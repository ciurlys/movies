using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.EntityModels;
using Movies.Mvc.Data;
using Movies.Mvc.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Movies.Mvc.Controllers;

[Authorize]
public class CommentController : Controller
{
    private readonly MoviesDataContext _db;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<CommentController> _logger;
    public CommentController(MoviesDataContext db,
			     UserManager<IdentityUser> userManager,
			     ILogger<CommentController> logger)
    {
        _db = db;
	_userManager = userManager;
	_logger = logger;
    }

    //GET /comment/addcomment/{id}
    [HttpGet]
    public async Task<IActionResult> AddComment(int? id)
    {
        if (id is null || id <= 0)
        {
            return NotFound();
        }
	try
	{
	    Movie? movieInDb = await _db.Movies
		.Include(m => m.Comments)
		.FirstOrDefaultAsync(m => m.MovieId == id);
	
	    if (movieInDb is null)
	    {
		_logger.LogWarning("Failed to find movie - Movie Id: {Id}", id);
		return NotFound();
	    }
	    
	    HomeMovieViewModel model = new(movieInDb);
	    return View(model);
	}
	catch (Exception ex)
	{
	    _logger.LogError(ex, "Error searching for movie - Movie Id: {Id}", id);
            return StatusCode(500, "Internal server error");
	}
     }


    [HttpDelete]
    public async Task<IActionResult> DeleteComment(int? id)
    {
        if (id is null || id <= 0)
        {
            return NotFound();
        }
	try
	{
	    Comment comment = _db.Comments.Find(id);
	    
	    if (comment is null)
	    {
		_logger.LogWarning("Failed to find comment - Comment Id: {Id}", id);			      return NotFound("Could not find the comment by id");
	    }
	    
	    _db.Comments.Remove(comment);
	    await _db.SaveChangesAsync();
	    
	    return NoContent();
	}
       	catch (Exception ex)
	{
	    _logger.LogError(ex, "Error searching for comment - Comment Id: {Id}", id);
            return StatusCode(500, "Internal server error");
	}
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(int movieId, string userId, string title, string description)
    {
	try
	{
	    Movie? movie = await _db.Movies.FindAsync(movieId);

	    if (movie is null)
	    {
		_logger.LogWarning("Failed to find movie - Movie Id: {Id}", movieId);
		return NotFound("Failed to find movie id");
	    }
	}
	catch (Exception ex)
	{
	    _logger.LogError(ex, "Error searching for movie - Movie Id: {Id}", movieId);
	    return StatusCode(500, "Internal server error");
	}
	
        Comment comment = new()
        {
            MovieId = movieId,
            UserId = userId,
            Title = title,
            Description = description
        };

        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();
	

	//Now we inform all users that a new comment has been made
	var userIds = await GetAllUserIds();

	var filteredUserIds = userIds.Where(id => id != userId).ToList();

	var unreadEntries = filteredUserIds.Select(userId => new UserCommentRead
	    {
		UserId = userId,
		CommentId = comment.CommentId,
		Seen = false
            });

	_db.UserCommentReads.AddRange(unreadEntries);
	await _db.SaveChangesAsync();
	    
        return RedirectToAction("AddComment", new { id = movieId });
    }

    [HttpGet]
    public async Task<IActionResult> EditComment(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        Comment? commentInDb = _db.Comments.Find(id);

        if (commentInDb is null)
        {
            return NotFound();
        }

        return View(commentInDb);
    }
    [HttpPost]
    public async Task<IActionResult> EditComment(Comment comment)
    {
        int affected = 0;
        if (ModelState.IsValid)
        {
            Comment? commentInDb = _db.Comments.Find(comment.CommentId);

            if (commentInDb is not null)
            {
                commentInDb.Title = comment.Title;
                commentInDb.Description = comment.Description;

                affected = await _db.SaveChangesAsync();
            }
        }

        if (affected == 0)
        {
            // Comment/EditComment
            return View(comment);
        }
        else
        {
            return RedirectToAction("AddComment", new {id = comment.MovieId});
        }
    }
    public async Task<List<string>> GetAllUserIds()
    {
	return await _userManager.Users
	    .Select(u => u.Id)
	    .ToListAsync();
    }
    
    //These methods help the JavaScript frontend figure out the comment state 
    [HttpGet]
    public async Task<IActionResult> GetUnreadCommentsCount(int movieId)
    {
	var currentUserId = _userManager.GetUserId(User);
	var unreadCount = await _db.Comments
	    .Where(c => c.MovieId == movieId)
	    .Where(c => c.UserId != currentUserId &&
		   _db.UserCommentReads.Any(ucr =>
					     ucr.CommentId == c.CommentId &&
					     ucr.UserId == currentUserId &&
					    ucr.Seen == false)).CountAsync();
	return Json(unreadCount);
    }

    [HttpGet]
    public async Task<IActionResult> MarkCommentsAsRead(int movieId)
    {
	var currentUserId = _userManager.GetUserId(User);

	 var unreadCommentIds = await _db.Comments
	     .Where(c => c.MovieId == movieId && c.UserId != currentUserId)
	     .Select(c => c.CommentId)
	     .ToListAsync();
	 var unreadEntries = await _db.UserCommentReads
	     .Where(ucr =>
		    unreadCommentIds.Contains(ucr.CommentId) &&
		    ucr.UserId == currentUserId &&
		    ucr.Seen == false)
	     .ToListAsync();

	 foreach (var entry in unreadEntries)
	 {
	     entry.Seen = true;
	 }

	 await _db.SaveChangesAsync();
	
	 return Json(new {Count = unreadEntries.Count});
    }

    [HttpGet]
    public async Task<IActionResult> IsCommentSeen(int commentId)
    {
	var currentUserId = _userManager.GetUserId(User);
	var commentReadState = await _db.UserCommentReads
	    .FirstOrDefaultAsync(ucr => ucr.CommentId == commentId &&
		   ucr.UserId == currentUserId);
	if (commentReadState is null || commentReadState.Seen)
	{
	    return Json(0);
	}
	
	return Json(1);
    }
}
