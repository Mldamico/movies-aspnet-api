using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Movies.Data;

namespace Movies.Helpers;

public class MovieExistAttribute: Attribute, IAsyncResultFilter
{
    private readonly ApplicationDbContext _dbContext;

    public MovieExistAttribute(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var movieIdObject = context.HttpContext.Request.RouteValues["movieId"];
        if (movieIdObject==null)
        {
            return;
        }

        var movieId = int.Parse(movieIdObject.ToString());
        var existMovie = await _dbContext.Movies.AnyAsync(x => x.Id == movieId);
        if (!existMovie)
        {
            context.Result = new NotFoundResult();
        }
        else
        {
            await next();
        }
    }
}