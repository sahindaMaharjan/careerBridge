using System;
using System.Collections.Generic;
using System.Linq;
using careerBridge.Areas.Identity.Data;

namespace careerBridge.Models
{
    public class MentorDashboardViewModel
    {
        /// <summary>
        /// All of the students whose session requests you have accepted.
        /// </summary>
        public List<careerBridgeUser> Mentees { get; set; } = new();

        /// <summary>
        /// Your upcoming sessions (today or in the future).
        /// </summary>
        public List<MentorSession> UpcomingSessions { get; set; } = new();

        /// <summary>
        /// All pending registration requests across *all* your sessions.
        /// </summary>
        public List<MentorSessionRegistration> PendingRegistrations { get; set; } = new();
    }
}
