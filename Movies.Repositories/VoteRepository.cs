using Movies.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace Movies.Repositories;

public class VoteRepository : IVoteRepository
{
    private readonly MoviesDataContext _db;

    public VoteRepository(MoviesDataContext db)
    {
        _db = db;
    }


    public async Task<List<VoteDate>> GetDates(string userId)
    {
        return await _db.Dates
            .OrderByDescending(d => d.Votes)
            .ThenBy(d => d.ProposedDate)
            .Select(d => new VoteDate
            {
                VoteDateId = d.VoteDateId,
                ProposedDate = d.ProposedDate,
                Votes = d.Votes,
                UserVotes = d.UserVotes,
                HasVoted = (_db.UserVotesDate
                    .Any(uvd =>
                          uvd.DateId == d.VoteDateId &&
                          uvd.UserId == userId &&
                          uvd.HasVoted)) ? true : false
            })
            .ToListAsync();
    }

    public async Task<List<Movie>> GetMovies(string userId)
    {
        return await _db.Movies
            .OrderByDescending(m => m.Votes)
            .ThenBy(m => m.Title)
            .Select(m => new Movie
            {
                MovieId = m.MovieId,
                Title = m.Title,
                Director = m.Director,
                Description = m.Description,
                ReleaseDate = m.ReleaseDate,
                Seen = m.Seen,
                ImagePath = m.ImagePath,
                Votes = m.Votes,
                HasVoted = (_db.UserVotesMovie
                    .Any(uvm =>
                          uvm.MovieId == m.MovieId &&
                          uvm.UserId == userId &&
                          uvm.HasVoted)) ? true : false
            })
            .Where(m => !m.Seen)
            .ToListAsync();
    }

    public async Task<VoteDate?> GetTopDate()
    {
        return await _db.Dates
            .OrderByDescending(d => d.Votes)
            .ThenBy(d => d.ProposedDate)
            .FirstOrDefaultAsync();
    }

    public async Task<Movie?> GetTopMovie()
    {

        return await _db.Movies
            .OrderByDescending(m => m.Votes)
            .ThenBy(m => m.Title)
            .Where(m => !m.Seen)
            .FirstOrDefaultAsync();
    }


    //Will set the movie as seen and delete all proposed dates
    public async Task<int> Reset()
    {
        var movie = await GetTopMovie();
        movie!.Seen = true;
        movie!.Votes = 0;
        //Find the current votes and delete them

        var votesToDelete = _db.UserVotesMovie.Where(v => v.MovieId == movie.MovieId);
        _db.UserVotesMovie.RemoveRange(votesToDelete);

        //Delete all dates
        var dates = _db.Dates;
        _db.Dates.RemoveRange(dates);

        return await _db.SaveChangesAsync();
    }
}
