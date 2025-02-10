using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.EntityModels;
using Microsoft.AspNetCore.Identity;
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
    public async Task<IActionResult> AddComment(int id)
    {
        Movie? movie;
        List<Comment> comments;
        var userId = _userManager.GetUserId(User);
        if (id <= 0)
        {
            return NotFound();
        }

        if (userId is null)
        {
            _logger.LogWarning("User Id {UserId} not found", userId);
            return NotFound();
        }
        try
        {
            comments = await _commentRepository.GetByMovieIdAsync(id, userId);


            if (comments is null)
            {
                _logger.LogWarning("Failed to find comments for  movie - Movie Id: {Id}", id);
                return NotFound();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching comments for movie - Movie Id: {Id}", id);
            return StatusCode(500, "Internal server error");
        }

        try
        {
            movie = await _movieRepository.GetByIdAsync(id);

            if (movie is null)
            {
                _logger.LogWarning("Failed to find comments for  movie - Movie Id: {Id}", id);
                return NotFound();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching comments for movie - Movie Id: {Id}", id);
            return StatusCode(500, "Internal server error");
        }

        var model = new
        {
            Movie = movie,
            Comments = comments
        };

        return View(model);

    }

    [HttpDelete]
    public async Task<IActionResult> DeleteComment(int id)
    {
        if (id <= 0)
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
    public async Task<IActionResult> EditComment(int id)
    {
        if (id <= 0)
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
            return RedirectToAction("AddComment", new { id = comment.MovieId });
        }
    }

    //This method helps the JavaScript frontend mark the comments as read 
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

        return Json(new { Count = unreadEntries.Count });
    }
}
