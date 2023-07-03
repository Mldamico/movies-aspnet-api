using System.ComponentModel.DataAnnotations;

namespace Movies.DTOs;

public class ActorCreateDto
{
    [Required]
    [StringLength(120)]
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
}