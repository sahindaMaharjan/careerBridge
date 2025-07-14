// File: Models/ChatSummary.cs
using System;

namespace careerBridge.Models
{
    /// <summary>
    /// A lightweight DTO for displaying recent chats on the Employer dashboard.
    /// </summary>
    public class ChatSummary
    {
        /// <summary>
        /// The student’s Identity User-ID (string), used to route to the chat.
        /// </summary>
        public string StudentId { get; set; } = string.Empty;

        /// <summary>
        /// The student’s display name.
        /// </summary>
        public string StudentName { get; set; } = string.Empty;

        /// <summary>
        /// A snippet of the last message.
        /// </summary>
        public string LastMessageSnippet { get; set; } = string.Empty;

        /// <summary>
        /// When the last message was sent.
        /// </summary>
        public DateTime LastMessageTime { get; set; }
    }
}
