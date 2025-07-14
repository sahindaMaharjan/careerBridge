using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using careerBridge.Areas.Identity.Data;
using careerBridge.Models;

namespace careerBridge.Controllers
{
    public class ChatController : Controller
    {
        private readonly careerBridgeDb _context;
        private readonly UserManager<careerBridgeUser> _userManager;

        public ChatController(careerBridgeDb context, UserManager<careerBridgeUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var otherUser = await _context.Users.FindAsync(userId);

            if (otherUser == null || currentUser.Id == userId)
                return NotFound();

            bool allowed = (currentUser.RoleType == "Student" && (otherUser.RoleType == "Employer" || otherUser.RoleType == "Mentor"))
                        || ((currentUser.RoleType == "Employer" || currentUser.RoleType == "Mentor") && otherUser.RoleType == "Student");

            if (!allowed)
                return Unauthorized();

            var messages = await _context.Messages
                .Where(m => (m.SenderId == currentUser.Id && m.ReceiverId == userId) ||
                            (m.SenderId == userId && m.ReceiverId == currentUser.Id))
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .OrderBy(m => m.SentOn)
                .ToListAsync();

            ViewBag.CurrentUserId = currentUser.Id;
            ViewBag.ChatUserId = userId;
            ViewBag.OtherUser = otherUser.Fullname;

            return View(messages);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string receiverId, string content)
        {
            var sender = await _userManager.GetUserAsync(User);

            var message = new Message
            {
                SenderId = sender.Id,
                ReceiverId = receiverId,
                Content = content
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { userId = receiverId });
        }

        public async Task<IActionResult> SelectUser()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }
    }
}
