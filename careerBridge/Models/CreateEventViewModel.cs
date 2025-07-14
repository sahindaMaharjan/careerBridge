// File: Models/CreateEventViewModel.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace careerBridge.Models
{
    public class CreateEventViewModel
    {
        [Required]
        public string Title { get; set; } = "";

        [Required]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        public string? Description { get; set; }
    }
}
