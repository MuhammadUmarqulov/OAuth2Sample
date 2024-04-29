using Microsoft.AspNetCore.Authentication;
using OAuth2Samples;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthenticationWithOAuth2Config(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapGet("/", (HttpContext ctx) => { return ctx.User.Claims.Select(x => new { x.Type, x.Value }).ToList();});

app.MapGet("/info", (HttpContext ctx) => { return ctx.User.Claims.Select(x => new { x.Type, x.Value }).ToList();});

app.MapGet("/git-hub", () => { return Results.Challenge( new AuthenticationProperties() { RedirectUri = "https://localhost:7114/" }, [ "GitHub" ]); });

app.MapGet("/twitter", () => { return Results.Challenge( new AuthenticationProperties() { RedirectUri = "https://localhost:7114/" }, [ "X" ]); });

app.MapGet("/linkedin", () => { return Results.Challenge( new AuthenticationProperties() { RedirectUri = "https://localhost:7114/info" }, [ "LinkedIn" ]);});

app.MapGet("/google", () => { return Results.Challenge( new AuthenticationProperties() { RedirectUri = "https://localhost:7114" }, [ "Google" ]);});

app.MapGet("/yandex", () => { return Results.Challenge( new AuthenticationProperties() { RedirectUri = "https://localhost:7114/info" }, [ "Yandex" ]);});

app.MapGet("/gitlab", () => { return Results.Challenge( new AuthenticationProperties() { RedirectUri = "https://localhost:7114/info" }, [ "GitLab" ]);});

app.MapGet("/yahoo", () => { return Results.Challenge( new AuthenticationProperties() { RedirectUri = "https://localhost:7114/" }, [ "Yahoo" ]);});

app.MapGet("/amazon", () => { return Results.Challenge( new AuthenticationProperties() { RedirectUri = "https://localhost:7114/info" }, [ "Amazon" ]);});

app.Run();

