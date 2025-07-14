using careerBridge.Areas.Identity.Data;
using Microsoft.Extensions.Hosting;

public class Reply
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime RepliedAt { get; set; } = DateTime.Now;

    public string UserId { get; set; }
    public careerBridgeUser User { get; set; }

    public int PostId { get; set; }
    public Post Post { get; set; }
}
