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
			.AddGitLab("GitLab", g =>
			{
                g.SignInScheme = "Cookie";

                g.ClientId = configuration["OAuth2.0:GitLab:ClientId"];
                g.ClientSecret = configuration["OAuth2.0:GitLab:ClientSecret"];
                g.CallbackPath = "/signin-gitlab";
                g.SaveTokens = true;
            })
			.AddYahoo("Yahoo", y =>
			{
                y.SignInScheme = "Cookie";

                y.ClientId = configuration["OAuth2.0:Yahoo:ClientId"];
                y.ClientSecret = configuration["OAuth2.0:Yahoo:ClientSecret"];
                y.CallbackPath = "/";
                y.SaveTokens = true;
            })         // Worked
			.AddGoogle("Google", g =>
			{
                g.SignInScheme = "Cookie";
				
                g.ClientId = configuration["OAuth2.0:Google:ClientId"];
                g.ClientSecret = configuration["OAuth2.0:Google:ClientSecret"];
                g.CallbackPath = "/signin-google";
                g.SaveTokens = true;
            })       // Worked
			.AddLinkedIn("LinkedIn", l =>
			{
				l.SignInScheme = "Cookie";

				l.ClientId = configuration["OAuth2.0:LinkedIn:ClientId"];
				l.ClientSecret = configuration["OAuth2.0:LinkedIn:ClientSecret"];
				l.CallbackPath = "/any";
				l.SaveTokens = true;

                l.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                l.ClaimActions.MapJsonKey("OpenId", "openid");
                l.ClaimActions.MapJsonKey("Profile", "profile");

                l.Events.OnCreatingTicket = async ctx =>
                {
                    using var request = new HttpRequestMessage(HttpMethod.Get, ctx.Options.UserInformationEndpoint);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ctx.AccessToken);
                    using var result = await ctx.Backchannel.SendAsync(request);
                    var user = await result.Content.ReadFromJsonAsync<JsonElement>();
                    ctx.RunClaimActions(user);
                };
            })
			.AddYandex("Yandex", y =>
			{
				y.SignInScheme = "Cookie";
				
				y.ClientId = configuration["OAuth2.0:Yandex:ClientId"];
				y.ClientSecret = configuration["OAuth2.0:Yandex:ClientSecret"];
				y.CallbackPath = "/";
				y.SaveTokens = true;

                y.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                y.ClaimActions.MapJsonKey(ClaimTypes.Name, "login");

                y.Events.OnCreatingTicket = async ctx =>
                {
                    using var request = new HttpRequestMessage(HttpMethod.Get, ctx.Options.UserInformationEndpoint);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ctx.AccessToken);
                    using var result = await ctx.Backchannel.SendAsync(request);
                    var user = await result.Content.ReadFromJsonAsync<JsonElement>();
                    ctx.RunClaimActions(user);
                };
            })       // Worked
			.AddTwitter("X", t =>
			{
				t.SignInScheme = "Cookie";

				t.ConsumerKey = configuration["OAuth2.0:X:ClientId"];
				t.ConsumerSecret = configuration["OAuth2.0:X:ClientSecret"];
				t.CallbackPath = "/any";
				t.SaveTokens = true;
            })           // Worked
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
            });       // Worked

		return services;
	}
}

