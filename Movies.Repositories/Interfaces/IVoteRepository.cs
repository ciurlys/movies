using Movies.EntityModels;

namespace Movies.Repositories;

public interface IVoteRepository
{
    Task<List<VoteDate>> GetDates(string userId);
    Task<List<Movie>> GetMovies(string userId);
    Task<VoteDate?> GetTopDate();
    Task<Movie?> GetTopMovie();
    Task<int> Reset();
}
