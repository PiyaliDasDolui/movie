using Microsoft.EntityFrameworkCore;
using MovieApi.Models;
using MovieApi.Repositories;
using MovieApi.Services;
using AutoMapper;
using MovieApi.Mapping;
using Azure.AI.OpenAI;
using Azure;
using System.ClientModel;

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
// builder.Services.AddCors(options => {
//     options.AddPolicy("AllowReactApp",
//         policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
        
// });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // <-- Replace with your React URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Required for SignalR
    });
});

builder.Services.AddSignalR();

// Register the AzureOpenAIClient
builder.Services.AddSingleton(sp => 
{
    var endpoint = builder.Configuration["AzureOpenAI:Endpoint"]!;
    var key = builder.Configuration["AzureOpenAI:Key"]!;

    // Note: ApiKeyCredential works here even without the explicit 'using' 
    // if ImplicitUsings is enabled.
    return new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(key));
});

// 2. Register the specific ChatClient (This is what your MovieChatHub uses)
builder.Services.AddSingleton(sp => 
{
    var azureClient = sp.GetRequiredService<AzureOpenAIClient>();
    var deployment = builder.Configuration["AzureOpenAI:DeploymentName"];
    return azureClient.GetChatClient(deployment!);
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

    var seedMovies = new List<Movie>
    {
        new Movie { Title = "Inception", ReleaseDate = "2010", PosterPath = "https://image.tmdb.org/t/p/w500/oYuLcIPwsj9S79IuGfxY7Q7mBvW.jpg", Overview = "A thief who steals corporate secrets..." },
        new Movie { Title = "The Matrix", ReleaseDate = "1999", PosterPath = "https://image.tmdb.org/t/p/w500/f89U3Y9L7dbptkS2gnpSuaM3p3t.jpg", Overview = "A computer hacker learns about the true nature of reality..." },
        new Movie { Title = "The Shawshank Redemption", ReleaseDate = "1994", PosterPath = "https://image.tmdb.org/t/p/w500/q6y0Go1tsGEsmtFryDOJo3dEmqu.jpg", Overview = "Two imprisoned men bond over a number of years..." },
        new Movie { Title = "The Godfather", ReleaseDate = "1972", PosterPath = "https://image.tmdb.org/t/p/w500/3bhkrj58Vtu7enYsRolD1fZdja1.jpg", Overview = "The aging patriarch of an organized crime dynasty transfers control..." },
        new Movie { Title = "The Dark Knight", ReleaseDate = "2008", PosterPath = "https://image.tmdb.org/t/p/w500/qJ2tW6WMUDux911r6m7haRef0WH.jpg", Overview = "When the menace known as the Joker wreaks havoc..." },
        new Movie { Title = "Pulp Fiction", ReleaseDate = "1994", PosterPath = "https://image.tmdb.org/t/p/w500/dM2w364MScsjFf8pfMbaWUcWrR.jpg", Overview = "The lives of two mob hitmen, a boxer, and a gangster's wife..." },
        new Movie { Title = "Fight Club", ReleaseDate = "1999", PosterPath = "https://image.tmdb.org/t/p/w500/bptfVGEQuv6vDTIMVCHjJ9Dz8PX.jpg", Overview = "An insomniac office worker and a devil-may-care soap maker form an underground fight club..." },
        new Movie { Title = "Forrest Gump", ReleaseDate = "1994", PosterPath = "https://image.tmdb.org/t/p/w500/saHP97rTPS5eLmrLQEcANmKrsFl.jpg", Overview = "The presidencies of Kennedy and Johnson... through the eyes of an Alabama man..." },
        new Movie { Title = "Interstellar", ReleaseDate = "2014", PosterPath = "https://image.tmdb.org/t/p/w500/rAiYTfKGqDCRIIqo664sY9XZIvQ.jpg", Overview = "A team of explorers travel through a wormhole in space..." },
        new Movie { Title = "The Lord of the Rings: The Fellowship of the Ring", ReleaseDate = "2001", PosterPath = "https://image.tmdb.org/t/p/w500/6oom5QYQ2yQTMJIbnvbkBL9cHo6.jpg", Overview = "A meek Hobbit from the Shire and eight companions set out on a journey..." },
        new Movie { Title = "Gladiator", ReleaseDate = "2000", PosterPath = "https://image.tmdb.org/t/p/w500/ty8TGRuvJLPUmAR1H1nRIsgwvim.jpg", Overview = "A former Roman General sets out to exact vengeance..." }
    };

    foreach (var seed in seedMovies)
    {
        if (!context.Movies.Any(m => m.Title == seed.Title))
        {
            context.Movies.Add(seed);
        }
    }

    if (context.ChangeTracker.HasChanges())
    {
        context.SaveChanges();
    }
}

app.MapHub<MovieChatHub>("/moviechathub"); // This must match your URL path
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
