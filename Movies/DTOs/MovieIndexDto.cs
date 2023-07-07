namespace Movies.DTOs;

public class MovieIndexDto
{
    public List<MovieDto> NextRelease { get; set; }
    public List<MovieDto> Showcasing { get; set; }
}