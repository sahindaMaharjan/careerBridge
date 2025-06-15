using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace careerBridge.Models
{
    public class JobListing
    {
        public int JobListingID { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime PostedOn { get; set; } = DateTime.Now;

        // Foreign key
        public int EmployerID { get; set; }

        // Navigation property
        public EmployerProfile Employer { get; set; } = null!;

        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();

    }
}
