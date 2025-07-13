using careerBridge.Areas.Identity.Data;
using careerBridge.Models;
using careerBridge.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace careerBridge.Controllers
{
    public class StudentController : Controller
    {
        private readonly JobSearchService _jobSearchService;
        private readonly UserManager<careerBridgeUser> _userManager;
        private readonly careerBridgeDb _context;

        public StudentController(careerBridgeDb context, UserManager<careerBridgeUser> userManager, JobSearchService jobSearchService)
        {
            _context = context;
            _userManager = userManager;
            _jobSearchService = jobSearchService;
        }

        // JOB LIST FROM EXTERNAL API
        public async Task<IActionResult> Index(string searchQuery, string location, int? posted, int? minSalary)
        {
            List<ExternalJobViewModel> jobList = new();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var json = await _jobSearchService.SearchJobsAsync(searchQuery, location, posted, minSalary);
                var response = JsonConvert.DeserializeObject<JobApiResponse>(json);
                if (response?.Data != null)
                    jobList = response.Data;
            }

            return View(jobList);
        }

        [HttpPost]
        public IActionResult Apply(string jobTitle, string company)
        {
            TempData["Message"] = $"You applied for: {jobTitle} at {company}";
            return RedirectToAction("Index");
        }

        // JOB LIST FROM EMPLOYER
        public IActionResult JobList()
        {
            var jobs = _context.JobListings.ToList();
            return View(jobs);
        }

        // MENTOR REQUEST - GET
        [HttpGet]
        public async Task<IActionResult> BookMentor()
        {
            var userId = _userManager.GetUserId(User); // logged-in Identity user Id (string)
            if (userId == null)
                return RedirectToAction("Login", "Account");

            // Get StudentProfile by UserID (int StudentID)
            var student = await _context.Students
                .Include(s => s.RequestedMentors)  // Include mentors already requested
                .FirstOrDefaultAsync(s => s.UserID == userId);

            if (student == null)
                return Unauthorized(); // Or redirect to profile creation page

            // Get all mentors
            var mentors = await _context.Mentors.ToListAsync();

            // Build view model to mark which mentors were already requested
            var model = mentors.Select(m => new BookMentorViewModel
            {
                MentorID = m.MentorID,
                FullName = m.FullName,
                Requested = student.RequestedMentors.Any(rm => rm.MentorID == m.MentorID)
            }).ToList();

            return View(model);
        }

        // MENTOR REQUEST - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMentorRequest(int mentorId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return Unauthorized();

            // Load student with requested mentors
            var student = await _context.Students
                .Include(s => s.RequestedMentors)
                .FirstOrDefaultAsync(s => s.UserID == userId);

            if (student == null)
                return NotFound("Student profile not found.");

            var mentor = await _context.Mentors.FindAsync(mentorId);
            if (mentor == null)
                return NotFound("Mentor not found.");

            // Add mentor to student's requested mentors if not already added
            if (!student.RequestedMentors.Any(rm => rm.MentorID == mentorId))
            {
                student.RequestedMentors.Add(mentor);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(BookMentor));
        }
        public async Task<IActionResult> MentorDetails(int id) // id = MentorID
        {
            var mentor = await _context.Mentors
                .FirstOrDefaultAsync(m => m.MentorID == id);

            if (mentor == null)
                return NotFound();

            return View(mentor);
        }
    }
}
