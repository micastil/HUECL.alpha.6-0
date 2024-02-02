using HUECL.alpha._6_0.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HUECL.alpha._6_0.Controllers
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
            return View();
        }


    }
}