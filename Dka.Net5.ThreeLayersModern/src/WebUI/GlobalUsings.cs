global using System;
global using System.Linq;

global using Azure.Extensions.AspNetCore.Configuration.Secrets;
global using Azure.Identity;
global using Azure.Security.KeyVault.Secrets;

global using Microsoft.ApplicationInsights.Extensibility;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authentication.Cookies;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;

global using FluentValidation.AspNetCore;

global using Serilog;
global using Serilog.Exceptions;

global using Application.Extensions;
global using Application.Logic.Tenants.Commands;
global using Dka.Net5.IdentityWithDapper.Utils.Extensions;
global using WebUI;
global using WebUI.Mapping;
global using WebUI.Utils.Extensions;
global using WebUI.Middleware;
