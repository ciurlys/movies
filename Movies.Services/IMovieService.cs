using Movies.EntityModels;

namespace Movies.Services;

public interface IMovieService
{
    Task<Movie?> GetMovieAsync(string title);

}
