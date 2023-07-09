using System.ComponentModel.DataAnnotations;

namespace Movies.Entities;

public class Cinema : IId
{
    public int Id { get; set; }
    [Required]
    [StringLength(120)]
    public string Name { get; set; }

    public List<MoviesCinemas> MoviesCinemas { get; set; }
    
    
}