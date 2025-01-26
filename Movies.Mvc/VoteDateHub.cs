using Microsoft.AspNetCore.SignalR;
using Movies.Chat.Models;
using Movies.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Movies.Chat.Models;

namespace Movies.SignalR.Service.Hubs;

public class VoteDateHub : Hub
{
    private readonly MoviesDataContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public VoteDateHub (MoviesDataContext db, UserManager<IdentityUser> userManager)
    {
	_db = db;
	_userManager = userManager;
    }
    
    public async Task SendVote(VoteModel vote)//Vote is true if vote up, otherwise - false
    {
	Console.WriteLine($"DateId: {vote.DateId}");

	var currentUserId = Context.UserIdentifier;
	if (currentUserId == null) return;
	using var transaction = await _db.Database.BeginTransactionAsync();

	//Get the current votes of the date
	var currentDate = await _db.Dates.FirstOrDefaultAsync(d => d.VoteDateId == vote.DateId);	

	try
	{
	    var existingVote = await _db.UserVotesDate
		.FirstOrDefaultAsync(uvd =>
				     uvd.DateId == vote.DateId &&
				     uvd.UserId == currentUserId);
	    if (existingVote is null)
	    {
		existingVote = new UserVoteDate
		{
		    UserId = currentUserId,
		    DateId = vote.DateId,
		    HasVoted = true
		};
		_db.UserVotesDate.Add(existingVote);
		currentDate.Votes++;
	    }
	    else if (!existingVote.HasVoted)
	    {
		existingVote.HasVoted = true;
		currentDate.Votes++;
	    }
	    else if (existingVote.HasVoted) //Remove if already voted
	    {
		currentDate.Votes = Math.Max(0, currentDate.Votes - 1);
		_db.UserVotesDate.Remove(existingVote);
	    }
	    await _db.SaveChangesAsync();
	    await transaction.CommitAsync();
	}
	catch (Exception ex)
	{
	    await transaction.RollbackAsync();
	    Console.WriteLine("Error trying to vote");
	}

        IClientProxy proxy;
        proxy = Clients.All;
	

	await proxy.SendAsync("ReceiveVoteUpdate",
			      new { Votes = currentDate.Votes,
			            DateId = vote.DateId});

	return;
    }
}
 
