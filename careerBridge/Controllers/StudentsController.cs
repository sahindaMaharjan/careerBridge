using careerBridge.Areas.Identity.Data;
using careerBridge.Models;
using careerBridge.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace careerBridge.Controllers
{
    public class StudentController : Controller
    {
        private readonly JobSearchService _jobSearchService;
        private readonly UserManager<careerBridgeUser> _userManager;
        private readonly careerBridgeDb _context;
        private readonly string _eventbriteToken;

        public StudentController(careerBridgeDb context, UserManager<careerBridgeUser> userManager, JobSearchService jobSearchService, IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _jobSearchService = jobSearchService;
            _eventbriteToken = config["Eventbrite:Token"];
        }

        // EVENT LIST USING EVENTBRITE API
        [HttpGet]       


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
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var student = await _context.Students
                .Include(s => s.RequestedMentors)
                .FirstOrDefaultAsync(s => s.UserID == userId);

            if (student == null)
                return Unauthorized();

            var mentors = await _context.Mentors.ToListAsync();

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

            var student = await _context.Students
                .Include(s => s.RequestedMentors)
                .FirstOrDefaultAsync(s => s.UserID == userId);

            if (student == null)
                return NotFound("Student profile not found.");

            var mentor = await _context.Mentors.FindAsync(mentorId);
            if (mentor == null)
                return NotFound("Mentor not found.");

            if (!student.RequestedMentors.Any(rm => rm.MentorID == mentorId))
            {
                student.RequestedMentors.Add(mentor);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(BookMentor));
        }

        public async Task<IActionResult> MentorDetails(int id)
        {
            var mentor = await _context.Mentors
                .FirstOrDefaultAsync(m => m.MentorID == id);

            if (mentor == null)
                return NotFound();

            return View(mentor);
        }
    }
}
