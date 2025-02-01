using Movies.EntityModels;
using Movies.Models;

namespace Movies.Repositories;

public interface IChatRepository
{
    Task<List<MessageModel>> GetLatestAsync();
}
