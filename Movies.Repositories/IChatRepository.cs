using Movies.EntityModels;
using Movies.Chat.Models;

namespace Movies.Repositories;

public interface IChatRepository
{
    Task<List<MessageModel>> GetLatestAsync();
}
