using Microsoft.AspNetCore.SignalR;
using Movies.Models;
using Movies.EntityModels;
using Microsoft.AspNetCore.Identity;

namespace Movies.SignalR.Service.Hubs;

public class ChatHub : Hub
{
    private readonly MoviesDataContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public ChatHub (MoviesDataContext db, UserManager<IdentityUser> userManager)
    {
	_db = db;
	_userManager = userManager;
    }
    
    public async Task SendMessage(MessageModel message)
    {
	var chatMessage = new ChatMessage
	{
	    SenderName = message.From,
	    MessageText = message.Message
	};

	_db.ChatMessages.Add(chatMessage);
	await _db.SaveChangesAsync();

        IClientProxy proxy;
        proxy = Clients.All;
        await proxy.SendAsync("ReceiveMessage", message);
        return;
    }
}
