using System.ComponentModel.DataAnnotations;

namespace Movies.Entities;

public class Movie: IId
{
    public int Id { get; set; }
    [Required]
    [StringLength(300)]
    public string Title { get; set; }
    public bool Showcasing { get; set; }
    public DateTime DatePremiere { get; set; }
    public string Poster { get; set; }

    public List<MoviesActors> MoviesActors { get; set; }
    public List<MoviesGenres> MoviesGenres { get; set; }
    
}