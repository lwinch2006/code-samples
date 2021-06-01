using System;
using dka.net5.codesamples.web.Models.PartialViews;
using dka.net5.codesamples.web.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace dka.net5.codesamples.web.Controllers
{
    public class PartialViewsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Update(Guid id)
        {
            var vm = new PartialViewsUpdateViewModel
            {
                FirstName = new Partial1UpdateViewModel { Id = id },
                LastName = new Partial2UpdateViewModel { Id = id }
            };
            
            return View(vm);
        }

        [HttpPost]
        public IActionResult UpdateFirstName(Guid id, PartialViewsUpdateViewModel vm)
        {
            if (id != vm.FirstName.Id)
            {
                return BadRequest();
            }

            ModelState.Clear();
            this.ValidateModel(vm.FirstName, "FirstName.");
            
            if (!ModelState.IsValid)
            {
                return View("Update", vm);
            }
            
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateLastName(Guid id, PartialViewsUpdateViewModel vm)
        {
            if (id != vm.LastName.Id)
            {
                return BadRequest();
            }

            ModelState.Clear();
            this.ValidateModel(vm.LastName, "LastName.");
            
            if (!ModelState.IsValid)
            {
                return View("Update", vm);
            }

            return RedirectToAction("Index");
        }
    }
}