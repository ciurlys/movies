using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.EntityModels;
using Movies.Mvc.Data;
using Movies.Mvc.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Movies.Repositories;


namespace Movies.Mvc.Controllers;

[Authorize]
public class CommentController : Controller
{
    private readonly MoviesDataContext _db;
    private readonly ICommentRepository _commentRepository;
    private readonly IMovieRepository _movieRepository;
    private readonly IUserRepository _userRepository;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<CommentController> _logger;
    public CommentController(ICommentRepository commentRepository,
			     IUserRepository userRepository,
			     UserManager<IdentityUser> userManager,
			     ILogger<CommentController> logger,
			     IMovieRepository movieRepository,
			     MoviesDataContext db)
    {
	_db = db;
	_commentRepository = commentRepository;
	_movieRepository = movieRepository;
	_userRepository = userRepository;
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
	    List<Comment> comments = await _commentRepository.GetByMovieIdAsync(id);

	    //Move the movie getting elsewhere so the try catch would be more specific
	    Movie movie = await _movieRepository.GetByIdAsync(id);   
	    if (comments is null)
	    {
		_logger.LogWarning("Failed to find comments for  movie - Movie Id: {Id}", id);
		return NotFound();
	    }
	    var model = new {
		Movie = movie,
		Comments = comments
	    };

	    return View(model);
	}
	catch (Exception ex)
	{
	    _logger.LogError(ex, "Error searching comments for movie - Movie Id: {Id}", id); 
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
	    await _commentRepository.DeleteAsync(id);
	    return NoContent();
	}
       	catch (Exception ex)
	{
	    _logger.LogError(ex, "Error deleting comment - Comment Id: {Id}", id);
            return StatusCode(500, "Internal server error");
	}
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(int movieId, string userId, string title, string description)
    {
	int addedCommentId = await _commentRepository.AddAsync(movieId, userId, title, description);

	//Inform all users that a new comment has been made

	var userIds = await _userRepository.GetAllUserIds();
	var filteredUserIds = userIds.Where(id => id != userId);
	var unreadEntries = filteredUserIds.Select(userId => new UserCommentRead
	    {
		UserId = userId,
		CommentId = addedCommentId,
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

        Comment? commentInDb = await _commentRepository.GetByIdAsync(id);

        if (commentInDb is null)
        {
	    _logger.LogError("Error searching comments for movie - Movie Id: {Id}", id); 
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
	    affected = await _commentRepository.UpdateAsync(comment);
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
