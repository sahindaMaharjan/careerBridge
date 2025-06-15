using careerBridge.Areas.Identity.Data;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace careerBridge.Models
{
    public class MentorProfile
    {
        [Key]
        public int MentorID { get; set; }  // was string, now int

        [ForeignKey("CareerBridgeUser")]
        [Required]
        public string UserID { get; set; }

        public careerBridgeUser User { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }
        public string? ExpertiseArea { get; set; }

        [Required]
        public string? CertificatePath { get; set; }

        // Navigation properties
        public ICollection<Event> Events { get; set; } = new List<Event>();
        public ICollection<Message> SentMessages { get; set; } = new List<Message>();
        public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
    }
}
