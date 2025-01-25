using Microsoft.AspNetCore.SignalR;

namespace Movies.SignalR.Service.Hubs;

public class VoteHub : Hub
{
    public async Task NotifyVoteChange(int voteDateId, int newVoteCount)
    {
	await Clients.All.SendAsync("ReceiveVoteUpdate", voteDateId, newVoteCount);
    }

}
