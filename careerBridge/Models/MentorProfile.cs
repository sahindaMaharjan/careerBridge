// File: Models/MentorProfile.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using careerBridge.Areas.Identity.Data;

namespace careerBridge.Models
{
    public class MentorProfile
    {
        [Key]
        public int MentorID { get; set; }

        [Required]
        public string UserID { get; set; } = string.Empty;

        [ForeignKey(nameof(UserID))]
        public careerBridgeUser User { get; set; } = null!;

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, Phone]
        public string Phone { get; set; } = string.Empty;

        public string? ExpertiseArea { get; set; }
        public string? CertificatePath { get; set; }

        // Existing navigation
        public ICollection<StudentProfile> RequestedByStudents { get; set; }
            = new List<StudentProfile>();
        public ICollection<Event> Events { get; set; }
            = new List<Event>();
        public ICollection<Message> SentMessages { get; set; }
            = new List<Message>();
        public ICollection<Message> ReceivedMessages { get; set; }
            = new List<Message>();

        // ← Add this for your MentorSessions:
        public ICollection<MentorSession> MentorSessions { get; set; }
            = new List<MentorSession>();
    }
}
