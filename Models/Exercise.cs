using System.ComponentModel.DataAnnotations;

namespace Dotnet_Project.Models
{
    public class Exercise
    {
        public long Id { get; set; }

        [Required]
        public string Question { get; set; }

        [Required]
        public string Answer { get; set; }

        [Required]
        public bool IsMultipleChoice { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public long LessonId { get; set; }

        public virtual Lesson Lesson { get; set; } = null!;

        public virtual ICollection<Option> Options { get; set; } = new List<Option>();

        public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();


    }
}
