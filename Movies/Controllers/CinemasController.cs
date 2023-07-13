using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.Data;
using Movies.DTOs;
using Movies.Entities;
using NetTopologySuite.Geometries;

namespace Movies.Controllers;

[Route("api/cinemas")]
[ApiController]
public class CinemasController:CustomBaseController
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly GeometryFactory _geometryFactory;

    public CinemasController(ApplicationDbContext context, IMapper mapper, GeometryFactory geometryFactory) : base(context, mapper)
    {
        _context = context;
        _mapper = mapper;
        _geometryFactory = geometryFactory;
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

    [HttpGet("near")]
    public async Task<ActionResult<List<CinemaDistanceDto>>> Near([FromQuery] CinemaDistanceFilterDto filter)
    {
        var userAddress = _geometryFactory.CreatePoint(new Coordinate(filter.Longitude, filter.Latitude));

        var cinema = await _context.Cinemas.OrderBy(x => x.Address.Distance(userAddress))
            .Where(x => x.Address.IsWithinDistance(userAddress, filter.DistanceKm * 1000))
            .Select(x => new CinemaDistanceDto
            {
                Id = x.Id,
                Name = x.Name,
                Latitude = x.Address.Y,
                Longitude = x.Address.X,
                DistanceOnMeters = Math.Round(x.Address.Distance(userAddress))
            }).ToListAsync();

        return cinema;
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