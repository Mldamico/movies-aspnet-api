using System.ComponentModel.DataAnnotations;

namespace Movies.DTOs;

public class MovieDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool Showcasing { get; set; }
    public DateTime DatePremiere { get; set; }
    public string Poster { get; set; }
}