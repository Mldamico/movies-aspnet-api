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
        CreateMap<Actor, ActorDto>().ReverseMap();
        CreateMap<ActorCreateDto, Actor>().ForMember(x => x.Photo, options => options.Ignore());
        CreateMap<ActorPatchDto, Actor>().ReverseMap();
        CreateMap<Movie, MovieDto>().ReverseMap();
        CreateMap<MovieCreateDto, Movie>().ForMember(x => x.Poster, opt => opt.Ignore());
        CreateMap<MoviePatchDto, Movie>().ReverseMap();
    }
}