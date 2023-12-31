using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace Movies.Entities;

public class Cinema : IId
{
    public int Id { get; set; }
    [Required]
    [StringLength(120)]
    public string Name { get; set; }

    public Point Address { get; set; }
    public List<MoviesCinemas> MoviesCinemas { get; set; }
   
    
}