using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Movies.EntityModels;

public class Movie
{
    public int MovieId { get; set; }
    [Required]
    [StringLength(80)]
    public string Title { get; set; } = null!;
    public string? Director { get; set; }
    public string? Description { get; set; }
    public DateOnly ReleaseDate { get; set; }
    [Required]
    public bool Seen { get; set; }
    public string? ImagePath { get; set; }
    public ICollection<Comment>? Comments { get; set; }
    public int Votes { get; set; } = 0;
    [NotMapped]
    public bool HasVoted { get; set; }
    [NotMapped]
    public int UnreadCommentCount { get; set; }
}

