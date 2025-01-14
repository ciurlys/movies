using Microsoft.AspNetCore.SignalR;
using Movies.Chat.Models;

namespace Movies.SignalR.Service.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(MessageModel message)
    {
        IClientProxy proxy;
        proxy = Clients.All;
        await proxy.SendAsync("ReceiveMessage", message);
        return;
    }
}