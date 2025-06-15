using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace careerBridge.Areas.Identity.Data;

// Add profile data for application users by adding properties to the careerBridgeUser class
public class careerBridgeUser : IdentityUser
{

    public string Fullname { get; set; }

    public string RoleType { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

