using Movies.EntityModels;

namespace Movies.Mvc.Models;

public record HomeMoviesViewModel(IEnumerable<Movie>? Movies);