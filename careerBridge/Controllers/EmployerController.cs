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
using careerBridge.ViewModels;

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

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var employer = await _context.Employers.FirstOrDefaultAsync(e => e.UserID == user.Id);
            if (employer == null) return NotFound("Employer profile not found.");

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
                    ApplicantCount = j.Applications.Count,
                    IsOpen = j.IsOpen
                }).ToList(),

                Events = events.Select(e => new EventViewModel
                {
                    Id = e.EventID,
                    Name = e.Title,
                    Date = e.EventDate,
                    RegistrationCount = e.EventRegistrations.Count
                }).ToList(),

                RecentChats = messages.Select(m => new ChatSummary
                {
                    StudentId = m.SenderId == user.Id ? m.ReceiverId : m.SenderId,
                    StudentName = m.SenderId == user.Id ? m.Receiver.Fullname : m.Sender.Fullname,
                    LastMessageSnippet = m.Content.Length > 50 ? m.Content.Substring(0, 50) + "..." : m.Content,
                    LastMessageTime = m.SentOn
                }).ToList()
            };

            return View(model);
        }

        public IActionResult PostJob() => View(new JobListing());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostJob(JobListing job)
        {
            if (!ModelState.IsValid) return View(job);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var employer = await _context.Employers.FirstOrDefaultAsync(e => e.UserID == user.Id);
            if (employer == null)
            {
                ModelState.AddModelError("", "Employer profile not found.");
                return View(job);
            }

            if (job.MaxApplicants < 1)
            {
                ModelState.AddModelError("MaxApplicants", "Max Applicants must be at least 1.");
                return View(job);
            }

            job.EmployerID = employer.EmployerID;
            job.PostedOn = DateTime.UtcNow;

            _context.JobListings.Add(job);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Job posted successfully!";
            return RedirectToAction(nameof(PostJobConfirmation));
        }

        public IActionResult PostJobConfirmation() => View();

        public async Task<IActionResult> CreateEvent()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var employer = await _context.Employers.FirstOrDefaultAsync(e => e.UserID == user.Id);
            if (employer == null) return NotFound("Employer profile not found.");

            return View(new CreateEventViewModel { EventDate = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(CreateEventViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _userManager.GetUserAsync(User);
            var employer = await _context.Employers.FirstOrDefaultAsync(e => e.UserID == user!.Id);

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

        public IActionResult PostEvent() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostEvent(Event model)
        {
            if (ModelState.IsValid)
            {
                _context.Events.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event posted successfully!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<IActionResult> JobListings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var employer = await _context.Employers
                .Include(e => e.JobListings)
                .FirstOrDefaultAsync(e => e.UserID == user.Id);

            if (employer == null) return NotFound();

            var jobs = employer.JobListings.Select(j => new JobViewModel
            {
                Id = j.JobListingID,
                Title = j.Title,
                PostedOn = j.PostedOn,
                ApplicantCount = j.Applications?.Count ?? 0,
                IsOpen = j.IsOpen
            }).ToList();

            return View(new EmployerDashboardViewModel { Jobs = jobs });
        }

        public async Task<IActionResult> DeleteJob(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var employer = await _context.Employers.FirstOrDefaultAsync(e => e.UserID == user.Id);
            if (employer == null) return NotFound();

            var job = await _context.JobListings
                .FirstOrDefaultAsync(j => j.JobListingID == id && j.EmployerID == employer.EmployerID);

            if (job == null)
                return NotFound("Job not found or you do not have permission.");

            return View(job);
        }

        [HttpPost, ActionName("DeleteJob")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDeleteJob(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var employer = await _context.Employers.FirstOrDefaultAsync(e => e.UserID == user.Id);
            if (employer == null) return NotFound();

            var job = await _context.JobListings
                .Include(j => j.Applications)
                .FirstOrDefaultAsync(j => j.JobListingID == id && j.EmployerID == employer.EmployerID);

            if (job == null)
                return NotFound("Job not found or you do not have permission.");

            _context.JobApplications.RemoveRange(job.Applications);
            _context.JobListings.Remove(job);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Job deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleJobStatus(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var employer = await _context.Employers.FirstOrDefaultAsync(e => e.UserID == user.Id);
            if (employer == null) return NotFound();

            var job = await _context.JobListings
                .FirstOrDefaultAsync(j => j.JobListingID == id && j.EmployerID == employer.EmployerID);

            if (job == null) return NotFound();

            job.IsOpen = !job.IsOpen;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Applications(int jobId)
        {
            var user = await _userManager.GetUserAsync(User);
            var employer = await _context.Employers.FirstOrDefaultAsync(e => e.UserID == user.Id);

            if (employer == null) return Unauthorized();

            var applications = await _context.JobApplications
                .Include(a => a.Student)
                .Include(a => a.JobListing)
                .Where(a => a.JobListing.EmployerID == employer.EmployerID && a.JobListingID == jobId)
                .Select(a => new JobApplicationViewModel
                {
                    ApplicationID = a.ApplicationID,
                    StudentName = a.Student.FullName,
                    CoverLetter = a.CoverLetter ?? "",
                    ResumePath = a.ResumePath,
                    Status = a.Status,
                    AppliedOn = a.AppliedOn
                })
                .ToListAsync();

            ViewBag.JobTitle = _context.JobListings.FirstOrDefault(j => j.JobListingID == jobId)?.Title ?? "Job";

            return View(applications);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateApplicationStatus(int applicationId, string status)
        {
            var application = await _context.JobApplications.FindAsync(applicationId);
            if (application == null)
                return NotFound();

            application.Status = status;
            await _context.SaveChangesAsync();

            return RedirectToAction("Applications", new { jobId = application.JobListingID });
        }
    }
}
