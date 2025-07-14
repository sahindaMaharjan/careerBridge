using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace careerBridge.Models
{
    public class JobListing
    {
        [Key]
        public int JobListingID { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Salary { get; set; }

        public string? Location { get; set; }

        public bool IsOpen { get; set; } = true;

        public DateTime PostedOn { get; set; } = DateTime.Now;

        // Foreign key to Employer
        public int EmployerID { get; set; }

        [ForeignKey("EmployerID")]
        public EmployerProfile Employer { get; set; } = null!;

        // ✅ Add this navigation property
        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    }
}
