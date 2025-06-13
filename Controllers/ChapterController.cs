using Microsoft.AspNetCore.Mvc;
using Dotnet_Project.Data;
using Dotnet_Project.Models;
using Dotnet_Project.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace Dotnet_Project.Controllers
{
    public class ChapterController : Controller
    {
        private readonly DotnetProjectDbContext _context;
        public ChapterController(DotnetProjectDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(long id, long? selectedLessonId, long? selectedExerciseId, int? openChapterIndex)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Id == id);
            var chapters = _context.Chapters
                .Include(ch => ch.Lessons)
                    .ThenInclude(l => l.Exercises)
                        .ThenInclude(e => e.Options)
                .Where(ch => ch.CourseId == id)
                .OrderBy(ch => ch.Id)
                .ToList();

            Lesson? selectedLesson = null;
            Exercise? selectedExercise = null;
            int? openIdx = openChapterIndex;
            int unlockedExerciseIndex = 0;
            if (TempData["UnlockedExerciseIndex"] != null)
            {
                int.TryParse(TempData["UnlockedExerciseIndex"].ToString(), out unlockedExerciseIndex);
            }

            if (selectedLessonId.HasValue)
            {
                selectedLesson = _context.Lessons
                    .Include(l => l.Exercises)
                        .ThenInclude(e => e.Options)
                    .FirstOrDefault(l => l.Id == selectedLessonId.Value);
            }
            else if (chapters.Any() && chapters.First().Lessons.Any())
            {
                selectedLesson = chapters.First().Lessons
                    .Select(l => _context.Lessons
                        .Include(x => x.Exercises)
                            .ThenInclude(e => e.Options)
                        .FirstOrDefault(x => x.Id == l.Id))
                    .FirstOrDefault();
                openIdx = 0;
            }

            if (selectedExerciseId.HasValue && selectedLesson != null)
            {
                selectedExercise = selectedLesson.Exercises
                    .FirstOrDefault(e => e.Id == selectedExerciseId.Value);
            }
            else if (selectedLesson != null && selectedLesson.Exercises.Any())
            {
                selectedExercise = selectedLesson.Exercises.First();
            }

            // Pass lock state to view
            if (selectedLesson != null && selectedExercise != null)
            {
                var exerciseList = selectedLesson.Exercises.ToList();
                int currentIdx = exerciseList.FindIndex(e => e.Id == selectedExercise.Id);
                bool isLocked = currentIdx > unlockedExerciseIndex;
                ViewData["IsExerciseLocked"] = isLocked;
            }

            var vm = new ChapterIndexViewModel
            {
                Course = course,
                Chapters = chapters,
                SelectedLesson = selectedLesson,
                SelectedExercise = selectedExercise,
                OpenChapterIndex = openIdx
            };
            return View(vm);
        }

        [HttpPost]
        public IActionResult SubmitExercise(long exerciseId, long? selectedOptionId, string? textAnswer, int? openChapterIndex)
        {
            var exercise = _context.Exercises
                .Include(e => e.Lesson)
                    .ThenInclude(l => l.Chapter)
                        .ThenInclude(c => c.Course)
                .Include(e => e.Options)
                .FirstOrDefault(e => e.Id == exerciseId);
            if (exercise == null)
            {
                return NotFound();
            }
            bool isCorrect = false;
            if (exercise.IsMultipleChoice)
            {
                var selectedOption = exercise.Options.FirstOrDefault(o => o.Id == selectedOptionId);
                isCorrect = selectedOption != null && selectedOption.IsCorrect;
            }
            else if (!string.IsNullOrWhiteSpace(textAnswer))
            {
                // Case-insensitive, ignore whitespace
                var correct = (exercise.Answer ?? string.Empty).Trim().ToLowerInvariant();
                var submitted = (textAnswer ?? string.Empty).Trim().ToLowerInvariant();
                // Calculate match percentage (number of matching chars in order)
                int matchCount = 0;
                int minLen = Math.Min(correct.Length, submitted.Length);
                for (int i = 0; i < minLen; i++)
                {
                    if (correct[i] == submitted[i]) matchCount++;
                }
                double percent = correct.Length > 0 ? (double)matchCount / correct.Length : 0;
                // Also allow if submitted contains the answer as substring (for short answers)
                bool contains = correct.Length > 0 && submitted.Contains(correct);
                isCorrect = percent >= 0.5 || contains;
            }
            // Unlock next exercise if correct
            int unlockedExerciseIndex = 0;
            var exerciseList = exercise.Lesson.Exercises.OrderBy(e => e.Id).ToList();
            int currentIdx = exerciseList.FindIndex(e => e.Id == exerciseId);
            if (isCorrect)
            {
                unlockedExerciseIndex = currentIdx + 1;
            }
            else
            {
                unlockedExerciseIndex = currentIdx;
            }
            TempData["UnlockedExerciseIndex"] = unlockedExerciseIndex;
            TempData["ExerciseResult"] = isCorrect ? "Correct!" : "Incorrect.";
            return RedirectToAction("Index", new { id = exercise.Lesson.Chapter.CourseId, selectedLessonId = exercise.LessonId, selectedExerciseId = exerciseId, openChapterIndex });
        }

        public IActionResult List()
        {
            var chapters = _context.Chapters.Include(c => c.Course).ToList();
            return View(chapters);
        }

        public IActionResult Create()
        {
            ViewBag.CourseId = new SelectList(_context.Courses, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Chapter chapter)
        {
            if (ModelState.IsValid)
            {
                _context.Chapters.Add(chapter);
                _context.SaveChanges();
                return RedirectToAction("List");
            }
            ViewBag.CourseId = new SelectList(_context.Courses, "Id", "Name", chapter.CourseId);
            return View(chapter);
        }

        public IActionResult Edit(long id)
        {
            var chapter = _context.Chapters.Find(id);
            if (chapter == null)
                return NotFound();
            ViewBag.CourseId = new SelectList(_context.Courses, "Id", "Name", chapter.CourseId);
            return View(chapter);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Chapter chapter)
        {
            if (ModelState.IsValid)
            {
                _context.Chapters.Update(chapter);
                _context.SaveChanges();
                return RedirectToAction("List");
            }
            ViewBag.CourseId = new SelectList(_context.Courses, "Id", "Name", chapter.CourseId);
            return View(chapter);
        }

        public IActionResult Delete(long id)
        {
            var chapter = _context.Chapters.Find(id);
            if (chapter == null)
                return NotFound();
            _context.Chapters.Remove(chapter);
            _context.SaveChanges();
            return RedirectToAction("List");
        }
    }
}
