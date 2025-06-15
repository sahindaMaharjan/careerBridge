using careerBridge.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace careerBridge.Models
{
    public class StudentProfile
    {
        [Key]
        public int StudentID { get; set; }

        public string UserID { get; set; }

        [ForeignKey("UserID")]
        public careerBridgeUser User { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; } 

        public string CollegeName { get; set; } 


        // Navigation
        public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
        public ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();
        public ICollection<Message> SentMessages { get; set; } = new List<Message>();
        public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
    }
}