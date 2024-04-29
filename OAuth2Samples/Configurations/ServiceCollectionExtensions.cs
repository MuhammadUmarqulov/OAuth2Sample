using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

namespace OAuth2Samples;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddAuthenticationWithOAuth2Config(
		this IServiceCollection services, IConfiguration configuration)
	{
		services.AddAuthentication("Cookie")
			.AddCookie("Cookie")
			.AddTwitter("X", t =>
			{
				t.SignInScheme = "Cookie";

				t.ConsumerKey = configuration["OAuth2.0:X:ClientId"];
				t.ConsumerSecret = configuration["OAuth2.0:X:ClientSecret"];
				t.CallbackPath = "/any";
				t.SaveTokens = true;
            })
			.AddOAuth("GitHub", g =>
			{
				g.SignInScheme = "Cookie";

				g.ClientId = configuration["OAuth2.0:GitHub:ClientId"];
				g.ClientSecret = configuration["OAuth2.0:GitHub:ClientSecret"];

				g.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
				g.TokenEndpoint = "https://github.com/login/oauth/access_token";
				g.CallbackPath = "/any";
				g.SaveTokens = true;
				g.UserInformationEndpoint = "https://api.github.com/user";

				g.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
				g.ClaimActions.MapJsonKey(ClaimTypes.Name, "login");

                g.Events.OnCreatingTicket = async ctx =>
				{
					using var request = new HttpRequestMessage(HttpMethod.Get, ctx.Options.UserInformationEndpoint);
					request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ctx.AccessToken);
					using var result = await ctx.Backchannel.SendAsync(request);
					var user = await result.Content.ReadFromJsonAsync<JsonElement>();
					ctx.RunClaimActions(user);
				};
            });
		return services;
	}
}

