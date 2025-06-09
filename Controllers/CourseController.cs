using Microsoft.AspNetCore.Mvc;
using Dotnet_Project.Data;
using Dotnet_Project.Models;
using Dotnet_Project.ViewModels;
using System.Linq;

namespace Dotnet_Project.Controllers
{
    public class CourseController : Controller
    {
        private readonly DotnetProjectDbContext _context;
        public CourseController(DotnetProjectDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? searchTerm, long? selectedCourseId)
        {
            var coursesQuery = _context.Courses.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                coursesQuery = coursesQuery.Where(c => c.Name.Contains(searchTerm) || c.Code.Contains(searchTerm));
            }
            var courses = coursesQuery.OrderBy(c => c.Name).ToList();

            Course? selectedCourse = null;
            List<Chapter> chapters = new();
            if (selectedCourseId.HasValue)
            {
                selectedCourse = _context.Courses.FirstOrDefault(c => c.Id == selectedCourseId.Value);
                if (selectedCourse != null)
                {
                    chapters = _context.Chapters.Where(ch => ch.CourseId == selectedCourse.Id)
                        .OrderBy(ch => ch.Id).ToList();
                }
            }

            var vm = new CourseIndexViewModel
            {
                Courses = courses,
                SearchTerm = searchTerm,
                SelectedCourse = selectedCourse,
                Chapters = chapters
            };
            return View(vm);
        }
    }
}
