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

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = "";

        public string? Description { get; set; }

        [Required]
        public string Location { get; set; } = "";

        [Required]
        public DateTime EventDate { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        [ForeignKey("Mentor")]
        public int MentorID { get; set; }

        // Navigation
        public MentorProfile Mentor { get; set; } = null!;

        public ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();
    }
}
