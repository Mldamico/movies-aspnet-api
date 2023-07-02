using System.ComponentModel.DataAnnotations;

namespace Movies.DTOs;

public class GenreCreateDto
{
    [Required]
    [StringLength(40)]
    public string Name { get; set; }
}