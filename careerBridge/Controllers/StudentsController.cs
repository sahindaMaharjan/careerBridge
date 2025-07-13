using careerBridge.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace careerBridge.Controllers
{
    public class StudentController : Controller
    {
        // This will pick up Views/Student/Index.cshtml
        private readonly careerBridgeDb _context;
        public IActionResult Index()
        {
            return View();
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
