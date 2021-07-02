using System;
using System.Threading.Tasks;
using Application.Logic.Tenants.Commands;
using Application.Logic.Tenants.Queries;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers
{
    public class TenantsController : WebUiControllerBase
    {
        public async Task<IActionResult> Index()
        {
            var vm = await Mediator.Send(new GetTenantsQuery());
            return View(vm);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var vm = await Mediator.Send(new GetTenantQuery {Id = id});
            if (vm == null) return NotFound();
            return View(vm);
        }
        
        public IActionResult Create()
        {
            return View(new CreateTenantCommand());
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTenantCommand command)
        {
            if (!ModelState.IsValid) return View(command);
            await Mediator.Send(command);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(Guid id)
        {
            var tenantVm = await Mediator.Send(new GetTenantQuery {Id = id});
            if (tenantVm == null) return NotFound();
            var command = Mapper.Map<UpdateTenantCommand>(tenantVm);
            return View(command);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Guid id, UpdateTenantCommand command)
        {
            if (id != command.Id) return BadRequest();
            if (!ModelState.IsValid) return View(command);
            await Mediator.Send(command);
            return RedirectToAction(nameof(Index));
        }
        
        public IActionResult Delete(Guid id)
        {
            return View(new DeleteTenantCommand {Id = id});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, DeleteTenantCommand command)
        {
            if (id != command.Id) return BadRequest();
            if (!ModelState.IsValid) return View(command);
            await Mediator.Send(command);
            return RedirectToAction(nameof(Index));
        }
    }
}