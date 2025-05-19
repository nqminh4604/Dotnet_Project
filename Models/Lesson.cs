using System.ComponentModel.DataAnnotations;

namespace Dotnet_Project.Models
{
    public class Lesson
    {
        public long Id { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Lesson title cannot be longer than 500 characters.")]
        public string Title { get; set; }

        public long ChapterId { get; set; }

        public virtual Chapter Chapter { get; set; } = null!;

        public virtual ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}
