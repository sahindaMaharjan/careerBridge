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

        [ForeignKey("ReceiverId")]
        public careerBridgeUser Receiver { get; set; }
    }
}
