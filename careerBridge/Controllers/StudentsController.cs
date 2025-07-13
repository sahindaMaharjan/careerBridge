using careerBridge.Areas.Identity.Data;
using careerBridge.Models;
using careerBridge.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
﻿using careerBridge.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace careerBridge.Controllers
{
    public class StudentController : Controller
    {
        private readonly careerBridgeDb _context;
        
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public IActionResult JobList() 
        {
            var jobs = _context.JobListings.ToList();
            return View(jobs);
        }
    }
}
