// File: Controllers/StudentController.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using careerBridge.Areas.Identity.Data;
using careerBridge.Models;
using careerBridge.Services;
using careerBridge.ViewModels;
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
            _eventbriteToken = config["Eventbrite:Token"]!;
        }

<<<<<<< HEAD
        public async Task<IActionResult> Index(string searchQuery, string location, int? posted, int? minSalary)
=======
        // ────────────────────────────────────────────────────────────────────────────
        // External Eventbrite API
        // ────────────────────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> EventList(
            string keyword,
            string location,
            string startDate,
            string endDate)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _eventbriteToken);

            // … your existing Eventbrite search logic …

            var model = new EventListViewModel
            {
                SearchResults = new List<EventItem>(),
                DefaultEvents = new List<EventItem>()
            };
            return View(model);
        }

        // ────────────────────────────────────────────────────────────────────────────
        // External Job Search + Dashboard Counts
        // ────────────────────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Index(
            string searchQuery,
            string location,
            int? posted,
            int? minSalary)
>>>>>>> 340ce559dcf2625076fae191da88cdc738a2c09a
        {
            // 1) Get current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            // 2) Look up numeric StudentID
            var studentProfile = await _context.Students
                .FirstOrDefaultAsync(s => s.UserID == user.Id);

            if (studentProfile != null)
            {
                int sid = studentProfile.StudentID;

                // 3a) Count applications
                ViewBag.AppliedJobCount = await _context.JobApplications
                    .CountAsync(a => a.StudentID == sid);

                // 3b) Count chats
                ViewBag.ChatCount = await _context.Messages
                    .CountAsync(m => m.SenderId == user.Id || m.ReceiverId == user.Id);

                // 3c) Count event registrations
                ViewBag.EventRegisteredCount = await _context.EventRegistrations
                    .CountAsync(r => r.StudentID == sid);
            }
            else
            {
                ViewBag.AppliedJobCount = 0;
                ViewBag.ChatCount = 0;
                ViewBag.EventRegisteredCount = 0;
            }

            // 4) External job search logic
            var jobList = new List<ExternalJobViewModel>();
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
<<<<<<< HEAD
                var json = await _jobSearchService.SearchJobsAsync(searchQuery, location, posted, minSalary);
=======
                var json = await _jobSearchService
                                  .SearchJobsAsync(searchQuery, location, posted, minSalary);
>>>>>>> 340ce559dcf2625076fae191da88cdc738a2c09a
                var response = JsonConvert.DeserializeObject<JobApiResponse>(json);
                if (response?.Data != null)
                    jobList = response.Data;
            }
            return View(jobList);
        }

        public IActionResult Apply(string jobTitle, string company)
        {
            TempData["Message"] = $"You applied for: {jobTitle} at {company}";
            return RedirectToAction(nameof(JobList));
        }

        public async Task<IActionResult> JobList()
        {
            var jobs = await _context.JobListings.Include(j => j.Employer).OrderByDescending(j => j.PostedOn).ToListAsync();
            return View(jobs);
        }

        public async Task<IActionResult> JobDetails(int id)
        {
<<<<<<< HEAD
            var job = await _context.JobListings.Include(j => j.Employer).FirstOrDefaultAsync(j => j.JobListingID == id);
=======
            var job = await _context.JobListings
                                    .Include(j => j.Employer)
                                    .FirstOrDefaultAsync(j => j.JobListingID == id);
>>>>>>> 340ce559dcf2625076fae191da88cdc738a2c09a
            if (job == null) return NotFound();
            return View(job);
        }

        public async Task<IActionResult> BookMentor()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return RedirectToAction("Login", "Account");

<<<<<<< HEAD
            var student = await _context.Students.Include(s => s.RequestedMentors).FirstOrDefaultAsync(s => s.UserID == userId);
=======
            var student = await _context.Students
                .Include(s => s.RequestedMentors)
                .FirstOrDefaultAsync(s => s.UserID == userId);
