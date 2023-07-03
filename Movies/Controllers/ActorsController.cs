using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.Data;
using Movies.DTOs;
using Movies.Entities;

namespace Movies.Controllers;

[ApiController]
[Route("api/actors")]
public class ActorsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ActorsController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<ActorDto>>> GetActors()
    {
        var actors = await _context.Actors.ToListAsync();
        var actorsDto = _mapper.Map<List<ActorDto>>(actors);
        return actorsDto;
    }

    [HttpGet("{id:int}", Name = "GetActor")]
    public async Task<ActionResult<ActorDto>> GetActorById(int id)
    {
        var actor = await _context.Actors.FindAsync(id);
        if (actor == null) return NotFound();
        
        var actorDto  = _mapper.Map<ActorDto>(actor);
        return actorDto;
    }

    [HttpPost]
    public async Task<ActionResult> CreateActor(ActorCreateDto actorDto)
    {
        var actor = _mapper.Map<Actor>(actorDto);
        _context.Add(actor);
        var result = await _context.SaveChangesAsync() >0;
        if (result) return CreatedAtRoute("GetActor", new { Id = actor.Id}, actor);

        return BadRequest(new ProblemDetails {Title = "Problem creating new actor"});
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Actor>> UpdateActor(int id,ActorCreateDto actorDto)
    {
        var actor = await _context.Actors.FindAsync(id);
        if (actor == null)
        {
            return NotFound();
        }
        
        _mapper.Map(actorDto, actor);
        
        var result = await _context.SaveChangesAsync() > 0;

        if (result) return Ok(actor);
        return BadRequest(new ProblemDetails{Title = "Problem updating actor"});
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteActor(int id)
    {
        var actor = await _context.Actors.FindAsync(id);
        if (actor == null)
        {
            return NotFound();
        }

        _context.Actors.Remove(actor);

        var result = await _context.SaveChangesAsync() > 0;
        if (result) return Ok();
        return BadRequest(new ProblemDetails {Title = "Problem deleting actor"});
    }


}