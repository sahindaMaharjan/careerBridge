using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace careerBridge.Controllers
{
    public class EventbriteTestController : Controller
    {
        private readonly string _eventbriteToken;

        public EventbriteTestController(IConfiguration config)
        {
            _eventbriteToken = config["Eventbrite:Token"];
        }

        [HttpGet]

        [Route("Test/Eventbrite")]
        public async Task<IActionResult> CheckToken()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _eventbriteToken);

            var response = await client.GetAsync("https://www.eventbriteapi.com/v3/users/me/");
            var json = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return Content("✅ Token is working!\n\n" + json, "text/plain");
            }
            else
            {
                return Content($"❌ Token failed: {response.StatusCode}\n\n{json}", "text/plain");
            }
        }
    }
}
