using Microsoft.EntityFrameworkCore;
using MovieApi.Models;
using MovieApi.Repositories;
using MovieApi.Services;
using AutoMapper;
using MovieApi.Mapping;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddDbContext<MovieContext>(options =>
    options.UseSqlite("Data Source=movies.db"));

// Register repository, service and AutoMapper
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
// 1. Add CORS Policy
builder.Services.AddCors(options => {
    options.AddPolicy("AllowReactApp",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});
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

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
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





// 2. Use CORS Policy
app.UseCors("AllowReactApp");

app.MapControllers();
// Optional: Add a root message so you know it's working
app.MapGet("/", () => "API is running! Try /api/movies");
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MovieContext>();
    context.Database.EnsureCreated(); // Double check DB exists

    if (!context.Movies.Any())
    {
        context.Movies.AddRange(
            new Movie { Title = "Inception", ReleaseDate = "2010", PosterPath = "https://image.tmdb.org/t/p/w500/oYuLcIPwsj9S79IuGfxY7Q7mBvW.jpg", Overview = "A thief who steals corporate secrets..." },
            new Movie { Title = "The Matrix", ReleaseDate = "1999", PosterPath = "https://image.tmdb.org/t/p/w500/f89U3Y9L7dbptkS2gnpSuaM3p3t.jpg", Overview = "A computer hacker learns about the true nature of reality..." }
        );
        context.SaveChanges();
    }
}
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
