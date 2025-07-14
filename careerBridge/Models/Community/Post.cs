using careerBridge.Areas.Identity.Data;

public class Post
{
    public int Id { get; set; }

    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public string UserId { get; set; }
    public careerBridgeUser User { get; set; }

    public List<Reply> Replies { get; set; }
}
