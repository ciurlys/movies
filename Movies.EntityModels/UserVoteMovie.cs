using System.ComponentModel.DataAnnotations;

namespace Movies.EntityModels;

public class UserVoteMovie
{
    [Key]
    public int UserVoteMovieId { get; set; }
    public string? UserId { get; set; }
    public int MovieId { get; set; }
    public bool HasVoted { get; set; }
}
