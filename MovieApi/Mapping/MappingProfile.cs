using AutoMapper;
using MovieApi.DTOs;
using MovieApi.Models;

namespace MovieApi.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Movie, MovieDto>().ReverseMap();
        CreateMap<MovieCreateDto, Movie>();
    }
}
