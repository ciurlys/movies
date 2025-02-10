using Microsoft.AspNetCore.SignalR;
using Movies.Models;
using Movies.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Movies.SignalR.Service.Hubs;

public class VoteMovieHub : Hub
{
    private readonly MoviesDataContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public VoteMovieHub(MoviesDataContext db, UserManager<IdentityUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task SendMovieVote(VoteMovieModel voteMovie)
    {

        var currentUserId = Context.UserIdentifier;
        if (currentUserId == null) return;
        using var transaction = await _db.Database.BeginTransactionAsync();

        //Get the current votes of the movie
        var currentMovie = await _db.Movies
            .FirstOrDefaultAsync(m => m.MovieId == voteMovie.MovieId);
        try
        {
            var existingVote = await _db.UserVotesMovie
            .FirstOrDefaultAsync(uvd =>
                         uvd.MovieId == voteMovie.MovieId &&
                         uvd.UserId == currentUserId);
            if (existingVote is null)
            {
                existingVote = new UserVoteMovie
                {
                    UserId = currentUserId,
                    MovieId = voteMovie.MovieId,
                    HasVoted = true
                };
                _db.UserVotesMovie.Add(existingVote);
                currentMovie!.Votes++;
            }
            else if (!existingVote.HasVoted)
            {
                existingVote.HasVoted = true;
                currentMovie!.Votes++;
            }
            else if (existingVote.HasVoted) //Remove if already voted
            {
                currentMovie!.Votes = Math.Max(0, currentMovie!.Votes - 1);
                _db.UserVotesMovie.Remove(existingVote);
            }
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine("Error trying to vote: ", ex);
        }

        IClientProxy proxy;
        proxy = Clients.All;


        await proxy.SendAsync("ReceiveMovieVoteUpdate",
                      new
                      {
                          Votes = currentMovie!.Votes,
                          MovieId = voteMovie.MovieId
                      });

        return;
    }
}

