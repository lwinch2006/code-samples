using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLogic.Logic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Web.ViewModels.Tenants;

namespace Web.Controllers
{
    public class TenantsController : Controller
    {
        private readonly ITenantsLogic _tenantsLogic;
        private readonly IMapper _mapper;
        private readonly ILogger<TenantsController> _logger;

        public TenantsController(ITenantsLogic tenantsLogic, IMapper mapper, ILogger<TenantsController> logger)
        {
            _tenantsLogic = tenantsLogic;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var tenants = await _tenantsLogic.Get();
            var viewModel = new TenantsVm
            {
                Tenants = _mapper.Map<IEnumerable<TenantVm>>(tenants)
            };

            return View(viewModel);
        }
    }
}