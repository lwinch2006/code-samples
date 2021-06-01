using System;
using dka.net5.codesamples.web.Models.TestingRoutes;
using Microsoft.AspNetCore.Mvc;

namespace dka.net5.codesamples.web.Controllers
{
    public class TestingRoutes : Controller
    {
        public IActionResult Index(int testingRouteId)
        {
            var test = Url.RouteUrl("test", new {controller = "TestingRoutes", action = "Index", testingRouteId = 111});
            return View();
        }

        [HttpGet]
        public IActionResult Update(Guid id)
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Update(Guid id, TestingRoutesUpdateViewModel viewModel)
        {
            return RedirectToAction(nameof(Index));
        }
    }
}