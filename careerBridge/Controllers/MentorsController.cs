using System;
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
    [Authorize(Roles = "Mentor")]
    public class MentorController : Controller
    {
        private readonly careerBridgeDb _context;
        private readonly UserManager<careerBridgeUser> _userManager;

        public MentorController(careerBridgeDb context, UserManager<careerBridgeUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Mentor or /Mentor/Index
        // you can use this as the "dashboard" landing
        [HttpGet]
        public async Task<IActionResult> Index()
            => await Sessions();

        // GET: /Mentor/Dashboard
        // alias for Index
        [HttpGet]
        public async Task<IActionResult> Dashboard()
            => await Sessions();

        // GET: /Mentor/Sessions
        [HttpGet]
        public async Task<IActionResult> Sessions()
        {
            var user = await _userManager.GetUserAsync(User);
            var mentor = await _context.Mentors
                .FirstAsync(m => m.UserID == user!.Id);

            var sessions = await _context.MentorSessions
                .Where(s => s.MentorID == mentor.MentorID)
                .Include(s => s.Registrations)
                .ToListAsync();

            return View(sessions);
        }

        // GET: /Mentor/CreateSession
        [HttpGet]
        public IActionResult CreateSession()
            => View(new MentorSession { SessionDate = DateTime.Today });

        // POST: /Mentor/CreateSession
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSession(MentorSession model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            var mentor = await _context.Mentors
                .FirstAsync(m => m.UserID == user!.Id);

            model.MentorID = mentor.MentorID;
            _context.MentorSessions.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Session created!";
            return RedirectToAction(nameof(Sessions));
        }

        // GET: /Mentor/ManageRegistrations/5
        [HttpGet]
        public async Task<IActionResult> ManageRegistrations(int id)
        {
            var session = await _context.MentorSessions
                .Include(s => s.Registrations)
                    .ThenInclude(r => r.Student)
                .FirstOrDefaultAsync(s => s.MentorSessionID == id);

            if (session == null)
                return NotFound();

            return View(session);
        }

        // POST: /Mentor/UpdateRegistration
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRegistration(int registrationId, RegistrationStatus status)
        {
            var reg = await _context.MentorSessionRegistrations.FindAsync(registrationId);
            if (reg == null)
                return NotFound();

            reg.Status = status;
            await _context.SaveChangesAsync();

            // Optionally notify student via TempData
            TempData[$"Notif_{reg.StudentId}"] = $"Your session request was {status}.";

            return RedirectToAction(nameof(ManageRegistrations), new { id = reg.MentorSessionID });
        }
    }
}
