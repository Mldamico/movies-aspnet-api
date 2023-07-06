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
        CreateMap<MovieCreateDto, Movie>().ForMember(x => x.Poster, opt => opt.Ignore())
            .ForMember(x => x.MoviesGenres, opt => opt.MapFrom(MapMoviesGenres))
            .ForMember(x => x.MoviesActors, opt => opt.MapFrom(MapMoviesActors));
        CreateMap<MoviePatchDto, Movie>().ReverseMap();
    }

    private List<MoviesGenres> MapMoviesGenres(MovieCreateDto movieCreateDto, Movie movie)
    {
        var result = new List<MoviesGenres>();
        if (movieCreateDto.GenresIDs == null) return result;
        foreach (var id in movieCreateDto.GenresIDs)
        {
            result.Add(new MoviesGenres() { GenreId = id});
        }
        
        return result;
        
    }

    private List<MoviesActors> MapMoviesActors(MovieCreateDto movieCreateDto, Movie movie)
    {
        var result = new List<MoviesActors>();
        if (movieCreateDto.Actors == null) return result;
        foreach (var actor in movieCreateDto.Actors)
        {
            result.Add(new MoviesActors() {ActorId = actor.ActorId, Character = actor.Character});
        }

        return result;
    }
}