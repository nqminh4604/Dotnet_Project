using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Dotnet_Project.Models;
using Dotnet_Project.Data;

namespace Dotnet_Project.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly DotnetProjectDbContext _context;

    public HomeController(ILogger<HomeController> logger, DotnetProjectDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger;
    }

    public IActionResult Index()
    {
        //var courses = _context.Courses.ToList();
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
