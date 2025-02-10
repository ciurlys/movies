using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Movies.EntityModels;

public class VoteDate
{
    [Key]
    public int VoteDateId { get; set; }
    public DateTime ProposedDate { get; set; } = DateTime.Now;
    public int Votes { get; set; }
    public List<UserVoteDate> UserVotes { get; set; } = new();
    [NotMapped]
    public bool HasVoted { get; set; }
}
