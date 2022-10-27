using Application.Logic.Tenants.Queries;
using Microsoft.AspNetCore.Mvc;
using WebApp.ViewModels.Tenants;

namespace WebApp.Controllers
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
			var vm = await Mediator.Send(new GetTenantQuery { Id = id });
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
			var tenantVm = await Mediator.Send(new GetTenantQuery { Id = id });
			if (tenantVm == null) return NotFound();
			//var command = Mapper.Map<UpdateTenantCommand>(tenantVm);
			var vm = new UpdateTenantVm
			{
				Id = tenantVm.Id,
				Name = tenantVm.Name
			};

			return View(vm);
		}

		// [HttpPost]
		// [ValidateAntiForgeryToken]
		// public async Task<IActionResult> Update(Guid id, UpdateTenantCommand command)
		// {
		//     if (id != command.Id) return BadRequest();
		//     if (!ModelState.IsValid) return View(command);
		//     await Mediator.Send(command);
		//     return RedirectToAction(nameof(Index));
		// }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Update(Guid id, UpdateTenantVm updateTenantVm)
		{
			if (id != updateTenantVm.Id) return BadRequest();
			if (!ModelState.IsValid) return View(updateTenantVm);

			var command = new UpdateTenantCommand
			{
				Id = updateTenantVm.Id,
				Name = updateTenantVm.Name
			};

			await Mediator.Send(command);
			return RedirectToAction(nameof(Index));
		}


		public IActionResult Delete(Guid id)
		{
			return View(new DeleteTenantCommand { Id = id });
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