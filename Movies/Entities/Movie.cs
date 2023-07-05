using System.ComponentModel.DataAnnotations;

namespace Movies.Entities;

public class Movie
{
    public int Id { get; set; }
    [Required]
    [StringLength(300)]
    public string Title { get; set; }
    public bool Showcasing { get; set; }
    public DateTime DatePremiere { get; set; }
    public string Poster { get; set; }
    
}