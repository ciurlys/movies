using Movies.EntityModels;
using Microsoft.EntityFrameworkCore;
using Movies.Models;

namespace Movies.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly MoviesDataContext _db;
    private const int LATEST_MESSAGE_COUNT = 20;

    
    public ChatRepository(MoviesDataContext db)
    {
	_db = db;
    }

    public async Task<List<MessageModel>> GetLatestAsync()
    {
	return await _db.ChatMessages
	    .OrderByDescending(m => m.Id)
	    .Take(LATEST_MESSAGE_COUNT)
	    .Select(m => new MessageModel
	    {
		From = m.SenderName,
		Message = m.MessageText
	    })
	    .ToListAsync();
    }
}
