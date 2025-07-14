using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using careerBridge.Models;
using careerBridge.Areas.Identity.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace careerBridge.Controllers
{
    [Authorize(Roles = "Employer")]
    public class EmployerController : Controller
    {
        private readonly careerBridgeDb _context;
        private readonly UserManager<careerBridgeUser> _userManager;

        public EmployerController(careerBridgeDb context, UserManager<careerBridgeUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // === Dashboard ===
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var employer = await _context.Employers
                .FirstOrDefaultAsync(e => e.UserID == user.Id);
            if (employer == null) return NotFound("Employer profile not found.");

            var jobs = await _context.JobListings
                .Where(j => j.EmployerID == employer.EmployerID)
                .Include(j => j.Applications)
                .ToListAsync();

            var events = await _context.Events
                .OrderByDescending(e => e.EventDate)
                .Take(5)
                .Include(e => e.EventRegistrations)
                .ToListAsync();

            var messages = await _context.Messages
                .Where(m => m.SenderEmployerID == employer.EmployerID || m.ReceiverEmployerID == employer.EmployerID)
                .OrderByDescending(m => m.SentOn)
                .Take(5)
                .ToListAsync();

            var model = new EmployerDashboardViewModel
            {
                JobCount = jobs.Count,
                EventCount = events.Count,
                ChatCount = messages.Count,

                Jobs = jobs.Select(j => new JobViewModel
                {
                    Id = j.JobListingID,
                    Title = j.Title,
                    PostedOn = j.PostedOn,
                    ApplicantCount = j.Applications?.Count ?? 0,
                    IsOpen = j.IsOpen
                }).ToList(),

                Events = events.Select(e => new EventViewModel
                {
                    Id = e.EventID,
                    Name = e.Title,
                    Date = e.EventDate,
                    RegistrationCount = e.EventRegistrations?.Count() ?? 0
                }).ToList(),

                RecentChats = messages.Select(m => new ChatSummary
                {
                    StudentId = m.ReceiverStudentID ?? 0,
                    StudentName = m.StudentName,
                    LastMessageSnippet = m.LastMessageSnippet,
                    LastMessageTime = m.SentOn
                }).ToList()
            };

            return View(model);
        }

        // === GET: PostJob ===
        [HttpGet]
        public IActionResult PostJob()
        {
            return View();
        }

        // === POST: PostJob ===
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostJob(JobListing job)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized();

                var employer = await _context.Employers.FirstOrDefaultAsync(e => e.UserID == user.Id);
                if (employer == null)
                {
                    ModelState.AddModelError("", "Employer profile not found.");
                    return View(job);
                }

                job.EmployerID = employer.EmployerID;
                job.PostedOn = DateTime.Now;

                _context.JobListings.Add(job);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Job posted successfully!";
                return RedirectToAction("PostJobConfirmation");
            }

            return View(job);
        }

        // === GET: PostJobConfirmation ===
        [HttpGet]
        public IActionResult PostJobConfirmation()
        {
            return View();
        }
    }
}
