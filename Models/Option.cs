using System.ComponentModel.DataAnnotations;

namespace Dotnet_Project.Models
{
    public class Option
    {
        public long Id { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public bool IsCorrect { get; set; }

        public long ExerciseId { get; set; }

        public virtual Exercise Exercise { get; set; } = null!;

    }
}
