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

        public DateTime Date { get; set; }

        [Required]
        [ForeignKey("Mentor")]
        public int MentorID { get; set; } // Changed from string to int

        // Navigation
        public MentorProfile Mentor { get; set; } = null!;

        public ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();

    }
}
