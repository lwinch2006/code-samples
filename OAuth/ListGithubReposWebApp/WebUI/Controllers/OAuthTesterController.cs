using System.Net;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OAuthClient;
using OAuthClient.Models;
using OAuthClient.Models.Constants;
using OAuthClient.Models.Responses;
using OAuthClient.Utils;
using WebUI.Extensions;
using WebUI.Models;
using WebUI.ViewModels.OAuth;
using WebUI.ViewModels.OAuthTester;

namespace WebUI.Controllers;

public class OAuthTesterController : Controller
{
    private readonly IOptionsMonitor<OAuthClientConfiguration> _optionsMonitor;
    private readonly IOAuthFlowsFactory _oAuthFlowsFactory;

    public OAuthTesterController(IOptionsMonitor<OAuthClientConfiguration> optionsMonitor, IOAuthFlowsFactory oAuthFlowsFactory)
    {
        _optionsMonitor = optionsMonitor;
        _oAuthFlowsFactory = oAuthFlowsFactory;
    }
    
    public IActionResult Index(string configurationName)
    {
        var model = Utils.Mappers.OAuthMapper.GetNewOAuthTesterViewModel(configurationName);
        model.OAuthClientConfiguration = _optionsMonitor.Get(model.ConfigurationName.ToLower());
        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    //[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None, Duration = -1)]
    public async Task<IActionResult> Index(OAuthTesterViewModel oAuthTesterViewModel)
    {
        var state = StateUtils.Generate();  
        var oAuthClientConfiguration = oAuthTesterViewModel.OAuthClientConfiguration;
        var oAuthFlows = _oAuthFlowsFactory.CreateOAuthFlows(oAuthClientConfiguration.Name);
        var response = await oAuthFlows.RunFlow(oAuthClientConfiguration, state, oAuthTesterViewModel.Username, oAuthTesterViewModel.Password);
        
        switch (response)
        {
            case OAuthRedirect oAuthRedirect:
                TempData[Common.State] = oAuthRedirect.State;
                TempData[Common.CodeVerifier] = oAuthRedirect.CodeVerifier;
                TempData[WebUiConstants.OAuthTesterConfigurationName] = oAuthTesterViewModel.ConfigurationName;
                return Redirect(oAuthRedirect.Uri);                

            case AccessTokenResponse accessTokenResponse:
                oAuthTesterViewModel.AccessTokenResponse = Utils.Mappers.OAuthMapper.Map(accessTokenResponse);
                break;
            
            case ErrorResponse errorResponse:
                return ProcessOAuthClientErrorResponse(errorResponse);
        }
        
        return View(oAuthTesterViewModel);
    }

    public async Task<IActionResult> Callback(
        AuthorizationCodeResponseViewModel authorizationCodeResponseViewModel,
        ImplicitFlowResponseViewModel implicitFlowResponseViewModel,
        ErrorResponseViewModel errorResponseAtResponseViewModel)
    {
        if (!string.IsNullOrWhiteSpace(errorResponseAtResponseViewModel.Error))
        {
            var errorResponseAtCallback = Utils.Mappers.OAuthMapper.Map(errorResponseAtResponseViewModel);
            return ProcessOAuthClientErrorResponse(errorResponseAtCallback);
        }

        var configurationName = (string)TempData.ReadAndClear(WebUiConstants.OAuthTesterConfigurationName);
        var originalState = (string)TempData.ReadAndClear(Common.State);
        var codeVerifier = (string)TempData.ReadAndClear(Common.CodeVerifier);

        var oAuthTesterViewModel = Utils.Mappers.OAuthMapper.GetNewOAuthTesterViewModel(configurationName);
        oAuthTesterViewModel.OAuthClientConfiguration = _optionsMonitor.Get(configurationName);
        var oAuthFlows = _oAuthFlowsFactory.CreateOAuthFlows(configurationName);

        var authorizationCodeCallbackResponse = Utils.Mappers.OAuthMapper.Map(authorizationCodeResponseViewModel);
        var implicitFlowCallbackResponse = Utils.Mappers.OAuthMapper.Map(implicitFlowResponseViewModel);
        
        var response = await oAuthFlows.RunFlow(
            oAuthTesterViewModel.OAuthClientConfiguration, 
            authorizationCodeCallbackResponse, 
            implicitFlowCallbackResponse, 
            originalState, 
            codeVerifier);

        switch (response)
        {
            case AccessTokenResponse accessTokenResponse:
                oAuthTesterViewModel.AccessTokenResponse = Utils.Mappers.OAuthMapper.Map(accessTokenResponse);
                break;
            
            case ErrorResponse errorResponse:
                return ProcessOAuthClientErrorResponse(errorResponse);
        }
        
        return View("Index", oAuthTesterViewModel);
    }
    
    private IActionResult ProcessOAuthClientErrorResponse(ErrorResponse errorResponse)
    {
        return Json(errorResponse);
    }    
}