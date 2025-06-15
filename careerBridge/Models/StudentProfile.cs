using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;

namespace careerBridge.Models
{
    public class StudentProfile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // Prevent auto-increment
        public int StudentID { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; } = "";

        [EmailAddress]
        public string Email { get; set; } = "";

        [Phone]
        public string Phone { get; set; } = "";

        [Required]
        public string CollegeName { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";

        // Navigation
        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
        public ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();
        public ICollection<Message> SentMessages { get; set; } = new List<Message>();
        public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
    }
}