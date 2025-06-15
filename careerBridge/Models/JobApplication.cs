using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace careerBridge.Models
{
    public class JobApplication
    {
        [Key]
        public int ApplicationID { get; set; }

        [ForeignKey("JobListing")]
        public int JobListingID { get; set; }

        [ForeignKey("Student")]
        public int StudentID { get; set; }

        public DateTime AppliedOn { get; set; }

        public string Status { get; set; }

        // Navigation
        public JobListing JobListing { get; set; }
        public StudentProfile Student { get; set; }
    }
}
