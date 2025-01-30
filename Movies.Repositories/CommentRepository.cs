using Movies.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace Movies.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly MoviesDataContext _db;

    public CommentRepository(MoviesDataContext db)
    {
	_db = db;
    }

    public async Task<Comment> GetByIdAsync(int? id)
    {
	return await _db.Comments
	    .FirstOrDefaultAsync(c => c.CommentId == id, CancellationToken.None);
    }
    
    public async Task<List<Comment>> GetByMovieIdAsync(int? movieId)
    {
	return await _db.Comments
	    .Where(c => c.MovieId == movieId)
	    .ToListAsync();
    }
    
    //Returns ID of the added comment
    public async Task<int> AddAsync(
	int movieId,
	string userId,
	string title,
	string description)
    {
	Comment comment = new()
        {
            MovieId = movieId,
            UserId = userId,
            Title = title,
            Description = description
        };

	_db.Comments.AddAsync(comment);
	await _db.SaveChangesAsync();
	return comment.CommentId;
    }

    public async Task<int> UpdateAsync(Comment comment)
    {
	Comment? commentInDb = await GetByIdAsync(comment.CommentId);
	if (commentInDb is not null)
	{
	    commentInDb.Title = comment.Title;
	    commentInDb.Description = comment.Description;
	}
	return await _db.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(int? id)
    {
	Comment? commentInDb = await GetByIdAsync(id);
	_db.Comments.Remove(commentInDb);
	return await _db.SaveChangesAsync();
    }

}
