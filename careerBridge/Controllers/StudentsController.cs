using Microsoft.AspNetCore.Mvc;

namespace careerBridge.Controllers
{
    public class StudentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
