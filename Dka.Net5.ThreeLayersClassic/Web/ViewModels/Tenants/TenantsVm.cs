using System.Collections.Generic;

namespace Web.ViewModels.Tenants
{
    public class TenantsVm
    {
        public IEnumerable<TenantVm> Tenants { get; set; }
    }
}