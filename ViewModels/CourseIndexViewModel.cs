using System.Collections.Generic;
using Dotnet_Project.Models;

namespace Dotnet_Project.ViewModels
{
    public class CourseIndexViewModel
    {
        public List<Course> Courses { get; set; } = new();
        public string? SearchTerm { get; set; }
        public Course? SelectedCourse { get; set; }
        public List<Chapter> Chapters { get; set; } = new();
    }
}
