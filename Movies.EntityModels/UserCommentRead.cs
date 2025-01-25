namespace Movies.EntityModels;

public class UserCommentRead
{
    public string UserId { get; set; }
    public int CommentId { get; set; }
    public bool Seen { get; set; }
}
