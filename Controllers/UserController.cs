using Microsoft.AspNetCore.Mvc;
using Dotnet_Project.Data;
using Dotnet_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace Dotnet_Project.Controllers
{
    public class UserController : Controller
    {
        private readonly DotnetProjectDbContext _context;
        public UserController(DotnetProjectDbContext context)
        {
            _context = context;
        }

        public IActionResult List()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("List");
            }
            return View(user);
        }

        public IActionResult Edit(long id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Update(user);
                _context.SaveChanges();
                return RedirectToAction("List");
            }
            return View(user);
        }

        public IActionResult Delete(long id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound();
            _context.Users.Remove(user);
            _context.SaveChanges();
            return RedirectToAction("List");
        }
    }
}
