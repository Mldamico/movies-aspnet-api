using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Movies.Data;
using Movies.DTOs;
using Movies.Entities;

namespace Movies.Controllers;

[Route("api/cinemas")]
[ApiController]
public class CinemasController:CustomBaseController
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CinemasController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<CinemaDto>>> GetCinemas()
    {
        return await Get<Cinema, CinemaDto>();
    }
    
    [HttpGet("{id:int}", Name = "getCinema")]
    public async Task<ActionResult<CinemaDto>> GetCinemaById(int id)
    {
        return await GetById<Cinema, CinemaDto>(id);
    }

    [HttpPost]
    public async Task<ActionResult> CreateCinema(CinemaCreateDto cinemaCreateDto)
    {
        return await Post<CinemaCreateDto, Cinema, CinemaDto>(cinemaCreateDto, "getCinema");
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCinema(int id, CinemaCreateDto cinemaCreateDto)
    {
        return await Put<CinemaCreateDto, Cinema>(id, cinemaCreateDto);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCinema(int id)
    {
        return await Delete<Cinema>(id);
    }

}