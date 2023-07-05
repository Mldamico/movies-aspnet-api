using System.ComponentModel.DataAnnotations;
using Movies.Validations;

namespace Movies.DTOs;

public class MovieCreateDto : MoviePatchDto
{
   
    [WeightFileValidation(4)]
    [FileTypeValidation(FileGroupType.Imagen)]
    public IFormFile Poster { get; set; }

    public List<int> GenresIDs { get; set; }
}