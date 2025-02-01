using Microsoft.AspNetCore.SignalR;
using Movies.EntityModels;
using Microsoft.AspNetCore.Identity;


namespace Movies.SignalR.Service.Hubs;

public class ProposeDateHub : Hub
{
    private readonly MoviesDataContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public ProposeDateHub (MoviesDataContext db, UserManager<IdentityUser> userManager)
    {
	_db = db;
	_userManager = userManager;
    }
    
    public async Task ProposeDate(string proposedDate)
    {
	var currentUserId = Context.UserIdentifier;
	if (currentUserId == null) return;
	using var transaction = await _db.Database.BeginTransactionAsync();

	DateTime parsedTime = DateTime.Parse(proposedDate);
	DateTime utcDate = parsedTime.ToUniversalTime();
	
	VoteDate newDate = new VoteDate
	    {
		ProposedDate = utcDate,
		Votes = 0
	    };
	try
	{ 
	    _db.Dates.Add(newDate);
	    await _db.SaveChangesAsync();
	    await transaction.CommitAsync();

	}
	catch (Exception ex)
	{
	    await transaction.RollbackAsync();
	    Console.WriteLine("Error trying to add date: ", ex);
	}

        IClientProxy proxy;
        proxy = Clients.All;
	

	await proxy.SendAsync("ReceiveProposedDate",
			      new { Votes = newDate.Votes,
				    ProposedDate = newDate.ProposedDate,
			            DateId = newDate.VoteDateId});

	return;
    }
}
 
