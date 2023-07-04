using System.ComponentModel.DataAnnotations;
using Movies.Validations;

namespace Movies.DTOs;

public class ActorCreateDto : ActorPatchDto
{
    [WeightFileValidation(maxFileWeightOnMegabyte: 4)]
    [FileTypeValidation(fileGroupType: FileGroupType.Imagen)]
    public IFormFile Photo { get; set; }
}