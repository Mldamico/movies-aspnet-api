using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.Data;
using Movies.DTOs;
using Movies.Entities;
using Movies.Helpers;

namespace Movies.Controllers;

public class CustomBaseController: ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CustomBaseController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    protected async Task<List<TDto>> Get<TEntity, TDto>() where TEntity : class
    {
        var entities = await _context.Set<TEntity>().AsNoTracking().ToListAsync();
        var dtos = _mapper.Map<List<TDto>>(entities);
        return dtos;
    }
    
    protected async Task<List<TDto>> GetPagination<TEntity, TDto>(PaginationDto paginationDto) where TEntity : class
    {
        var queryable = _context.Set<TEntity>().AsQueryable();
        return await GetPagination<TEntity, TDto>(paginationDto, queryable);
    }
    
    protected async Task<List<TDto>> GetPagination<TEntity, TDto>(PaginationDto paginationDto, IQueryable<TEntity> queryable) where TEntity : class
    {
        await HttpContext.PaginationParameters(queryable, paginationDto.AmountPerPage);
        var entities = await queryable.Paginate(paginationDto).ToListAsync();
        return _mapper.Map<List<TDto>>(entities);
    }

    
    protected async Task<ActionResult<TDto>> GetById<TEntity, TDto>(int id) where TEntity : class, IId
    {
        var entity = await _context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        return _mapper.Map<TDto>(entity);
    }

    protected async Task<ActionResult> Post<TCreation, TEntity, TRead>(TCreation createDto, string nameRoute)
        where TEntity : class, IId
    {
        var entity = _mapper.Map<TEntity>(createDto);
        _context.Add(entity);
        await _context.SaveChangesAsync();
        var dto = _mapper.Map<TRead>(entity);
        return new CreatedAtRouteResult(nameRoute, new {id = entity.Id}, dto );

    }

    protected async Task<ActionResult> Put<TCreation, TEntity>(int id, TCreation creationDto) where TEntity: class, IId
    {
        var entity = _mapper.Map<TEntity>(creationDto);
        entity.Id = id;
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    protected async Task<ActionResult> Delete<TEntity>(int id) where TEntity : class, IId, new()
    {
        var exists = await _context.Set<TEntity>().AnyAsync(x => x.Id ==id);
        if (!exists)
        {
            return NotFound();
        }

        _context.Remove(new TEntity() { Id = id});

        await _context.SaveChangesAsync();
        return NoContent();
    }

 
    protected async Task<ActionResult> Patch<TEntity, TDto>(int id, JsonPatchDocument<TDto> patchDocument) where TDto: class where TEntity: class, IId
    {
        if (patchDocument == null) return BadRequest(new ProblemDetails {Title = "Bad Request"});
        var entity = await _context.Set<TEntity>().FindAsync(id);
        if (entity == null) return NotFound();

        var entityDto = _mapper.Map<TDto>(entity);
        
        patchDocument.ApplyTo(entityDto, ModelState);

        var isValid = TryValidateModel(entityDto);

        if (!isValid) return BadRequest(ModelState);

        _mapper.Map(entityDto, entity);

        var result = await _context.SaveChangesAsync() > 0;

        if (result) return NoContent();
        return BadRequest(new ProblemDetails{Title = "Problem updating actor"});
    }
}