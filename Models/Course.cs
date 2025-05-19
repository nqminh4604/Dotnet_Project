using System.ComponentModel.DataAnnotations;

namespace Dotnet_Project.Models
{
    public class Course
    {
        public long Id { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Course code cannot be longer than 20 characters.")]
        public string Code { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Course name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        public string? Description { get; set; }

        public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
    }
}
