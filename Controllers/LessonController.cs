using Microsoft.AspNetCore.Mvc;
using Dotnet_Project.Data;
using Dotnet_Project.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dotnet_Project.Controllers
{
    public class LessonController : Controller
    {
        private readonly DotnetProjectDbContext _context;
        public LessonController(DotnetProjectDbContext context)
        {
            _context = context;
        }

        public IActionResult List()
        {
            var lessons = _context.Lessons.Include(l => l.Chapter).ToList();
            return View(lessons);
        }

        public IActionResult Create()
        {
            ViewBag.ChapterId = new SelectList(_context.Chapters, "Id", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Lesson lesson)
        {
            if (ModelState.IsValid)
            {
                _context.Lessons.Add(lesson);
                _context.SaveChanges();
                return RedirectToAction("List");
            }
            ViewBag.ChapterId = new SelectList(_context.Chapters, "Id", "Title", lesson.ChapterId);
            return View(lesson);
        }

        public IActionResult Edit(long id)
        {
            var lesson = _context.Lessons.Find(id);
            if (lesson == null)
                return NotFound();
            ViewBag.ChapterId = new SelectList(_context.Chapters, "Id", "Title", lesson.ChapterId);
            return View(lesson);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Lesson lesson)
        {
            if (ModelState.IsValid)
            {
                _context.Lessons.Update(lesson);
                _context.SaveChanges();
                return RedirectToAction("List");
            }
            ViewBag.ChapterId = new SelectList(_context.Chapters, "Id", "Title", lesson.ChapterId);
            return View(lesson);
        }

        public IActionResult Delete(long id)
        {
            var lesson = _context.Lessons.Find(id);
            if (lesson == null)
                return NotFound();
            _context.Lessons.Remove(lesson);
            _context.SaveChanges();
            return RedirectToAction("List");
        }
    }
}
