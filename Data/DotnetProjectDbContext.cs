using Dotnet_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dotnet_Project.Data
{
    public class DotnetProjectDbContext : IdentityDbContext<User, IdentityRole<long>, long>
    {
        public DotnetProjectDbContext(DbContextOptions<DotnetProjectDbContext> options)
            : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Chapter> Chapters { get; set; } = null!;
        public DbSet<Lesson> Lessons { get; set; } = null!;
        public DbSet<Exercise> Exercises { get; set; } = null!;
        public DbSet<Option> Options { get; set; } = null!;
        public DbSet<Submission> Submissions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Chapter>()
                .HasOne(c => c.Course)
                .WithMany(c => c.Chapters)
                .HasForeignKey(c => c.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Chapter)
                .WithMany(c => c.Lessons)
                .HasForeignKey(l => l.ChapterId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Exercise>()
                .HasOne(e => e.Lesson)
                .WithMany(l => l.Exercises)
                .HasForeignKey(e => e.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Option>()
                .HasOne(o => o.Exercise)
                .WithMany(e => e.Options)
                .HasForeignKey(o => o.ExerciseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Submission>()
                .HasOne(s => s.Exercise)
                .WithMany(e => e.Submissions)
                .HasForeignKey(s => s.ExerciseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Submission>()
                .HasOne(s => s.User)
                .WithMany(u => u.Submissions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
