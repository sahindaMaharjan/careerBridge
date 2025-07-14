using careerBridge.Areas.Identity.Data;  
using careerBridge.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

public class CommunityController : Controller
{
    private readonly careerBridgeDb _context;
    private readonly UserManager<careerBridgeUser> _userManager;

    public CommunityController(careerBridgeDb context, UserManager<careerBridgeUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Show all posts
    public async Task<IActionResult> Index()
    {
        var posts = await _context.Posts
            .Include(p => p.User)
        .Include(p => p.Replies)
            .ThenInclude(r => r.User)
        .OrderByDescending(p => p.CreatedAt)
        .ToListAsync();

        return View(posts); // Views/Community/Index.cshtml
    }

    // View post + replies
    public async Task<IActionResult> Details(int id)
    {
        var post = await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Replies)
                .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null) return NotFound();

        return View(post); // Views/Community/Details.cshtml
    }

    // Show form to create a new post
    [Authorize]
    public IActionResult Create()
    {
        return View(); // Views/Community/Create.cshtml
    }

    // Handle post creation
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string title, string content)
    {
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
        {
            ModelState.AddModelError("", "Title and content are required.");
            return View();
        }

        var user = await _userManager.GetUserAsync(User);

        var post = new Post
        {
            Title = title,
            Content = content,
            UserId = user.Id,
            CreatedAt = System.DateTime.Now
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    //AFTER CREATING POST
    [Authorize]
    public IActionResult Post()
    {
        return View(); // Shows the Create.cshtml view
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Post(Post model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);

        model.UserId = user.Id;
        model.CreatedAt = DateTime.Now;

        _context.Posts.Add(model);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    // Add a reply to a post
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddReply(int postId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            TempData["ReplyError"] = "Reply cannot be empty.";
            return RedirectToAction("Index");
        }

        var user = await _userManager.GetUserAsync(User);

        var reply = new Reply
        {
            PostId = postId,
            Content = content,
            UserId = user.Id,
            RepliedAt = DateTime.Now
        };

        _context.Replies.Add(reply);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    // TO DELETE A POST
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var post = await _context.Posts
            .Include(p => p.Replies)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null) return NotFound();
        if (post.UserId != user.Id) return Forbid();

        _context.Replies.RemoveRange(post.Replies);
        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}
