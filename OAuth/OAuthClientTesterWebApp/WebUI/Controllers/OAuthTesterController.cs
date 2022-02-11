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
        model.OAuthClientConfiguration = _optionsMonitor.GetEx(model.ConfigurationName);
        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
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
                TempData[TempDataNames.OAuthTesterConfigurationName] = oAuthTesterViewModel.ConfigurationName;
                return Redirect(oAuthRedirect.Uri);                

            case AccessTokenResponse accessTokenResponse:
                oAuthTesterViewModel.AccessTokenResponse = Utils.Mappers.OAuthMapper.Map(accessTokenResponse);
                break;
                
            case DeviceCodeResponse deviceCodeResponse:
                TempData[TempDataNames.OAuthTesterConfigurationName] = oAuthTesterViewModel.ConfigurationName;
                TempData.Write(TempDataNames.DeviceCodeResponse, deviceCodeResponse);
                return RedirectToAction(nameof(PingDeviceToken));
            
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

        var configurationName = (string)TempData.ReadAndClear(TempDataNames.OAuthTesterConfigurationName);
        var originalState = (string)TempData.ReadAndClear(Common.State);
        var codeVerifier = (string)TempData.ReadAndClear(Common.CodeVerifier);

        var oAuthTesterViewModel = Utils.Mappers.OAuthMapper.GetNewOAuthTesterViewModel(configurationName);
        oAuthTesterViewModel.OAuthClientConfiguration = _optionsMonitor.GetEx(configurationName);
        var oAuthFlows = _oAuthFlowsFactory.CreateOAuthFlows(configurationName);

        var authorizationCodeCallbackResponse = Utils.Mappers.OAuthMapper.Map(authorizationCodeResponseViewModel);
        var implicitFlowCallbackResponse = Utils.Mappers.OAuthMapper.Map(implicitFlowResponseViewModel);
        
        var response = await oAuthFlows.RunFlow(
            oAuthTesterViewModel.OAuthClientConfiguration, 
            authorizationCodeCallbackResponse, 
            implicitFlowCallbackResponse, 
            originalState: originalState, 
            codeVerifier: codeVerifier);

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

    public IActionResult PingDeviceToken()
    {
        var deviceCodeResponse = TempData.ReadAndClean<DeviceCodeResponse>(TempDataNames.DeviceCodeResponse);
        var viewModel = Utils.Mappers.OAuthMapper.Map(deviceCodeResponse);
        TempData.Write(TempDataNames.DeviceCodeResponse, deviceCodeResponse);
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> PingDeviceToken(DeviceCodeResponseViewModel viewModel)
    {
        var configurationName = (string)TempData.ReadAndClear(TempDataNames.OAuthTesterConfigurationName);
        var deviceCodeResponse = TempData.ReadAndClean<DeviceCodeResponse>(TempDataNames.DeviceCodeResponse);
        var oAuthTesterViewModel = Utils.Mappers.OAuthMapper.GetNewOAuthTesterViewModel(configurationName);
        oAuthTesterViewModel.OAuthClientConfiguration = _optionsMonitor.GetEx(configurationName);

        var oAuthFlows = _oAuthFlowsFactory.CreateOAuthFlows(configurationName);

        var response = await oAuthFlows.RunFlow(oAuthTesterViewModel.OAuthClientConfiguration, deviceCodeResponse: deviceCodeResponse);

        switch (response)
        {
            case AccessTokenResponse accessTokenResponse:
                oAuthTesterViewModel.AccessTokenResponse = Utils.Mappers.OAuthMapper.Map(accessTokenResponse);
                return View("Index", oAuthTesterViewModel);
            
            case ErrorResponse {Error: DeviceTokenResponseErrors.Slowdown}:
                deviceCodeResponse.Interval += 1;
                break;
        }
        
        TempData[TempDataNames.OAuthTesterConfigurationName] = configurationName;
        TempData.Write(TempDataNames.DeviceCodeResponse, deviceCodeResponse);
        return View(viewModel);
    }

    private IActionResult ProcessOAuthClientErrorResponse(ErrorResponse errorResponse)
    {
        return Json(errorResponse);
    }    
}