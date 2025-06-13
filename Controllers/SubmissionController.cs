using Microsoft.AspNetCore.Mvc;
using Dotnet_Project.Data;
using Dotnet_Project.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dotnet_Project.Controllers
{
    public class SubmissionController : Controller
    {
        private readonly DotnetProjectDbContext _context;
        public SubmissionController(DotnetProjectDbContext context)
        {
            _context = context;
        }

        public IActionResult List()
        {
            var submissions = _context.Submissions.Include(s => s.Exercise).Include(s => s.User).ToList();
            return View(submissions);
        }

        public IActionResult Create()
        {
            ViewBag.ExerciseId = new SelectList(_context.Exercises, "Id", "Question");
            ViewBag.UserId = new SelectList(_context.Users, "Id", "UserName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Submission submission)
        {
            if (ModelState.IsValid)
            {
                _context.Submissions.Add(submission);
                _context.SaveChanges();
                return RedirectToAction("List");
            }
            ViewBag.ExerciseId = new SelectList(_context.Exercises, "Id", "Question", submission.ExerciseId);
            ViewBag.UserId = new SelectList(_context.Users, "Id", "UserName", submission.UserId);
            return View(submission);
        }

        public IActionResult Edit(long id)
        {
            var submission = _context.Submissions.Find(id);
            if (submission == null)
                return NotFound();
            ViewBag.ExerciseId = new SelectList(_context.Exercises, "Id", "Question", submission.ExerciseId);
            ViewBag.UserId = new SelectList(_context.Users, "Id", "UserName", submission.UserId);
            return View(submission);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Submission submission)
        {
            if (ModelState.IsValid)
            {
                _context.Submissions.Update(submission);
                _context.SaveChanges();
                return RedirectToAction("List");
            }
            ViewBag.ExerciseId = new SelectList(_context.Exercises, "Id", "Question", submission.ExerciseId);
            ViewBag.UserId = new SelectList(_context.Users, "Id", "UserName", submission.UserId);
            return View(submission);
        }

        public IActionResult Delete(long id)
        {
            var submission = _context.Submissions.Find(id);
            if (submission == null)
                return NotFound();
            _context.Submissions.Remove(submission);
            _context.SaveChanges();
            return RedirectToAction("List");
        }
    }
}
