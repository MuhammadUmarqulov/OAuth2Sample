using Microsoft.AspNetCore.Authentication;
using OAuth2Samples;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthenticationWithOAuth2Config(builder.Configuration);

builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapGet("/", (HttpContext ctx) =>
{
    return ctx.User.Claims.Select(x => new { x.Type, x.Value }).ToList();
}); 

app.MapGet("/git-hub", () => {
    return Results.Challenge(
        new AuthenticationProperties()
        {
            RedirectUri = "https://localhost:7114/"
        },
        new List<string> { "GitHub" });
});

app.MapGet("/twitter", () => {
    return Results.Challenge(
        new AuthenticationProperties()
        {
            RedirectUri = "https://localhost:7114/"
        },
        new List<string> { "X" });
});
app.Run();

