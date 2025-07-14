using System.Linq;
using System.Threading.Tasks;
using careerBridge.Areas.Identity.Data;
using careerBridge.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace careerBridge.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly careerBridgeDb _context;
        private readonly UserManager<careerBridgeUser> _userManager;

        public ChatController(careerBridgeDb context, UserManager<careerBridgeUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Chat/Index?userId={otherUserId}
        public async Task<IActionResult> Index(string userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || currentUser.Id == userId)
                return NotFound();

            var otherUser = await _userManager.FindByIdAsync(userId);
            if (otherUser == null)
                return NotFound();

            // Determine roles
            var meRole = currentUser.RoleType;
            var themRole = otherUser.RoleType;

            // Only allow Student↔Mentor chats if there's an accepted session registration
            if (meRole == "Student" && themRole == "Mentor")
            {
                var ok = await _context.MentorSessionRegistrations
                    .Include(r => r.MentorSession)
                        .ThenInclude(s => s.Mentor)
                    .AnyAsync(r =>
                        r.StudentId == currentUser.Id
                        && r.Status == RegistrationStatus.Accepted
                        && r.MentorSession.Mentor.UserID == otherUser.Id
                    );
                if (!ok) return Forbid();
            }
            else if (meRole == "Mentor" && themRole == "Student")
            {
                var ok = await _context.MentorSessionRegistrations
                    .Include(r => r.MentorSession)
                        .ThenInclude(s => s.Mentor)
                    .AnyAsync(r =>
                        r.StudentId == otherUser.Id
                        && r.Status == RegistrationStatus.Accepted
                        && r.MentorSession.Mentor.UserID == currentUser.Id
                    );
                if (!ok) return Forbid();
            }
            // (Optional) allow Employer↔Student chats unconditionally, or customize:
            else if ((meRole == "Employer" && themRole == "Student") ||
                     (meRole == "Student" && themRole == "Employer"))
            {
                // keep your original check or just allow:
                // bool allowed = true;
            }
            else
            {
                // disallow any other pairing
                return Forbid();
            }

            // At this point, the two users are allowed to chat
            var messages = await _context.Messages
                .Where(m =>
                    (m.SenderId == currentUser.Id && m.ReceiverId == otherUser.Id) ||
                    (m.SenderId == otherUser.Id && m.ReceiverId == currentUser.Id)
                )
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .OrderBy(m => m.SentOn)
                .ToListAsync();

            ViewBag.CurrentUserId = currentUser.Id;
            ViewBag.ChatUserId = otherUser.Id;
            ViewBag.OtherUser = otherUser.Fullname;

            return View(messages);
        }

        // POST: /Chat/SendMessage
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(string receiverId, string content)
        {
            var sender = await _userManager.GetUserAsync(User);
            if (sender == null || receiverId == null || content == null)
                return BadRequest();

            // Optionally re-run the same authorization check here...

            var message = new Message
            {
                SenderId = sender.Id,
                ReceiverId = receiverId,
                Content = content
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { userId = receiverId });
        }

        // GET: /Chat/SelectUser
        public async Task<IActionResult> SelectUser()
        {
            // You can restrict which users show up here if you want
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }
    }
}
