using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ThreeLayersModernApi.Tenants.Responses;

namespace ThreeLayersModernApi.Tenants;

public class GetTenants
{
    [FunctionName("GetTenants")]
    public IActionResult RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "tenants")] HttpRequest req, 
        ILogger log)
    {
        log.LogInformation("Incoming request to get tenants");

        var tenants = new[]
        {
            new Tenant
            {
                Id = Guid.NewGuid(),
                Name = "Sample tenant"
            }
        };

        return new OkObjectResult(tenants);
    }
}