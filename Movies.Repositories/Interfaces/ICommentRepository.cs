using Movies.EntityModels;

namespace Movies.Repositories;

public interface ICommentRepository
{
    Task<Comment?> GetByIdAsync(int id);
    Task<List<Comment>> GetByMovieIdAsync(int id, string userId);
    Task<int> AddAsync(int movieId, string userId, string title, string description);
    Task<int> UpdateAsync(Comment comment);
    Task<int> DeleteAsync(int id);
}
