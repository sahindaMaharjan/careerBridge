using careerBridge.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using careerBridge.Models;

namespace careerBridge.Areas.Identity.Data;

public class careerBridgeDb : IdentityDbContext<careerBridgeUser>
{
    public careerBridgeDb(DbContextOptions<careerBridgeDb> options)
        : base(options)
    {        
    }
    public DbSet<StudentProfile> Students { get; set; }
    public DbSet<EmployerProfile> Employers { get; set; }
    public DbSet<MentorProfile> Mentors { get; set; }
    public DbSet<JobListing> JobListings { get; set; }
    public DbSet<JobApplication> Applications { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventRegistration> EventRegistrations { get; set; }
    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Disable identity for manually entered StudentID
        modelBuilder.Entity<StudentProfile>()
            .Property(s => s.StudentID)
            .ValueGeneratedNever();

        // Employer relationships
        modelBuilder.Entity<EmployerProfile>()
            .HasMany(e => e.ReceivedMessages)
            .WithOne(m => m.ReceiverEmployer)
            .HasForeignKey(m => m.ReceiverEmployerID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployerProfile>()
            .HasMany(e => e.SentMessages)
            .WithOne(m => m.SenderEmployer)
            .HasForeignKey(m => m.SenderEmployerID)
            .OnDelete(DeleteBehavior.Restrict);

        // Student relationships
        modelBuilder.Entity<StudentProfile>()
            .HasMany(s => s.ReceivedMessages)
            .WithOne(m => m.ReceiverStudent)
            .HasForeignKey(m => m.ReceiverStudentID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StudentProfile>()
            .HasMany(s => s.SentMessages)
            .WithOne(m => m.SenderStudent)
            .HasForeignKey(m => m.SenderStudentID)
            .OnDelete(DeleteBehavior.Restrict);

        // Mentor relationships
        modelBuilder.Entity<MentorProfile>()
            .HasMany(m => m.ReceivedMessages)
            .WithOne(msg => msg.ReceiverMentor)
            .HasForeignKey(msg => msg.ReceiverMentorID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MentorProfile>()
            .HasMany(m => m.SentMessages)
            .WithOne(msg => msg.SenderMentor)
            .HasForeignKey(msg => msg.SenderMentorID)
            .OnDelete(DeleteBehavior.Restrict);
    }

}
