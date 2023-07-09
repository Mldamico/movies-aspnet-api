using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Movies.Data;
using Movies.DTOs;
using Movies.Entities;

namespace Movies.Controllers;

[ApiController]
[Route("api/genres")]
public class GenresController : CustomBaseController
{
  

    public GenresController(ApplicationDbContext context, IMapper mapper) : base(context,mapper)
    {
    }

    [HttpGet]
    public async Task<ActionResult<List<GenreDto>>> GetGenres()
    {
        return await Get<Genre, GenreDto>();
    }

    [HttpGet("{id:int}", Name = "GetGenre")]
    public async Task<ActionResult<GenreDto>> GetGenreById(int id)
    {
        return await GetById<Genre, GenreDto>(id);
    }

    [HttpPost]
    public async Task<ActionResult> CreateGenre(GenreCreateDto genre)
    {
        return await Post<GenreCreateDto, Genre, GenreDto>(genre, "getGenre");
    }


    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateGenre(int id, GenreCreateDto genreDto)
    {
        return await Put<GenreCreateDto, Genre>(id, genreDto);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteGenre(int id)
    {
        return await Delete<Genre>(id);
    }
}