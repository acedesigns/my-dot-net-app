using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 36)) // Update to your MySQL version
    )
);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/", () => "My API is running on macOS 🚀");

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");


// --- Posts API endpoints

// Get all posts
app.MapGet("/api/post", async (AppDbContext db) =>
{
    var posts = await db.Posts.OrderByDescending(p => p.CreatedAt).ToListAsync();
    return Results.Ok(posts);
});

// Get single post
app.MapGet("/api/post/{id}", async (int id, AppDbContext db) =>
{
    var post = await db.Posts.FindAsync(id);
    return post is not null ? Results.Ok(post) : Results.NotFound();
});

// Add a new post
app.MapPost("/api/post", async (Post post, AppDbContext db) =>
{
    db.Posts.Add(post);
    await db.SaveChangesAsync();
    return Results.Created($"/api/post/{post.Id}", post);
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
