using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace careerBridge.Models
{
    public class EventRegistration
    {
        [Key]
        public int EventRegistrationID { get; set; }

        // Foreign key to Student (now using int)
        [Required]
        public int StudentID { get; set; }

        // Foreign key to Event
        [Required]
        public int EventID { get; set; }

        public DateTime RegisteredOn { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("StudentID")]
        public StudentProfile Student { get; set; } = null!;

        [ForeignKey("EventID")]
        public Event Event { get; set; } = null!;
    }
}
