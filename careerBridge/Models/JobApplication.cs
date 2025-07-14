using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace careerBridge.Models
{
    public class JobApplication
    {
        [Key]
        public int ApplicationID { get; set; }

        [Required]
        [ForeignKey("JobListing")]
        public int JobListingID { get; set; }

        [Required]
        [ForeignKey("Student")]
        public int StudentID { get; set; }

        public DateTime AppliedOn { get; set; } = DateTime.UtcNow;

        [Required]
        public string Status { get; set; } = "Pending"; // "Pending", "Approved", "Rejected"

        [Required]
        public string ResumePath { get; set; } = string.Empty;

        public string? CoverLetter { get; set; }

        // Navigation properties
        public JobListing JobListing { get; set; } = null!;
        public StudentProfile Student { get; set; } = null!;
    }
}
