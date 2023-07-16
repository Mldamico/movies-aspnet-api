using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Movies.Data;
using Movies.DTOs;
using Movies.Entities;
using Movies.Services;

namespace Movies.Controllers;

[ApiController]
[Route("api/actors")]
public class ActorsController : CustomBaseController
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IFileManager _fileManager;
    private readonly string _container = "actors";

    public ActorsController(ApplicationDbContext context, IMapper mapper, IFileManager fileManager): base(context, mapper)
    {
        _context = context;
        _mapper = mapper;
        _fileManager = fileManager;
    }

    [HttpGet]
    public async Task<ActionResult<List<ActorDto>>> GetActors([FromQuery] PaginationDto paginationDto)
    {
        return await GetPagination<Actor, ActorDto>(paginationDto);

    }

    [HttpGet("{id:int}", Name = "GetActor")]
    public async Task<ActionResult<ActorDto>> GetActorById(int id)
    {
        return await GetById<Actor, ActorDto>(id);
    }

    [HttpPost]
    public async Task<ActionResult> CreateActor([FromForm]ActorCreateDto actorDto)
    {
        var actor = _mapper.Map<Actor>(actorDto);
        if (actorDto.Photo != null)
        {
            using (var memoryStream = new MemoryStream())
            {
                await actorDto.Photo.CopyToAsync(memoryStream);
                var content = memoryStream.ToArray();
                var extension = Path.GetExtension(actorDto.Photo.FileName);
                actor.Photo = await _fileManager.SaveFile(content, extension, _container, actorDto.Photo.ContentType);
            }
        }
        _context.Add(actor);
        var result = await _context.SaveChangesAsync() >0;
        if (result) return CreatedAtRoute("GetActor", new { Id = actor.Id}, actor);
        
        return BadRequest(new ProblemDetails {Title = "Problem creating new actor"});

 
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> UpdateActorPatch(int id, JsonPatchDocument<ActorPatchDto> patchDocument)
    {
        return await Patch<Actor, ActorPatchDto>(id, patchDocument);
    }
    
    [HttpPut("{id:int}")]
    public async Task<ActionResult<Actor>> UpdateActor(int id, [FromForm]ActorCreateDto actorDto)
    {
        var actor = await _context.Actors.FindAsync(id);
        if (actor == null)
        {
            return NotFound();
        }
        
        var actorDb = _mapper.Map(actorDto, actor);
        
        if (actorDto.Photo != null)
        {
            using (var memoryStream = new MemoryStream())
            {
                await actorDto.Photo.CopyToAsync(memoryStream);
                var content = memoryStream.ToArray();
                var extension = Path.GetExtension(actorDto.Photo.FileName);
                actorDb.Photo = await _fileManager.EditFile(content, extension, _container, actorDb.Photo ,actorDto.Photo.ContentType);
            }
        }
        
        var result = await _context.SaveChangesAsync() > 0;

        if (result) return Ok(actor);
        return BadRequest(new ProblemDetails{Title = "Problem updating actor"});
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteActor(int id)
    {
        return await Delete<Actor>(id);
    }


}