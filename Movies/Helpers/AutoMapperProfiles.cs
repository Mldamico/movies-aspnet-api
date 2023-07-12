using System.IO.Hashing;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Movies.DTOs;
using Movies.Entities;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace Movies.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles(GeometryFactory geometryFactory)
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
        CreateMap<Movie, MovieDetailsDto>().ForMember(x => x.Genres, opt =>
            opt.MapFrom(MapMoviesGenres))
            .ForMember(x => x.Actors, opt => opt.MapFrom(MapMoviesActors));
        CreateMap<Cinema, CinemaDto>()
            .ForMember(x => x.Latitude, x => x.MapFrom(y=> y.Address.Y))
            .ForMember(x => x.Longitude, x=> x.MapFrom(y=> y.Address.X));
        // var geomtryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        CreateMap<CinemaDto, Cinema>().ForMember(x => x.Address, x => x.MapFrom(y =>
            geometryFactory.CreatePoint(new Coordinate(y.Longitude, y.Latitude))));
        CreateMap<CinemaCreateDto, Cinema>().ForMember(x => x.Address, x => x.MapFrom(y =>
            geometryFactory.CreatePoint(new Coordinate(y.Longitude, y.Latitude))));;
        CreateMap<IdentityUser, UserDto>();
    }

    private List<ActorMovieDetailDto> MapMoviesActors(Movie movie, MovieDetailsDto movieDetailsDto)
    {
        var result = new List<ActorMovieDetailDto>();
        if (movie.MoviesActors == null) return result;
        foreach (var actor in movie.MoviesActors)
        {
            result.Add(new ActorMovieDetailDto(){ActorId = actor.ActorId, Character = actor.Character, NamePerson = actor.Actor.Name});
        }

        return result;
    }

    private List<GenreDto> MapMoviesGenres(Movie movie, MovieDetailsDto movieDetailsDto)
    {
        var result = new List<GenreDto>();
        if (movie.MoviesGenres == null) return result;
        foreach (var genre in movie.MoviesGenres)
        {
            result.Add(new GenreDto(){Id = genre.GenreId, Name = genre.Genre.Name});
        }

        return result;
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