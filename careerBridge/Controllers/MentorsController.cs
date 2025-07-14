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

        // ── Landing to Dashboard ─────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Index() => await Dashboard();

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            // 1. Identify the current mentor
            var user = await _userManager.GetUserAsync(User);
            var mentor = await _context.Mentors
                .FirstOrDefaultAsync(m => m.UserID == user!.Id);
            if (mentor == null)
                return NotFound("Mentor profile not found.");

            // 2. Fetch accepted registrations for this mentor's sessions → mentees
            var mentees = await _context.MentorSessionRegistrations
                .Where(r => r.MentorSession.MentorID == mentor.MentorID
                         && r.Status == RegistrationStatus.Accepted)
                .Include(r => r.Student)
                .Select(r => r.Student)
                .Distinct()
                .ToListAsync();

            // 3. Upcoming sessions (all sessions by this mentor whose date ≥ today)
            var upcoming = await _context.MentorSessions
                .Where(s => s.MentorID == mentor.MentorID && s.SessionDate >= DateTime.Today)
                .ToListAsync();

            // 4. Any pending registrations to review
            var pending = await _context.MentorSessionRegistrations
                .Where(r => r.MentorSession.MentorID == mentor.MentorID
                         && r.Status == RegistrationStatus.Pending)
                .Include(r => r.Student)
                .Include(r => r.MentorSession)
                .ToListAsync();

            var vm = new MentorDashboardViewModel
            {
                Mentees = mentees,
                UpcomingSessions = upcoming,
                PendingRegistrations = pending
            };

            return View(vm);
        }

        // ── List Your Sessions ──────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Sessions()
        {
            var user = await _userManager.GetUserAsync(User);
            var mentor = await _context.Mentors
                .FirstOrDefaultAsync(m => m.UserID == user!.Id);
            if (mentor == null)
                return NotFound("Mentor profile not found.");

            var sessions = await _context.MentorSessions
                .Where(s => s.MentorID == mentor.MentorID)
                .Include(s => s.Registrations)
                .ToListAsync();

            return View(sessions);
        }

        // ── Create a New Session ────────────────────────────────────────────
        [HttpGet]
        public IActionResult CreateSession()
            => View(new MentorSession { SessionDate = DateTime.Today });

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSession(MentorSession model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            var mentor = await _context.Mentors
                .FirstOrDefaultAsync(m => m.UserID == user!.Id);
            if (mentor == null)
                return NotFound("Mentor profile not found.");

            model.MentorID = mentor.MentorID;
            _context.MentorSessions.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Session created!";
            return RedirectToAction(nameof(Sessions));
        }

        // ── Manage Registrations for One Session ──────────────────────────
        [HttpGet]
        public async Task<IActionResult> ManageRegistrations(int id)
        {
            var session = await _context.MentorSessions
                .Include(s => s.Registrations)
                    .ThenInclude(r => r.Student)
                .FirstOrDefaultAsync(s => s.MentorSessionID == id);

            if (session == null)
                return NotFound("Session not found.");

            return View(session);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRegistration(int registrationId, RegistrationStatus status)
        {
            var reg = await _context.MentorSessionRegistrations.FindAsync(registrationId);
            if (reg == null)
                return NotFound("Registration not found.");

            reg.Status = status;
            await _context.SaveChangesAsync();

            TempData[$"Notif_{reg.StudentId}"] = $"Your session request was {status}.";
            return RedirectToAction(nameof(ManageRegistrations), new { id = reg.MentorSessionID });
        }
    }
}
