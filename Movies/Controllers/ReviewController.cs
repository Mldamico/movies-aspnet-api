using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.Data;
using Movies.DTOs;
using Movies.Entities;
using Movies.Helpers;

namespace Movies.Controllers;

[Route("api/movies/{movieId:int}/review")]
[ServiceFilter(typeof(MovieExistAttribute))]
public class ReviewController : CustomBaseController
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ReviewController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<ReviewDto>>> GetReviews(int movieId, [FromQuery] PaginationDto paginationDto)
    {
        
        var queryable = _context.Reviews.Include(x => x.User).AsQueryable();
        queryable = queryable.Where(x => x.MovieId == movieId);
        return await GetPagination<Review, ReviewDto>(paginationDto, queryable);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> CreateReview(int movieId, [FromBody] ReviewCreateDto reviewCreateDto)
    {
        
        var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
        var reviewFromUserAlreadyDone =
            await _context.Reviews.AnyAsync(x => x.MovieId == movieId && x.UserId == userId);
        if (reviewFromUserAlreadyDone) return BadRequest("User already posted a review for this movie.");
        var review = _mapper.Map<Review>(reviewCreateDto);
        review.MovieId = movieId;
        review.UserId = userId;
        _context.Add(review);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{reviewId:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> UpdateReview(int movieId, int reviewId, [FromBody] ReviewCreateDto reviewCreateDto)
    {
       

        var reviewDb = await _context.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId);
        if (reviewDb == null) return NotFound();
        var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
        if (reviewDb.UserId != userId) return Forbid();

        reviewDb = _mapper.Map(reviewCreateDto, reviewDb);

        await _context.SaveChangesAsync();
        return NoContent();

    }

    [HttpDelete("{revieId:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> DeleteReview(int reviewId)
    {
        var reviewDb = await _context.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId);
        if (reviewDb == null) return NotFound();

        var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
        if (reviewDb.UserId != userId) return Forbid();
        _context.Remove(reviewDb);
        await _context.SaveChangesAsync();
        return NoContent();
    }
} 