using careerBridge.Areas.Identity.Data;
using careerBridge.Models;
using careerBridge.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace careerBridge.Controllers
{
    public class StudentController : Controller
    {
        private readonly JobSearchService _jobSearchService;
        private readonly careerBridgeDb _context;

        public StudentController(careerBridgeDb context)
        {
            _context = context;
            _jobSearchService = new JobSearchService();
        }

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

        public IActionResult JobList()
        {
            var jobs = _context.JobListings.ToList();
            return View(jobs);
        }
    }
}
