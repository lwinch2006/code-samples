using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ThreeLayersModernApi.Tenants.Responses;

namespace ThreeLayersModernApi.Tenants;

public class GetTenants
{
    private readonly ILogger<GetTenants> _logger;
    
    public GetTenants(ILogger<GetTenants> logger)
    {
        _logger = logger;
    }
    
    [FunctionName("GetTenants")]
    public IActionResult RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "tenants")] HttpRequest req)
    {
        _logger.LogWarning("Incoming request to get tenants");

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