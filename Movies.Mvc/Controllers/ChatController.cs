using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Mvc.Models;
using Microsoft.EntityFrameworkCore;
using Movies.EntityModels;
using Microsoft.AspNetCore.Identity;
using Movies.Chat.Models;

namespace Movies.Mvc.Controllers;
[Authorize]
public class ChatController : Controller
{
    private readonly MoviesDataContext _db;
    private const int LATEST_MESSAGE_COUNT = 20;
    public ChatController(MoviesDataContext db)
    {
	_db = db;
    }

    public async Task<IActionResult> GetChatHistory()
    {
	var messages = await _db.ChatMessages
	    .OrderByDescending(m => m.Id)
	    .Take(LATEST_MESSAGE_COUNT)
	    .Select(m => new MessageModel
	    {
		From = m.SenderName,
		Message = m.MessageText
	    })
	    .ToListAsync();
	    
	return Json(messages);
    }
}
