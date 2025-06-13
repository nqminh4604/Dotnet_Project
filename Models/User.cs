using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Dotnet_Project.Models
{
    public class User : IdentityUser<long>
    {

        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters.")]
        public string FullName { get; set; } = null!;

        [StringLength(50)]
        public string? Role { get; set; }

        [Phone]
        public override string? PhoneNumber { get; set; }

        [EmailAddress]
        public override string? Email { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();

    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty; // Student or Teacher
    }
}
