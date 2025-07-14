using careerBridge.Areas.Identity.Data;
using careerBridge.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace careerBridge.Areas.Identity.Data
{
    public class careerBridgeDb : IdentityDbContext<careerBridgeUser>
    {
        public careerBridgeDb(DbContextOptions<careerBridgeDb> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Reply> Replies { get; set; }
        public DbSet<StudentProfile> Students { get; set; }

        public DbSet<EmployerProfile> Employers { get; set; }
        public DbSet<MentorProfile> Mentors { get; set; }
        public DbSet<JobListing> JobListings { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventRegistration> EventRegistrations { get; set; }

        public DbSet<Message> Messages { get; set; } // ✅ Shared chat model

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // === STUDENT ↔ MENTOR RELATIONSHIP ===
            modelBuilder.Entity<StudentProfile>()
                .HasMany(s => s.RequestedMentors)
                .WithMany(m => m.RequestedByStudents)
                .UsingEntity<Dictionary<string, object>>(
                    "MentorRequests",
                    j => j.HasOne<MentorProfile>()
                          .WithMany()
                          .HasForeignKey("MentorId")
                          .OnDelete(DeleteBehavior.Restrict),
                    j => j.HasOne<StudentProfile>()
                          .WithMany()
                          .HasForeignKey("StudentId")
                          .OnDelete(DeleteBehavior.Restrict),
                    j =>
                    {
                        j.HasKey("StudentId", "MentorId");
                        j.Property<DateTime>("RequestedAt").HasDefaultValueSql("GETDATE()");
                        j.Property<bool>("IsApproved").HasDefaultValue(false);
                    }
                );

            // === REPLY CASCADE OFF ===
            modelBuilder.Entity<Reply>()
                .HasOne(r => r.Post)
                .WithMany(p => p.Replies)
                .HasForeignKey(r => r.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            // === STUDENT ID AUTO GEN ===
            modelBuilder.Entity<StudentProfile>()
                .Property(s => s.StudentID)
                .ValueGeneratedOnAdd();

            // === JOB APPLICATION RULES ===
            modelBuilder.Entity<JobApplication>()
                .HasOne(ja => ja.Student)
                .WithMany(s => s.JobApplications)
                .HasForeignKey(ja => ja.StudentID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<JobApplication>()
                .HasOne(ja => ja.JobListing)
                .WithMany(j => j.Applications)
                .HasForeignKey(ja => ja.JobListingID)
                .OnDelete(DeleteBehavior.Cascade);

            // === EVENT REGISTRATION RULES ===
            modelBuilder.Entity<EventRegistration>()
                .HasOne(er => er.Student)
                .WithMany(s => s.EventRegistrations)
                .HasForeignKey(er => er.StudentID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventRegistration>()
                .HasOne(er => er.Event)
                .WithMany(e => e.EventRegistrations)
                .HasForeignKey(er => er.EventID)
                .OnDelete(DeleteBehavior.Cascade);

            // === SIMPLIFIED MESSAGE RELATION ===
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
