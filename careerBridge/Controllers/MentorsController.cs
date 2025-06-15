using Microsoft.AspNetCore.Mvc;

namespace careerBridge.Controllers
{
    public class MentorsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
