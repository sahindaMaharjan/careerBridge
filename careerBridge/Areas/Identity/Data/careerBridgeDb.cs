// File: Areas/Identity/Data/careerBridgeDb.cs
using System;
using System.Collections.Generic;
using careerBridge.Models;
using careerBridge.Areas.Identity.Data;
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

        public DbSet<MentorSession> MentorSessions { get; set; }
        public DbSet<MentorSessionRegistration> MentorSessionRegistrations { get; set; }

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
                .OnDelete(DeleteBehavior.Restrict);   // ← was Cascade

            modelBuilder.Entity<Reply>()
                .HasOne(r => r.User)
                .WithMany()  // assuming careerBridgeUser has no Replies nav
                .HasForeignKey(r => r.UserId)
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

            // === MESSAGE RELATION ===
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

            // === MENTOR SESSION & REGISTRATION ===
            modelBuilder.Entity<MentorSession>()
                .HasMany(s => s.Registrations)
                .WithOne(r => r.MentorSession)
                .HasForeignKey(r => r.MentorSessionID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MentorSessionRegistration>()
                .HasOne(r => r.Student)
                .WithMany()
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MentorSessionRegistration>()
                .Property(r => r.Status)
                .HasConversion<int>();

            modelBuilder.Entity<MentorSession>()
                .HasOne(s => s.Mentor)
                .WithMany(m => m.MentorSessions)
                .HasForeignKey(s => s.MentorID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
