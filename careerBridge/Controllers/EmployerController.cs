// File: Controllers/EmployerController.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using careerBridge.Areas.Identity.Data;
using careerBridge.Models;

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

        // GET: /Employer
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var employer = await _context.Employers
                .FirstOrDefaultAsync(e => e.UserID == user.Id);
            if (employer == null)
                return NotFound("Employer profile not found.");

            var jobs = await _context.JobListings
                .Where(j => j.EmployerID == employer.EmployerID)
                .Include(j => j.Applications)
                .ToListAsync();

            var events = await _context.Events
                .Where(e => e.EmployerID == employer.EmployerID)
                .OrderByDescending(e => e.EventDate)
                .Take(5)
                .Include(e => e.EventRegistrations)
                .ToListAsync();

            var messages = await _context.Messages
                .Where(m => m.SenderId == user.Id || m.ReceiverId == user.Id)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
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
                    StudentId = m.SenderId == user.Id
                                                        ? m.ReceiverId
                                                        : m.SenderId,
                    StudentName = m.SenderId == user.Id
                                                        ? m.Receiver.Fullname
                                                        : m.Sender.Fullname,
                    LastMessageSnippet = m.Content.Length > 50
                                                        ? m.Content.Substring(0, 50) + "..."
                                                        : m.Content,
                    LastMessageTime = m.SentOn
                }).ToList()
            };

            return View(model);
        }

        // GET: /Employer/PostJob
        [HttpGet]
        public IActionResult PostJob()
            => View();

        // POST: /Employer/PostJob
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostJob(JobListing job)
        {
            if (!ModelState.IsValid)
                return View(job);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var employer = await _context.Employers
                .FirstOrDefaultAsync(e => e.UserID == user.Id);
            if (employer == null)
            {
                ModelState.AddModelError("", "Employer profile not found.");
                return View(job);
            }

            job.EmployerID = employer.EmployerID;
            job.PostedOn = DateTime.UtcNow;

            _context.JobListings.Add(job);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Job posted successfully!";
            return RedirectToAction(nameof(PostJobConfirmation));
        }

        // GET: /Employer/PostJobConfirmation
        [HttpGet]
        public IActionResult PostJobConfirmation()
            => View();

        // GET: /Employer/CreateEvent
        [HttpGet]
        public async Task<IActionResult> CreateEvent()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var employer = await _context.Employers
                .FirstOrDefaultAsync(e => e.UserID == user.Id);
            if (employer == null)
                return NotFound("Employer profile not found.");

            // Initialize view model
            var vm = new CreateEventViewModel
            {
                EventDate = DateTime.Today
            };
            return View(vm);
        }

        // POST: /Employer/CreateEvent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(CreateEventViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var user = await _userManager.GetUserAsync(User);
            var employer = await _context.Employers
                                .FirstOrDefaultAsync(e => e.UserID == user!.Id);

            var ev = new Event
            {
                Title = vm.Title,
                EventDate = vm.EventDate,
                Description = vm.Description,
                EmployerID = employer!.EmployerID
            };

            _context.Events.Add(ev);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Event posted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
