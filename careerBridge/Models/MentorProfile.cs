using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace careerBridge.Models
{
    public class MentorProfile
    {
        [Key]
        public int MentorID { get; set; }  // was string, now int


        [Required]
        public string FullName { get; set; } = "";

        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";

        [Required]
        public string Phone { get; set; }
        public string? ExpertiseArea { get; set; }

        public string? CertificatePath { get; set; }

        // Navigation properties
        public ICollection<Event> Events { get; set; } = new List<Event>();
        public ICollection<Message> SentMessages { get; set; } = new List<Message>();
        public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
    }
}
