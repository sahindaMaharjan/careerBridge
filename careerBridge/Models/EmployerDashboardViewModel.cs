using System;
using System.Collections.Generic;

namespace careerBridge.Models
{
    public class EmployerDashboardViewModel
    {
        public int JobCount { get; set; }
        public int ChatCount { get; set; }
        public int EventCount { get; set; }

        public IEnumerable<JobViewModel> Jobs { get; set; } = new List<JobViewModel>();
        public IEnumerable<EventViewModel> Events { get; set; } = new List<EventViewModel>();
        public IEnumerable<ChatSummary> RecentChats { get; set; } = new List<ChatSummary>();
    }

    public class JobViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime PostedOn { get; set; }
        public int ApplicantCount { get; set; }
        public bool IsOpen { get; set; }
    }

    public class EventViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int RegistrationCount { get; set; }
    }

    public class ChatSummary
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string LastMessageSnippet { get; set; } = string.Empty;
        public DateTime LastMessageTime { get; set; }
    }
}
