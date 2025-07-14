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
        public string Title { get; set; } = "";

        public string? Description { get; set; }

        public DateTime EventDate { get; set; }

        // — Employer posts this event now —
        [Required]
        [ForeignKey(nameof(Employer))]
        public int EmployerID { get; set; }
        public EmployerProfile Employer { get; set; } = null!;

        public ICollection<EventRegistration> EventRegistrations { get; set; }
            = new List<EventRegistration>();
    }
}
