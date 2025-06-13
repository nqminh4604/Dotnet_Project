using Microsoft.AspNetCore.Mvc;

namespace Dotnet_Project.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
