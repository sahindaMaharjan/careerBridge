using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace careerBridge.ViewModels
{
    public class ApplyJobViewModel
    {
        [Required]
        public int JobListingID { get; set; }

        public string JobTitle { get; set; } = string.Empty;

        public string CompanyName { get; set; } = string.Empty;

        [Display(Name = "Cover Letter (optional)")]
        public string? CoverLetter { get; set; }

        [Required(ErrorMessage = "Please upload your resume.")]
        public IFormFile Resume { get; set; } = null!;
    }
}
