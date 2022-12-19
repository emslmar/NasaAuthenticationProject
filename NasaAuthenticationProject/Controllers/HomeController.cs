using Microsoft.AspNetCore.Mvc;
using NasaAuthenticationProject.Models;
using System.Diagnostics;

namespace NasaAuthenticationProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var cookie = Request.Cookies["LoggedIn"];
            if (cookie != null && cookie.Equals("True"))
            {
                ViewData["LoggedIn"] = "True";
                return View();
            }
            ViewData["LoggedIn"] = "False";
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
}