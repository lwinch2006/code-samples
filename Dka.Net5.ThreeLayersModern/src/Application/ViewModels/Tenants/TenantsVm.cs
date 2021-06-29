using System.Collections.Generic;

namespace Application.ViewModels.Tenants
{
    public class TenantsVm
    {
        public IEnumerable<TenantVm> Tenants { get; set; }
    }
}