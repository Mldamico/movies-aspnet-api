using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.Data;
using Movies.DTOs;
using Movies.Entities;

namespace Movies.Controllers;

[ApiController]
[Route("api/genres")]
public class GenresController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GenresController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<GenreDto>>> GetGenres()
    {
        var genres = await _context.Genres.ToListAsync();
        var genresDto = _mapper.Map<List<GenreDto>>(genres);
        return genresDto;
    }

    [HttpGet("{id:int}", Name = "GetGenre")]
    public async Task<ActionResult<GenreDto>> GetGenreById(int id)
    {
        var genre = await _context.Genres.FirstOrDefaultAsync(x => x.Id == id);
        if (genre == null)
        {
            return NotFound();
        }

        var genreDto = _mapper.Map<GenreDto>(genre);
        return genreDto;
    }

    [HttpPost]
    public async Task<ActionResult> CreateGenre(GenreCreateDto genre)
    {
        var genreEntity = _mapper.Map<Genre>(genre);
        _context.Add(genreEntity);
        await _context.SaveChangesAsync();
        var genreDto = _mapper.Map<GenreDto>(genreEntity);
        return new CreatedAtRouteResult("GetGenre", new {id = genreDto.Id}, genreDto);
    }


    [HttpPut("{id:int}")]
    public async Task<ActionResult<Genre>> UpdateGenre(int id, GenreCreateDto genreDto)
    {
        var genre = await _context.Genres.FindAsync(id);
        if (genre == null)
        {
            return NotFound();
        }
        
        _mapper.Map(genreDto, genre);
        
        var result = await _context.SaveChangesAsync() > 0;

        if (result) return Ok(genre);
        return BadRequest(new ProblemDetails{Title = "Problem updating genre"});
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteGenre(int id)
    {
        var genre = await _context.Genres.FindAsync(id);
        if (genre == null)
        {
            return NotFound();
        }

        _context.Genres.Remove(genre);

        var result = await _context.SaveChangesAsync() > 0;
        if (result) return Ok();
        return BadRequest(new ProblemDetails {Title = "Problem deleting genre"});
    }
}