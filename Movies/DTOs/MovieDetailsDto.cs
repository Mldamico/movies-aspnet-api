namespace Movies.DTOs;

public class MovieDetailsDto : MovieDto
{
    public List<GenreDto> Genres { get; set; }
    public List<ActorMovieDetailDto> Actors { get; set; }
    
}