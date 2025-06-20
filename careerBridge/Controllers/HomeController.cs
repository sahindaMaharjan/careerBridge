using System.Diagnostics;
using careerBridge.Models;
using Microsoft.AspNetCore.Mvc;

namespace careerBridge.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Student()
        {
            return View();
        }

        public IActionResult Mentor()
        {
            return View();
        }

        public IActionResult chat()
        {
            return View();
        }
        public IActionResult Mentorstd()
        {
            return View();
        }
        public IActionResult GroupChat()
        {

            return View();
        }

        public IActionResult Community()
        {
            return View();
        }
            
      
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
