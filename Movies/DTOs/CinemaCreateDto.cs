using System.ComponentModel.DataAnnotations;

namespace Movies.DTOs;

public class CinemaCreateDto
{
    [Required]
    [StringLength(120)]
    public string Name { get; set; }
}