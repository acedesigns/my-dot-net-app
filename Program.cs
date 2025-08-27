using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --- Services ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 36)) // Update to your MySQL version
    )
);

// OpenAPI / Swagger
builder.Services.AddOpenApi();

// Blazor
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

// --- Middleware ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// --- Blazor Endpoints ---
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// --- API Endpoints ---
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Weather Forecast (demo)
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild",
    "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/api/weatherforecast", () =>
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

// Posts API
app.MapGet("/api/post", async (AppDbContext db) =>
{
    var posts = await db.Posts.OrderByDescending(p => p.CreatedAt).ToListAsync();
    return Results.Ok(posts);
});

app.MapGet("/api/post/{id}", async (int id, AppDbContext db) =>
{
    var post = await db.Posts.FindAsync(id);
    return post is not null ? Results.Ok(post) : Results.NotFound();
});

app.MapPost("/api/post", async (Post post, AppDbContext db) =>
{
    post.CreatedAt = DateTime.UtcNow; // ensure timestamp
    db.Posts.Add(post);
    await db.SaveChangesAsync();
    return Results.Created($"/api/post/{post.Id}", post);
});

app.Run();

// --- Record Types ---
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}