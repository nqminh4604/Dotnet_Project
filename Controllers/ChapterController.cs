using Microsoft.AspNetCore.Mvc;
using Dotnet_Project.Data;
using Dotnet_Project.Models;
using Dotnet_Project.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Dotnet_Project.Controllers
{
    public class ChapterController : Controller
    {
        private readonly DotnetProjectDbContext _context;
        private readonly UserManager<User> _userManager;

        public ChapterController(DotnetProjectDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
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
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> SubmitExercise(long exerciseId, long? selectedOptionId, string? textAnswer, int? openChapterIndex)
        {
            // Defensive: ensure exerciseId is valid
            if (exerciseId <= 0)
            {
                TempData["ExerciseResult"] = "Invalid exercise.";
                return RedirectToAction("Index");
            }
            // Query with minimal includes to avoid nulls
            var exercise = await _context.Exercises
                .AsNoTracking()
                .Include(e => e.Options)
                .Include(e => e.Lesson)
                    .ThenInclude(l => l.Chapter)
                        .ThenInclude(c => c.Course)
                .FirstOrDefaultAsync(e => e.Id == exerciseId);
            if (exercise == null)
            {
                TempData["ExerciseResult"] = "Exercise not found.";
                return RedirectToAction("Index");
            }
            // Defensive: check for required fields in Exercise
            if (string.IsNullOrWhiteSpace(exercise.Question) || (exercise.IsMultipleChoice && (exercise.Options == null || !exercise.Options.Any())))
            {
                TempData["ExerciseResult"] = "Exercise data is incomplete.";
                return RedirectToAction("Index", new { id = exercise.Lesson?.Chapter?.CourseId ?? 0 });
            }
            bool isCorrect = false;
            string submissionContent = string.Empty;
            if (exercise.IsMultipleChoice)
            {
                if (selectedOptionId == null)
                {
                    TempData["ExerciseResult"] = "Please select an option.";
                    return RedirectToAction("Index", new { id = exercise.Lesson.Chapter.CourseId, selectedLessonId = exercise.LessonId, selectedExerciseId = exerciseId, openChapterIndex });
                }
                var selectedOption = exercise.Options.FirstOrDefault(o => o.Id == selectedOptionId);
                isCorrect = selectedOption != null && selectedOption.IsCorrect;
                submissionContent = selectedOption?.Content ?? string.Empty;
            }
            else if (!string.IsNullOrWhiteSpace(textAnswer))
            {
                var correct = (exercise.Answer ?? string.Empty).Trim().ToLowerInvariant();
                var submitted = (textAnswer ?? string.Empty).Trim().ToLowerInvariant();
                int matchCount = 0;
                int minLen = Math.Min(correct.Length, submitted.Length);
                for (int i = 0; i < minLen; i++)
                {
                    if (correct[i] == submitted[i]) matchCount++;
                }
                double percent = correct.Length > 0 ? (double)matchCount / correct.Length : 0;
                bool contains = correct.Length > 0 && submitted.Contains(correct);
                isCorrect = percent >= 0.5 || contains;
                submissionContent = textAnswer ?? string.Empty;
            }
            else
            {
                TempData["ExerciseResult"] = "Please provide an answer.";
                return RedirectToAction("Index", new { id = exercise.Lesson.Chapter.CourseId, selectedLessonId = exercise.LessonId, selectedExerciseId = exerciseId, openChapterIndex });
            }
            // Save submission
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var submission = new Submission
                {
                    UserId = user.Id,
                    ExerciseId = exerciseId,
                    SubmissionContent = submissionContent,
                    Score = isCorrect ? 100 : 0,
                    SubmittedAt = DateTime.UtcNow
                };
                _context.Submissions.Add(submission);
                await _context.SaveChangesAsync();
            }
            // Unlock next exercise if correct
            int unlockedExerciseIndex = 0;
            var exerciseList = (await _context.Exercises.Where(e => e.LessonId == exercise.LessonId).OrderBy(e => e.Id).ToListAsync());
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
    }
}
