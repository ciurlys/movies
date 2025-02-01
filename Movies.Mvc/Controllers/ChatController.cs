using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Repositories;

namespace Movies.Mvc.Controllers;
[Authorize]
public class ChatController : Controller
{
    private readonly ILogger<ChatController> _logger;
    private readonly IChatRepository _chatRepository;
    
    public ChatController(ILogger<ChatController> logger,
			  IChatRepository chatRepository)
    {
	_logger = logger;
	_chatRepository = chatRepository;
    }
    public IActionResult Index()
    {
        return View();
    }
    
    public async Task<IActionResult> GetChatHistory()
    {
	try
	{
	    var messages = await _chatRepository.GetLatestAsync();
	
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
