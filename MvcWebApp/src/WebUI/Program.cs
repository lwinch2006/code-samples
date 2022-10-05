using ApplicationBuildingBlocks.App;
using ApplicationBuildingBlocks.Builder;
using MvcWebApp.WebUI;

WebApplication
    .CreateBuilder(args)
    .ConfigureBuilder()
    .CreateApp()
    .ConfigureWebApp()
    .Run();