>>>>>>> 340ce559dcf2625076fae191da88cdc738a2c09a
            if (student == null) return Unauthorized();

            var mentors = await _context.Mentors.ToListAsync();
            var model = mentors.Select(m => new BookMentorViewModel
            {
                MentorID = m.MentorID,
                FullName = m.FullName,
                Requested = student.RequestedMentors.Any(rm => rm.MentorID == m.MentorID)
            }).ToList();

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMentorRequest(int mentorId)
        {
            var userId = _userManager.GetUserId(User)!;
<<<<<<< HEAD
            var student = await _context.Students.Include(s => s.RequestedMentors).FirstOrDefaultAsync(s => s.UserID == userId);
=======
            var student = await _context.Students
                .Include(s => s.RequestedMentors)
                .FirstOrDefaultAsync(s => s.UserID == userId);
>>>>>>> 340ce559dcf2625076fae191da88cdc738a2c09a
            if (student == null) return NotFound();

            if (!student.RequestedMentors.Any(rm => rm.MentorID == mentorId))
            {
                var mentor = await _context.Mentors.FindAsync(mentorId);
                if (mentor != null)
                {
                    student.RequestedMentors.Add(mentor);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(BookMentor));
        }

        public async Task<IActionResult> MentorDetails(int id)
        {
<<<<<<< HEAD
            var mentor = await _context.Mentors.FirstOrDefaultAsync(m => m.MentorID == id);
=======
            var mentor = await _context.Mentors
                                      .FirstOrDefaultAsync(m => m.MentorID == id);
>>>>>>> 340ce559dcf2625076fae191da88cdc738a2c09a
            if (mentor == null) return NotFound();
            return View(mentor);
        }

        public async Task<IActionResult> Events()
        {
            var events = await _context.Events.Include(e => e.Employer).OrderBy(e => e.EventDate).ToListAsync();
            return View(events);
        }

        public async Task<IActionResult> Details(int id)
        {
<<<<<<< HEAD
            var ev = await _context.Events.Include(e => e.Employer).FirstOrDefaultAsync(e => e.EventID == id);
=======
            var ev = await _context.Events
                                   .Include(e => e.Employer)
                                   .FirstOrDefaultAsync(e => e.EventID == id);
>>>>>>> 340ce559dcf2625076fae191da88cdc738a2c09a
            if (ev == null) return NotFound();
            return View(ev);
        }

<<<<<<< HEAD
=======
        // ────────────────────────────────────────────────────────────────────────────
        // Mentor Sessions (New Feature)
        // ────────────────────────────────────────────────────────────────────────────
        [HttpGet]
>>>>>>> 340ce559dcf2625076fae191da88cdc738a2c09a
        public async Task<IActionResult> AvailableSessions()
        {
            var me = await _userManager.GetUserAsync(User)!;

            var sessions = await _context.MentorSessions
                .Include(s => s.Mentor)
                .Include(s => s.Registrations)
                .ToListAsync();

            var model = sessions.Select(s =>
            {
                var mine = s.Registrations.FirstOrDefault(r => r.StudentId == me.Id);

                return new AvailableSessionViewModel
                {
                    MentorSessionID = s.MentorSessionID,
                    Title = s.Title,
                    SessionDate = s.SessionDate,
                    MentorName = s.Mentor.FullName,
                    Capacity = s.Capacity,
                    AcceptedCount = s.Registrations.Count(r => r.Status == RegistrationStatus.Accepted),
                    MyStatus = mine?.Status
                };
            }).ToList();

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterSession(int sessionId)
        {
            var me = await _userManager.GetUserAsync(User)!;

            bool exists = await _context.MentorSessionRegistrations.AnyAsync(r => r.MentorSessionID == sessionId && r.StudentId == me.Id);

            if (exists)
                TempData["Error"] = "You’ve already requested this session.";
            else
            {
                _context.MentorSessionRegistrations.Add(new MentorSessionRegistration
                {
                    MentorSessionID = sessionId,
                    StudentId = me.Id
                });
                await _context.SaveChangesAsync();
                TempData["Success"] = "Session request sent!";
            }

            return RedirectToAction(nameof(AvailableSessions));
        }

<<<<<<< HEAD
=======
        [HttpGet]
>>>>>>> 340ce559dcf2625076fae191da88cdc738a2c09a
        public async Task<IActionResult> MyRegistrations()
        {
            var me = await _userManager.GetUserAsync(User)!;

            var regs = await _context.MentorSessionRegistrations
                .Where(r => r.StudentId == me.Id)
                .Include(r => r.MentorSession).ThenInclude(s => s.Mentor)
                .ToListAsync();

            return View(regs);
        }

<<<<<<< HEAD
        public async Task<IActionResult> MyApplications()
=======
        // ────────────────────────────────────────────────────────────────────────────
        // Calendar / Reminders
        // ────────────────────────────────────────────────────────────────────────────

        // Renders the student calendar page
        [HttpGet]
        public IActionResult Calendar()
        {
            return View();
        }

        // Returns accepted sessions as JSON events for FullCalendar
        [HttpGet]
        public async Task<IActionResult> GetCalendarEvents()
>>>>>>> 340ce559dcf2625076fae191da88cdc738a2c09a
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

<<<<<<< HEAD
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserID == user.Id);
            if (student == null) return NotFound("Student profile not found.");

            var applications = await _context.JobApplications
                .Include(a => a.JobListing).ThenInclude(j => j.Employer)
                .Where(a => a.StudentID == student.StudentID)
                .OrderByDescending(a => a.AppliedOn)
               .Select(a => new StudentApplicationViewModel
               {
                   ApplicationID = a.ApplicationID,
                   JobTitle = a.JobListing.Title,
                   CompanyName = a.JobListing.Employer.CompanyName,
                   AppliedOn = a.AppliedOn,
                   Status = a.Status,
                   ResumePath = a.ResumePath,
                   CoverLetter = a.CoverLetter
               })
                .ToListAsync();

            return View(applications);
=======
            var studentProfile = await _context.Students
                .FirstOrDefaultAsync(s => s.UserID == user.Id);
            if (studentProfile == null) return Unauthorized();

            // no sid needed here
            var regs = await _context.MentorSessionRegistrations
                .Where(r => r.StudentId == user.Id
                         && r.Status == RegistrationStatus.Accepted)
                .Include(r => r.MentorSession)
                .ToListAsync();

            var events = regs.Select(r => new
            {
                title = r.MentorSession.Title,
                start = r.MentorSession.SessionDate.ToString("s"),
                id = r.RegistrationID
            });

            return Json(events);
>>>>>>> 340ce559dcf2625076fae191da88cdc738a2c09a
        }
    }
}