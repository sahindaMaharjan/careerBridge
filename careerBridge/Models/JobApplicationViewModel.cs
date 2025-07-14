using System;

namespace careerBridge.ViewModels
{
    public class JobApplicationViewModel
    {
        public int ApplicationID { get; set; }

        public string StudentName { get; set; } = string.Empty;

        public DateTime AppliedOn { get; set; }

        public string CoverLetter { get; set; } = string.Empty;

        public string ResumePath { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";

        public string ResumeUrl => "/resumes/" + ResumePath;

        // ✅ Add these two properties to fix the errors
        public string JobTitle { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
    }
}
