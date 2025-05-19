using System.ComponentModel.DataAnnotations;

namespace Dotnet_Project.Models
{
    public class Submission
    {
        public long Id { get; set; }

        [Required]
        public string SubmissionContent { get; set; }

        public float? Score { get; set; }

        [Required]
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public long ExerciseId { get; set; }

        public virtual Exercise Exercise { get; set; } = null!;

        public long UserId { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
