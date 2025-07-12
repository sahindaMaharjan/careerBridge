using Microsoft.AspNetCore.Mvc;

namespace careerBridge.Controllers
{
    public class StudentController : Controller
    {
        // This will pick up Views/Student/Index.cshtml
        public IActionResult Index()
        {
            return View();
        }
    }
}
