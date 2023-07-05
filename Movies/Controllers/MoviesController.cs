using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.Data;
using Movies.DTOs;
using Movies.Entities;
using Movies.Services;

namespace Movies.Controllers;

[ApiController]
[Route("api/movies")]
public class MoviesController: ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IFileManager _fileManager;
    private readonly string _container = "movies";

    public MoviesController(ApplicationDbContext context, IMapper mapper, IFileManager fileManager)
    {
        _context = context;
        _mapper = mapper;
        _fileManager = fileManager;
    }

    [HttpGet]
    public async Task<ActionResult<List<MovieDto>>> GetMovies()
    {
        var movies = await _context.Movies.ToListAsync();
        return _mapper.Map<List<MovieDto>>(movies);
    }

    [HttpGet("{id}", Name = "GetMovie")]
    public async Task<ActionResult<MovieDto>> GetMovieById(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie == null) return NotFound();
        return _mapper.Map<MovieDto>(movie);
    }

    [HttpPost]
    public async Task<ActionResult> CreateMovie([FromForm]MovieCreateDto movieDto)
    {
        var movie = _mapper.Map<Movie>(movieDto);
        if (movieDto.Poster != null)
        {
            using (var memoryStream = new MemoryStream())
            {
                await movieDto.Poster.CopyToAsync(memoryStream);
                var content = memoryStream.ToArray();
                var extension = Path.GetExtension(movieDto.Poster.FileName);
                movie.Poster = await _fileManager.SaveFile(content, extension, _container, movieDto.Poster.ContentType);
            }
        }
        _context.Add(movie);
        var result = await _context.SaveChangesAsync() >0;
        if (result) return CreatedAtRoute("GetMovie", new { Id = movie.Id}, movie);
        
        return BadRequest(new ProblemDetails {Title = "Problem creating new movie"});
    }
    
    [HttpPatch("{id}")]
    public async Task<ActionResult<Actor>> UpdateMoviePatch(int id, JsonPatchDocument<MoviePatchDto> patchDocument)
    {
        if (patchDocument == null) return BadRequest(new ProblemDetails {Title = "Problem updating the movie"});
        var movie = await _context.Movies.FindAsync(id);
        if (movie == null) return NotFound();

        var movieDto = _mapper.Map<MoviePatchDto>(movie);
        
        patchDocument.ApplyTo(movieDto, ModelState);

        var isValid = TryValidateModel(movieDto);

        if (!isValid) return BadRequest(ModelState);

        _mapper.Map(movieDto, movie);

        var result = await _context.SaveChangesAsync() > 0;

        if (result) return Ok(movie);
        return BadRequest(new ProblemDetails{Title = "Problem updating movie"});
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateMovie(int id, [FromForm] MovieCreateDto movieDto)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie == null)
        {
            return NotFound();
        }
        
        var movieDb = _mapper.Map(movieDto, movie);
        
        if (movieDto.Poster != null)
        {
            using (var memoryStream = new MemoryStream())
            {
                await movieDto.Poster.CopyToAsync(memoryStream);
                var content = memoryStream.ToArray();
                var extension = Path.GetExtension(movieDto.Poster.FileName);
                movieDb.Poster = await _fileManager.EditFile(content, extension, _container, movieDb.Poster , movieDto.Poster.ContentType);
            }
        }
        
        var result = await _context.SaveChangesAsync() > 0;

        if (result) return Ok(movie);
        return BadRequest(new ProblemDetails{Title = "Problem updating movie"});
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMovie(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie == null)
        {
            return NotFound();
        }

        _context.Movies.Remove(movie);

        var result = await _context.SaveChangesAsync() > 0;
        if (result) return Ok();
        return BadRequest(new ProblemDetails {Title = "Problem deleting movie"});
    }

}