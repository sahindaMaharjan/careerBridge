<<<<<<< HEAD
=======
using careerBridge.Areas.Identity.Data;
>>>>>>> 349ac193138a91b4fe1be58fd4526b6ebc73c69e
using careerBridge.Models;
using careerBridge.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
<<<<<<< HEAD
=======
﻿using careerBridge.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
>>>>>>> 349ac193138a91b4fe1be58fd4526b6ebc73c69e

namespace careerBridge.Controllers
{
    public class StudentController : Controller
    {
<<<<<<< HEAD
        private readonly JobSearchService _jobSearchService;

        public StudentController()
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
=======
        private readonly careerBridgeDb _context;
        
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public IActionResult JobList() 
        {
            var jobs = _context.JobListings.ToList();
            return View(jobs);
>>>>>>> 349ac193138a91b4fe1be58fd4526b6ebc73c69e
        }
    }
}
