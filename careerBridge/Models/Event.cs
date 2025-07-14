// File: Models/Event.cs
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace careerBridge.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        [Required, StringLength(100)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        // Employer posts this event
        [Required, ForeignKey(nameof(Employer))]
        public int EmployerID { get; set; }

        [ValidateNever]  // skip MVC validation on the nav prop
        public EmployerProfile Employer { get; set; } = null!;

        public ICollection<EventRegistration> EventRegistrations { get; set; }
            = new List<EventRegistration>();
    }
}
