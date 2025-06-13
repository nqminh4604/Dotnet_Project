using Microsoft.AspNetCore.Mvc;
using Dotnet_Project.Data;
using Dotnet_Project.Models;
using Dotnet_Project.ViewModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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
                    chapters = _context.Chapters
                        .Include(ch => ch.Lessons)
                        .Where(ch => ch.CourseId == selectedCourse.Id)
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

        public IActionResult List()
        {
            var courses = _context.Courses.ToList();
            return View(courses);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Courses.Add(course);
                _context.SaveChanges();
                return RedirectToAction("List");
            }
            return View(course);
        }

        public IActionResult Edit(long id)
        {
            var course = _context.Courses.Find(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Courses.Update(course);
                _context.SaveChanges();
                return RedirectToAction("List");
            }
            return View(course);
        }

        public IActionResult Delete(long id)
        {
            var course = _context.Courses.Find(id);
            if (course == null)
                return NotFound();
            _context.Courses.Remove(course);
            _context.SaveChanges();
            return RedirectToAction("List");
        }
    }
}
