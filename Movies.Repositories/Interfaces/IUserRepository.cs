namespace Movies.Repositories;

public interface IUserRepository
{
    Task<List<string>> GetAllUserIds();
}
