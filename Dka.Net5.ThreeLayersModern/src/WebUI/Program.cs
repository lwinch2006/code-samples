using ApplicationBuildingBlocks.App;
using ApplicationBuildingBlocks.Builder;

WebApplication
    .CreateBuilder(args)
    .ConfigureBuilder()
    .CreateApp()
    .ConfigureWebApp()
    .Run();