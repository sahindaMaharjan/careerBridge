// Models/MentorSession.cs
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace careerBridge.Models
{
    public class MentorSession
    {
        [Key]
        public int MentorSessionID { get; set; }

        [Required, StringLength(100)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required, DataType(DataType.DateTime)]
        public DateTime SessionDate { get; set; }

        [Required, Range(1, 100)]
        public int Capacity { get; set; }

        // FK to Mentor
        [Required]
        public int MentorID { get; set; }

        [ValidateNever]
        [ForeignKey(nameof(MentorID))]
        public MentorProfile Mentor { get; set; } = null!;

        public ICollection<MentorSessionRegistration> Registrations { get; set; }
            = new List<MentorSessionRegistration>();
    }
}
