using Movies.EntityModels;

namespace Movies.Mvc.Models;

public record HomeMovieViewModel(int EntitiesAffected, Movie? Movie);