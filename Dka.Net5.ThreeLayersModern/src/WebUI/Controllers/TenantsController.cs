using System.Threading.Tasks;
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
    }
}