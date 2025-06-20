using Microsoft.AspNetCore.Mvc;
using Dotnet_Project.Data;
using Dotnet_Project.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace Dotnet_Project.Controllers
{
    public class ExerciseController : Controller
    {
        private readonly DotnetProjectDbContext _context;
        private readonly ILogger<ExerciseController> _logger;

        public ExerciseController(DotnetProjectDbContext context, ILogger<ExerciseController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult List()
        {
            var exercises = _context.Exercises.Include(e => e.Lesson).ToList();
            return View(exercises);
        }

        public IActionResult Create()
        {
            ViewBag.LessonId = new SelectList(_context.Lessons, "Id", "Title");
            // Pass a new Exercise instance to the view
            return View(new Exercise());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Exercise exercise)
        {
            
            if (exercise.IsMultipleChoice && exercise.Options != null)
            {
                // Remove empty options (in case user left blank rows)
                exercise.Options = exercise.Options.Where(o => !string.IsNullOrWhiteSpace(o.Content)).ToList();
                // Set the relationship for each option
                foreach (var option in exercise.Options)
                {
                    option.Exercise = exercise;
                }
            }
            
            if (ModelState.IsValid)
            {
                _context.Exercises.Add(exercise);
                _context.SaveChanges();
                return RedirectToAction("List");
            }
            ViewBag.LessonId = new SelectList(_context.Lessons, "Id", "Title", exercise.LessonId);
            return View(exercise);
        }

        public IActionResult Edit(long id)
        {
            var exercise = _context.Exercises
                .Include(e => e.Options)
                .FirstOrDefault(e => e.Id == id);
            if (exercise == null)
                return NotFound();
            ViewBag.LessonId = new SelectList(_context.Lessons, "Id", "Title", exercise.LessonId);
            return View(exercise);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Exercise exercise)
        {
            if (ModelState.IsValid)
            {
                // Load the existing exercise and its options
                var existingExercise = _context.Exercises
                    .Include(e => e.Options)
                    .FirstOrDefault(e => e.Id == exercise.Id);
                if (existingExercise == null)
                    return NotFound();

                // Update scalar properties
                existingExercise.Question = exercise.Question;
                existingExercise.Answer = exercise.Answer;
                existingExercise.IsMultipleChoice = exercise.IsMultipleChoice;
                existingExercise.LessonId = exercise.LessonId;
                existingExercise.UpdatedAt = DateTime.UtcNow;

                // Remove all existing options
                _context.Options.RemoveRange(existingExercise.Options);
                // Add new options
                if (exercise.Options != null)
                {
                    foreach (var option in exercise.Options)
                    {
                        option.ExerciseId = existingExercise.Id;
                        _context.Options.Add(option);
                    }
                }
                _context.SaveChanges();
                return RedirectToAction("List");
            }
            ViewBag.LessonId = new SelectList(_context.Lessons, "Id", "Title", exercise.LessonId);
            return View(exercise);
        }

        public IActionResult Delete(long id)
        {
            var exercise = _context.Exercises.Find(id);
            if (exercise == null)
                return NotFound();
            _context.Exercises.Remove(exercise);
            _context.SaveChanges();
            return RedirectToAction("List");
        }
    }
}
