using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace careerBridge.Models
{
    public class Message
    {
        [Key]
        public int MessageID { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime SentOn { get; set; } = DateTime.Now;

        // === Student (int-based) ===
        public int? SenderStudentID { get; set; }
        public int? ReceiverStudentID { get; set; }

        // === Mentor (int-based) ===
        public int? SenderMentorID { get; set; }
        public int? ReceiverMentorID { get; set; }

        // === Employer (int-based) ===
        public int? SenderEmployerID { get; set; }
        public int? ReceiverEmployerID { get; set; }

        // === Navigation Properties ===
        [ForeignKey("SenderStudentID")]
        public StudentProfile? SenderStudent { get; set; }

        [ForeignKey("ReceiverStudentID")]
        public StudentProfile? ReceiverStudent { get; set; }

        [ForeignKey("SenderMentorID")]
        public MentorProfile? SenderMentor { get; set; }

        [ForeignKey("ReceiverMentorID")]
        public MentorProfile? ReceiverMentor { get; set; }

        [ForeignKey("SenderEmployerID")]
        public EmployerProfile? SenderEmployer { get; set; }

        [ForeignKey("ReceiverEmployerID")]
        public EmployerProfile? ReceiverEmployer { get; set; }

        // ✅ Computed Properties (not stored in DB)
        [NotMapped]
        public string StudentName =>
            ReceiverStudent?.FullName ??
            SenderStudent?.FullName ??
            "Unknown";

        [NotMapped]
        public string LastMessageSnippet =>
            Content?.Length > 50 ? Content.Substring(0, 50) + "..." : Content;
    }
}
