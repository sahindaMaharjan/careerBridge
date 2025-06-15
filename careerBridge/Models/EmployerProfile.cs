using careerBridge.Areas.Identity.Data;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace careerBridge.Models
{
    public class EmployerProfile
    {
        [Key]
        public int EmployerID { get; set; }

        [Required]
        public string UserID { get; set; }

        [ForeignKey("UserID")]
        public careerBridgeUser User { get; set; }


        [Required, StringLength(100)]
        public string CompanyName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string? Phone { get; set; }

        public string? BusinessCertificatePath { get; set; }

        // Navigation
        
        public ICollection<JobListing> JobListings { get; set; } = new List<JobListing>();
        public ICollection<Event> Events { get; set; } = new List<Event>();
        public ICollection<Message> SentMessages { get; set; } = new List<Message>();
        public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
    }
}
