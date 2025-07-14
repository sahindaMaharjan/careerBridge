// Models/MentorSessionRegistration.cs
using careerBridge.Areas.Identity.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace careerBridge.Models
{
    public class MentorSessionRegistration
    {
        [Key]
        public int RegistrationID { get; set; }

        [Required]
        public int MentorSessionID { get; set; }

        [ForeignKey(nameof(MentorSessionID))]
        public MentorSession MentorSession { get; set; } = null!;

        [Required]
        public string StudentId { get; set; } = string.Empty;

        [ForeignKey(nameof(StudentId))]
        public careerBridgeUser Student { get; set; } = null!;

        [Required]
        public RegistrationStatus Status { get; set; } = RegistrationStatus.Pending;

        public DateTime RequestedOn { get; set; } = DateTime.UtcNow;
    }
}
