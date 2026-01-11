namespace MovieApi.DTOs;

public class MovieDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string PosterPath { get; set; } = string.Empty;
    public string ReleaseDate { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
}
