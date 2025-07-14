using careerBridge.Areas.Identity.Data;
using careerBridge.Models;
using careerBridge.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace careerBridge.Controllers
{
    public class JobController : Controller
    {
        private readonly careerBridgeDb _context;
        private readonly UserManager<careerBridgeUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public JobController(careerBridgeDb context, UserManager<careerBridgeUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        // === View Job Details ===
        public async Task<IActionResult> Details(int id)
        {
            var job = await _context.JobListings
                .Include(j => j.Employer)
                .FirstOrDefaultAsync(j => j.JobListingID == id);

            if (job == null)
                return NotFound();

            return View(job);
        }

        // === Edit Job Listing ===
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var job = await _context.JobListings.FindAsync(id);
            if (job == null)
                return NotFound();

            return View(job);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, JobListing updatedJob)
        {
            if (id != updatedJob.JobListingID)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(updatedJob);

            var existingJob = await _context.JobListings.FirstOrDefaultAsync(j => j.JobListingID == id);
            if (existingJob == null)
                return NotFound();

            existingJob.Title = updatedJob.Title;
            existingJob.Description = updatedJob.Description;
            existingJob.Salary = updatedJob.Salary;
            existingJob.Location = updatedJob.Location;
            existingJob.MaxApplicants = updatedJob.MaxApplicants;
            existingJob.IsOpen = updatedJob.IsOpen;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Job updated successfully!";
            return RedirectToAction("Index", "Employer");
        }

        // === Apply for Job (GET) ===
        [Authorize(Roles = "Student")]
        [HttpGet]
        public async Task<IActionResult> Apply(int id)
        {
            var job = await _context.JobListings
                .Include(j => j.Employer)
                .FirstOrDefaultAsync(j => j.JobListingID == id);

            if (job == null || !job.IsOpen)
            {
                TempData["ErrorMessage"] = "This job is unavailable or closed.";
                return RedirectToAction("Details", new { id });
            }

            var model = new ApplyJobViewModel
            {
                JobListingID = job.JobListingID,
                JobTitle = job.Title,
                CompanyName = job.Employer.CompanyName
            };

            return View(model);
        }

        // === Apply for Job (POST) ===
        [HttpPost]
        [Authorize(Roles = "Student")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(ApplyJobViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserID == user.Id);

            if (student == null)
            {
                TempData["ErrorMessage"] = "Student profile not found.";
                return RedirectToAction("Details", new { id = model.JobListingID });
            }

            var job = await _context.JobListings
                .Include(j => j.Applications)
                .FirstOrDefaultAsync(j => j.JobListingID == model.JobListingID);

            if (job == null || !job.IsOpen)
            {
                TempData["ErrorMessage"] = "This job is closed or no longer available.";
                return RedirectToAction("Details", new { id = model.JobListingID });
            }

            if (job.Applications.Count >= job.MaxApplicants)
            {
                TempData["ErrorMessage"] = "This job has reached the maximum number of applicants.";
                return RedirectToAction("Details", new { id = model.JobListingID });
            }

            // === Save resume file to "wwwroot/resumes/" ===
            var resumeFolder = Path.Combine(_env.WebRootPath, "resumes");
            if (!Directory.Exists(resumeFolder))
                Directory.CreateDirectory(resumeFolder);

            var resumeFileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.Resume.FileName)}";
            var resumePath = Path.Combine(resumeFolder, resumeFileName);

            using (var stream = new FileStream(resumePath, FileMode.Create))
            {
                await model.Resume.CopyToAsync(stream);
            }

            var application = new JobApplication
            {
                JobListingID = model.JobListingID,
                StudentID = student.StudentID,
                AppliedOn = DateTime.UtcNow,
                Status = "Pending",
                ResumePath = resumeFileName, // Store just the file name
                CoverLetter = model.CoverLetter
            };

            _context.JobApplications.Add(application);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Application submitted successfully!";
            return RedirectToAction("Details", new { id = model.JobListingID });
        }
    }
}
