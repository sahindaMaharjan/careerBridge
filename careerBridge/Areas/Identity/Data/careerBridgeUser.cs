using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using careerBridge.Models;
using Microsoft.AspNetCore.Identity;

namespace careerBridge.Areas.Identity.Data;

// Add profile data for application users by adding properties to the careerBridgeUser class
public class careerBridgeUser : IdentityUser
{

    public string Fullname { get; set; }

    public string RoleType { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public StudentProfile StudentProfile { get; set; }
    public EmployerProfile EmployerProfile { get; set; }
    public MentorProfile MentorProfile { get; set; }

}

