using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LeMarconnes.Controllers 
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Dit opent de welkomstpagina (https://localhost:xxxx/)
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Title"] = "Contact";
            return View();
        }

        // Optioneel: Een privacy pagina (standaard in ASP.NET projecten)
        public IActionResult Privacy()
        {
            return View();
        }

        // Foutafhandeling
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}