using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.Data;
using Movies.DTOs;
using Movies.Services;

namespace Movies.Controllers;

[ApiController]
[Route("api/movies")]
public class MoviesController: ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IFileManager _fileManager;

    public MoviesController(ApplicationDbContext context, IMapper mapper, IFileManager fileManager)
    {
        _context = context;
        _mapper = mapper;
        _fileManager = fileManager;
    }

    [HttpGet]
    public async Task<ActionResult<List<MovieDto>>> GetMovies()
    {
        var movies = _context.Movies.ToListAsync();
        return _mapper.Map<List<MovieDto>>(movies);
    }


}