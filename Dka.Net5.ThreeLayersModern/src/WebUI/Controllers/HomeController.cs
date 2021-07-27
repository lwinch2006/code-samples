using System;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult GetError()
        {
            throw new Exception("Generated exception");
        }

        public IActionResult GetNotFound()
        {
            return NotFound();
        }
    }
}