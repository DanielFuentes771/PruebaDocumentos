using LoginPrueba.Filters;
using LoginPrueba.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LoginPrueba.Controllers
{

    [ValidarSession]

    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(string token)
        {
            

            ViewBag.TokenJWT = HttpContext.Session.GetString("TokenJWT");
            return View();
        }
        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Login"); 
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
