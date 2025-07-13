using careerBridge.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using careerBridge.Services;


public class StudentController : Controller
{
    private readonly JobSearchService _jobSearchService;

    public StudentController()
    {
        _jobSearchService = new JobSearchService();
    }

    public async Task<IActionResult> Index(string searchQuery)
    {
        List<ExternalJobViewModel> jobList = new List<ExternalJobViewModel>();

        if (!string.IsNullOrEmpty(searchQuery))
        {
            var json = await _jobSearchService.SearchJobsAsync(searchQuery);
            jobList = JsonConvert.DeserializeObject<List<ExternalJobViewModel>>(json);
        }

        return View(jobList);
    }
}
