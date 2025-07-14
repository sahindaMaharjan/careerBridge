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
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure many-to-many join table between Students and Mentors
            modelBuilder.Entity<StudentProfile>()
                .HasMany(s => s.RequestedMentors)
                .WithMany(m => m.RequestedByStudents)
                .UsingEntity<Dictionary<string, object>>(
                    "MentorRequests", // join table name
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
                        j.HasKey("StudentId", "MentorId");  // composite key
                        j.Property<DateTime>("RequestedAt")
                            .HasDefaultValueSql("GETDATE()"); // set current time by default
                        j.Property<bool>("IsApproved")
                            .HasDefaultValue(false); // default false
                    }
                );

            // Disable cascade delete on Replies -> Posts
            modelBuilder.Entity<Reply>()
                .HasOne(r => r.Post)
                .WithMany(p => p.Replies)
                .HasForeignKey(r => r.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            // Auto-generate Student ID
            modelBuilder.Entity<StudentProfile>()
                .Property(s => s.StudentID)
                .ValueGeneratedOnAdd();

            // --- JobApplication ---
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

            // --- EventRegistration ---
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

            // --- Messages ---
            modelBuilder.Entity<Message>()
                .HasOne(m => m.SenderStudent)
                .WithMany(s => s.SentMessages)
                .HasForeignKey(m => m.SenderStudentID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.ReceiverStudent)
                .WithMany(s => s.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverStudentID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.SenderMentor)
                .WithMany(men => men.SentMessages)
                .HasForeignKey(m => m.SenderMentorID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.ReceiverMentor)
                .WithMany(men => men.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverMentorID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.SenderEmployer)
                .WithMany(emp => emp.SentMessages)
                .HasForeignKey(m => m.SenderEmployerID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.ReceiverEmployer)
                .WithMany(emp => emp.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverEmployerID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
