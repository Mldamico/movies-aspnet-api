using AutoMapper;
using Movies.DTOs;
using Movies.Entities;

namespace Movies.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<Genre, GenreDto>().ReverseMap();
        CreateMap<GenreCreateDto, Genre>();
    }
}