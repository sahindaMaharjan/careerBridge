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

        // ✅ FIXED EVENT LIST METHOD
        [HttpGet]
        public async Task<IActionResult> EventList(string keyword, string location, string startDate, string endDate)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _eventbriteToken);

            var searchEvents = new List<EventItem>();
            var defaultEvents = new List<EventItem>();

            // === SEARCHED EVENTS ===
            if (!string.IsNullOrEmpty(keyword) || !string.IsNullOrEmpty(location) || !string.IsNullOrEmpty(startDate) || !string.IsNullOrEmpty(endDate))
            {
                var searchParams = new List<string>();

                if (!string.IsNullOrEmpty(keyword))
                    searchParams.Add($"q={Uri.EscapeDataString(keyword)}");

                if (!string.IsNullOrEmpty(location))
                    searchParams.Add($"location.address={Uri.EscapeDataString(location)}");

                if (!string.IsNullOrEmpty(startDate))
                    searchParams.Add($"start_date.range_start={Uri.EscapeDataString(startDate)}");

                if (!string.IsNullOrEmpty(endDate))
                    searchParams.Add($"start_date.range_end={Uri.EscapeDataString(endDate)}");

                var searchUrl = "https://www.eventbriteapi.com/v3/events/search/";
                if (searchParams.Count > 0)
                    searchUrl += "?" + string.Join("&", searchParams) + "&expand=venue";

                var searchResponse = await client.GetAsync(searchUrl);
                if (searchResponse.IsSuccessStatusCode)
                {
                    var json = await searchResponse.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<eventbriteResponse>(json);
                    searchEvents = result?.events ?? new List<EventItem>();
                }
                else
                {
                    var errorJson = await searchResponse.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Error fetching search events: {searchResponse.StatusCode} - {errorJson}";
                }
            }

            // === DEFAULT EVENTS FALLBACK ===
            var defaultUrl = "https://www.eventbriteapi.com/v3/events/search/?q=tech&location.address=Canada&expand=venue";

            var defaultResponse = await client.GetAsync(defaultUrl);

            if (defaultResponse.IsSuccessStatusCode)
            {
                var json = await defaultResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<eventbriteResponse>(json);
                defaultEvents = result?.events ?? new List<EventItem>();
            }
            else
            {
                var errorJson = await defaultResponse.Content.ReadAsStringAsync();
                TempData["Error"] = $"Error fetching default events: {defaultResponse.StatusCode} - {errorJson}";
            }

            var model = new EventListViewModel
            {
                SearchResults = searchEvents,
                DefaultEvents = defaultEvents
            };

            return View(model);
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
            var mentor = await _context.Mentors.FirstOrDefaultAsync(m => m.MentorID == id);
            if (mentor == null)
                return NotFound();

            return View(mentor);
        }
    }
}
