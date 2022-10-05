using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcWebApp.WebUI.ViewModels.Error;

namespace WebUI.Controllers
{
    public class ErrorController : WebUiControllerBase
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }
        
        [Route("[controller]/{id:int}")]
        public IActionResult Index(int id)
        {
            var vm = new ErrorVm
            {
                StatusCode = id,
                RequestId = HttpContext.TraceIdentifier,
                TraceId = Activity.Current?.Id ?? string.Empty
            };
            
            var statusCodeReExecuteFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            if (statusCodeReExecuteFeature != null)
            {
                vm.Path =
                    statusCodeReExecuteFeature.OriginalPathBase
                    + statusCodeReExecuteFeature.OriginalPath
                    + statusCodeReExecuteFeature.OriginalQueryString;
            }
            
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            Exception error = null;

            if (exceptionHandlerPathFeature != null)
            {
                vm.Path = exceptionHandlerPathFeature.Path;
                error = exceptionHandlerPathFeature.Error;
            }
            
            using (_logger.BeginScope(new Dictionary<string, object> {{"StatusCode", id}}))
            {
                _logger.LogError(error, "System error");
            }
            
            return View(vm);
        }
    }
}