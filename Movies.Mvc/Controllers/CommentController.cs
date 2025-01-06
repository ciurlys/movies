using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.EntityModels;
using Movies.Mvc.Data;
using Movies.Mvc.Models;


namespace Movies.Mvc.Controllers;

[Authorize]
public class CommentController : Controller
{

    private readonly MoviesDataContext _db;

    public CommentController(MoviesDataContext db){
        _db = db;
    }

    //GET /comment/addcomment/{id}
    [HttpGet]
    public async Task<IActionResult> AddComment(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }
        Movie? movieInDb = await _db.Movies
            .Include(m => m.Comments)
            .FirstOrDefaultAsync(m => m.MovieId == id);

       if (movieInDb is null)
       {
            return NotFound();
       }
        HomeMovieViewModel model = new(0, movieInDb);
        return View(model);
    }
    [HttpDelete]
    public async Task<IActionResult> DeleteComment(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }
        Comment comment = _db.Comments.Find(id);

        if (comment is null)
        {
            return NotFound("Could not find the comment by id");
        }

        _db.Comments.Remove(comment);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(int movieId, string userId, string title, string description)
    {
        Movie? movie = await _db.Movies.FindAsync(movieId);

        if (movie is null)
        {
            return NotFound();
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

}