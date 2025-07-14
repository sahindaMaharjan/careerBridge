using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using careerBridge.Areas.Identity.Data;
using careerBridge.Models;
using careerBridge.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace careerBridge.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly careerBridgeDb _context;
        private readonly UserManager<careerBridgeUser> _userManager;
        private readonly JobSearchService _jobSearchService;
        private readonly string _eventbriteToken;

        public StudentController(
            careerBridgeDb context,
            UserManager<careerBridgeUser> userManager,
            JobSearchService jobSearchService,
            IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _jobSearchService = jobSearchService;
            _eventbriteToken = config["Eventbrite:Token"];
        }

        // External Eventbrite API
        [HttpGet]
        public async Task<IActionResult> EventList(string keyword, string location, string startDate, string endDate)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _eventbriteToken);

            var searchEvents = new List<EventItem>();
            var defaultEvents = new List<EventItem>();

            // ... search logic omitted for brevity ...

            var model = new EventListViewModel
            {
                SearchResults = searchEvents,
                DefaultEvents = defaultEvents
            };

            return View(model);
        }

        // External job search
        [HttpGet]
        public async Task<IActionResult> Index(string searchQuery, string location, int? posted, int? minSalary)
        {
            var jobList = new List<ExternalJobViewModel>();
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var json = await _jobSearchService.SearchJobsAsync(searchQuery, location, posted, minSalary);
                var response = JsonConvert.DeserializeObject<JobApiResponse>(json);
                if (response?.Data != null)
                    jobList = response.Data;
            }
            return View(jobList);
        }

        // Apply for an external or internal job
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Apply(string jobTitle, string company)
        {
            TempData["Message"] = $"You applied for: {jobTitle} at {company}";
            return RedirectToAction(nameof(JobList));
        }

        // Internal jobs posted by employers
        [HttpGet]
        public async Task<IActionResult> JobList()
        {
            var jobs = await _context.JobListings
                .Include(j => j.Employer)
                .OrderByDescending(j => j.PostedOn)
                .ToListAsync();

            return View(jobs);
        }

        // Details for a single job
        [HttpGet]
        public async Task<IActionResult> JobDetails(int id)
        {
            var job = await _context.JobListings
                .Include(j => j.Employer)
                .FirstOrDefaultAsync(j => j.JobListingID == id);
            if (job == null)
                return NotFound();
            return View(job);
        }

        // Mentor booking - GET
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

        // Mentor booking - POST
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
                return NotFound();

            var mentor = await _context.Mentors.FindAsync(mentorId);
            if (mentor == null)
                return NotFound();

            if (!student.RequestedMentors.Any(rm => rm.MentorID == mentorId))
            {
                student.RequestedMentors.Add(mentor);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(BookMentor));
        }

        // Mentor details
        [HttpGet]
        public async Task<IActionResult> MentorDetails(int id)
        {
            var mentor = await _context.Mentors.FirstOrDefaultAsync(m => m.MentorID == id);
            if (mentor == null)
                return NotFound();
            return View(mentor);
        }

        // Student view of employer-posted events
        [HttpGet]
        public async Task<IActionResult> Events()
        {
            var events = await _context.Events
                .Include(e => e.Employer)
                .OrderBy(e => e.EventDate)
                .ToListAsync();

            return View(events);
        }

        // Student details for a single event
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var ev = await _context.Events
                .Include(e => e.Employer)
                .FirstOrDefaultAsync(e => e.EventID == id);
            if (ev == null)
                return NotFound();

            return View(ev);
        }
    }
}
