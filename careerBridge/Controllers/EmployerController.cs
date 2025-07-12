using Microsoft.AspNetCore.Mvc;
using careerBridge.Models;
using System;

namespace careerBridge.Controllers
{
    public class EmployerController : Controller
    {
        public IActionResult Index()
        {
            // TODO: replace with real data retrieval from database or services
            var model = new EmployerDashboardViewModel
            {
                JobCount = 5,
                ChatCount = 12,
                EventCount = 3,

                Jobs = new[]
                {
                    new JobViewModel
                    {
                        Id = 1,
                        Title = "Full-Stack Developer",
                        PostedOn = DateTime.Today.AddDays(-10),
                        ApplicantCount = 8,
                        IsOpen = true
                    },
                    new JobViewModel
                    {
                        Id = 2,
                        Title = "UI/UX Designer",
                        PostedOn = DateTime.Today.AddDays(-20),
                        ApplicantCount = 5,
                        IsOpen = false
                    }
                },

                Events = new[]
                {
                    new EventViewModel
                    {
                        Id = 1,
                        Name = "Career Fair",
                        Date = DateTime.Today.AddDays(5),
                        RegistrationCount = 25
                    },
                    new EventViewModel
                    {
                        Id = 2,
                        Name = "Tech Talk",
                        Date = DateTime.Today.AddDays(15),
                        RegistrationCount = 40
                    }
                },

                RecentChats = new[]
                {
                    new ChatSummary
                    {
                        StudentId = 101,
                        StudentName = "Alice J.",
                        LastMessageSnippet = "Thanks for your time",
                        LastMessageTime = DateTime.Now.AddHours(-2)
                    },
                    new ChatSummary
                    {
                        StudentId = 102,
                        StudentName = "Mark L.",
                        LastMessageSnippet = "I sent my resume",
                        LastMessageTime = DateTime.Now.AddHours(-5)
                    }
                }
            };

            return View(model);
        }
    }
}
