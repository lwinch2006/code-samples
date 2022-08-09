namespace ApplicationBuildingBlocks.Builder.Authentication;

public static class TwitterAuthenticationExtensions
{
	public static WebApplicationBuilder AddTwitterAuthenticationServices(this WebApplicationBuilder builder)
	{
		builder.Services.AddAuthentication()
			.AddTwitter(twitterOptions =>
		{
			twitterOptions.ConsumerKey = builder.Configuration["TwitterAuthentication:ClientId"];
			twitterOptions.ConsumerSecret = builder.Configuration["TwitterAuthentication:ClientSecret"];
			twitterOptions.RetrieveUserDetails = true;
		});
		
		return builder;
	}
}