namespace careerBridge.ViewModels
{
    public class StudentApplicationViewModel
    {
        public int ApplicationID { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public DateTime AppliedOn { get; set; }
        public string ResumePath { get; set; } = string.Empty;
        public string CoverLetter { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public string ResumeUrl => "/resumes/" + ResumePath;
    }
}
