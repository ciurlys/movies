using Movies.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace Movies.Repositories;

public static class MovieExtension
{
    public static IQueryable<Movie> SelectMovieWithUnreadComments(
    this IQueryable<Movie> movies,
    MoviesDataContext db, string userId)
    {
        return movies.Select(m => new Movie
        {
            MovieId = m.MovieId,
            Title = m.Title,
            Director = m.Director,
            Description = m.Description,
            ReleaseDate = m.ReleaseDate,
            Seen = m.Seen,
            ImagePath = m.ImagePath,
            Votes = m.Votes,
            UnreadCommentCount = db.Comments
            .Where(c => c.MovieId == m.MovieId)
            .Where(c => c.UserId != userId &&
               db.UserCommentReads.Any(ucr =>
                            ucr.CommentId == c.CommentId &&
                            ucr.UserId == userId &&
                            ucr.Seen == false)).Count()
        });
    }


}
