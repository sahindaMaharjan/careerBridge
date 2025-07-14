// File: Models/JobListing.cs
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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

        // Salary is optional
        public decimal? Salary { get; set; }

        public string? Location { get; set; }

        public bool IsOpen { get; set; } = true;

        public DateTime PostedOn { get; set; } = DateTime.Now;

        // Foreign key to Employer
        public int EmployerID { get; set; }

        [ForeignKey("EmployerID")]
        [ValidateNever]  // Skip MVC validation for navigation property
        public EmployerProfile Employer { get; set; } = null!;

        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    }
}
