using System.ComponentModel.DataAnnotations;

namespace Movies.EntityModels;

public class UserVoteDate
{
    [Key]
    public int UserVoteDateId { get; set; }
    public string? UserId { get; set; }
    public int DateId { get; set; }
    public bool HasVoted { get; set; }
}
