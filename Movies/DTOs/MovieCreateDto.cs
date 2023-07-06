using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Movies.Helpers;
using Movies.Validations;
using Newtonsoft.Json;

namespace Movies.DTOs;

public class MovieCreateDto : MoviePatchDto
{
   
    [WeightFileValidation(4)]
    [FileTypeValidation(FileGroupType.Imagen)]
    public IFormFile Poster { get; set; }

 
    [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
    public List<int> GenresIDs { get; set; }
    
  
    [ModelBinder(BinderType = typeof(TypeBinder<List<ActorMovieCreateDto>>))]
    public List<ActorMovieCreateDto>Actors { get; set; }
    
    
}