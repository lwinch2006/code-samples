using ApplicationBuildingBlocks.App;
using ApplicationBuildingBlocks.Builder;
using WebApp;

WebApplication
	.CreateBuilder(args)
	.ConfigureBuilder()
	.CreateApp()
	.ConfigureWebApp()
	.Run();