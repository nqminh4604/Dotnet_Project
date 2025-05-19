using System.ComponentModel.DataAnnotations;

namespace Dotnet_Project.Models
{
    public class Chapter
    {
        public long Id { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Chapter title cannot be longer than 500 characters.")]
        public string Title { get; set; }

        public long CourseId { get; set; }

        public virtual Course Course { get; set; } = null!;

        public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    }
}
