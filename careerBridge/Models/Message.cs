using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using careerBridge.Areas.Identity.Data;

namespace careerBridge.Models
{
    public class Message
    {
        [Key]
        public int MessageID { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime SentOn { get; set; } = DateTime.UtcNow;

        [Required]
        public string SenderId { get; set; }

        [Required]
        public string ReceiverId { get; set; }

        [ForeignKey("SenderId")]
        public careerBridgeUser Sender { get; set; }

<<<<<<< HEAD
        [ForeignKey("ReceiverId")]
        public careerBridgeUser Receiver { get; set; }
=======
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
>>>>>>> 693cd619f0f4f08bc37d4f5def0a3a76d1c80f9d
    }
}
