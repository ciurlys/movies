using Movies.EntityModels;

namespace Movies.Mvc.Models;

public class HomeMoviesViewModel
{
    public IEnumerable<Movie> Movies { get; set; } = new List<Movie>();
}
