using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Movies.EntityModels;

public class Comment
{
    public int CommentId { get; set; }
    [Required]
    public string? UserId { get; set; } //Author of the comment
    [Required]
    public int MovieId{ get; set; } //Comment on what movie
    public string? Title { get; set; }
    public string? Description { get; set; }
    [NotMapped]
    public bool IsSeen { get; set; }
}
