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
    private readonly ILogger<ChatController> _logger;
    private const int LATEST_MESSAGE_COUNT = 20;
    public ChatController(MoviesDataContext db, ILogger<ChatController> logger)
    {
	_db = db;
	_logger = logger;
    }
    public IActionResult Index()
    {
        return View();
    }
    
    public async Task<IActionResult> GetChatHistory()
    {
	try
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
	
	    if (messages is null)
	    {
		_logger.LogInformation("Chat history not found");
		return NotFound("Chat history not found");
	    }
		
	    return Json(messages);
	}
	catch (Exception ex)
	{
	    _logger.LogError(ex, "Error searching chat history");
	    return StatusCode(500, "Internal server error");
	}
    }
}
