<<<<<<< HEAD
﻿using careerBridge.Models;
using careerBridge.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
=======
﻿using careerBridge.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
>>>>>>> a123d17c6224b5412124d9fbdcf593af9c3c41ad

namespace careerBridge.Controllers
{
    public class StudentController : Controller
    {
<<<<<<< HEAD
        private readonly JobSearchService _jobSearchService;

        public StudentController()
=======
        // This will pick up Views/Student/Index.cshtml
        private readonly careerBridgeDb _context;
        public IActionResult Index()
>>>>>>> a123d17c6224b5412124d9fbdcf593af9c3c41ad
        {
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
        public StudentController(careerBridgeDb context)
        {
            _context = context;
        }

        public IActionResult JobList() 
        {
            var jobs = _context.JobListings.ToList();
            return View(jobs);
        }
    }
}
