using System.ComponentModel.DataAnnotations;

namespace Movies.EntityModels;

public class VoteDate
{
    [Key]
    public int VoteDateId { get; set; } 
    public DateTime ProposedDate { get; set; } = DateTime.Now;
    public int Votes { get; set; }
    public List<UserVoteDate> UserVotes { get; set; } = new();
}

public class UserVoteDate
{
    [Key]
    public int UserVoteDateId { get; set; } 
    public string UserId { get; set; }
    public int DateId { get; set; }
    public bool HasVoted { get; set; }
}
