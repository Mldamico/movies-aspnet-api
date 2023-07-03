using System.ComponentModel.DataAnnotations;
using Movies.Validations;

namespace Movies.DTOs;

public class ActorCreateDto
{
    [Required]
    [StringLength(120)]
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    [WeightFileValidation(maxFileWeightOnMegabyte: 4)]
    [FileTypeValidation(fileGroupType: FileGroupType.Imagen)]
    public IFormFile Photo { get; set; }
}