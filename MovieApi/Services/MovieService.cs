using AutoMapper;
using MovieApi.DTOs;
using MovieApi.Models;
using MovieApi.Repositories;

namespace MovieApi.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _repo;
    private readonly IMapper _mapper;

    public MovieService(IMovieRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MovieDto>> GetAllAsync()
    {
        var movies = await _repo.GetAllAsync();
        return _mapper.Map<IEnumerable<MovieDto>>(movies);
    }

    public async Task<MovieDto?> GetByIdAsync(int id)
    {
        var movie = await _repo.GetByIdAsync(id);
        return movie is null ? null : _mapper.Map<MovieDto>(movie);
    }

    public async Task<MovieDto> CreateAsync(MovieCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new ArgumentException("Title is required");

        var movie = _mapper.Map<Movie>(dto);
        var created = await _repo.AddAsync(movie);
        return _mapper.Map<MovieDto>(created);
    }

    public async Task<bool> UpdateAsync(int id, MovieCreateDto dto)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing is null) return false;

        // simple business rule: title cannot be empty
        if (string.IsNullOrWhiteSpace(dto.Title)) return false;

        existing.Title = dto.Title;
        existing.PosterPath = dto.PosterPath;
        existing.ReleaseDate = dto.ReleaseDate;
        existing.Overview = dto.Overview;

        await _repo.UpdateAsync(existing);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing is null) return false;
        await _repo.DeleteAsync(id);
        return true;
    }
}
