using MovieApi.DTOs;

namespace MovieApi.Services;

public interface IMovieService
{
    Task<IEnumerable<MovieDto>> GetAllAsync();
    Task<MovieDto?> GetByIdAsync(int id);
    Task<MovieDto> CreateAsync(MovieCreateDto dto);
    Task<bool> UpdateAsync(int id, MovieCreateDto dto);
    Task<bool> DeleteAsync(int id);
}
