using System.ComponentModel.DataAnnotations;

namespace Movies.DTOs;

public class MoviePatchDto
{
 
    [Required]
    [StringLength(300)]
    public string Title { get; set; }
    public bool Showcasing { get; set; }
    public DateTime DatePremiere { get; set; }
}