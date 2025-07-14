using System;

namespace careerBridge.Models
{
    public class AvailableSessionViewModel
    {
        public int MentorSessionID { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; }
        public string MentorName { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int AcceptedCount { get; set; }

        /// <summary>
        /// The current student’s registration status:
        ///  - null   = never applied
        ///  - Pending, Accepted, Denied
        /// </summary>
        public RegistrationStatus? MyStatus { get; set; }
    }
}
