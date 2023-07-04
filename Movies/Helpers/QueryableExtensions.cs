using Movies.DTOs;

namespace Movies.Helpers;

public static class QueryableExtensions
{
    public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, PaginationDto paginationDto)
    {
        return queryable.Skip((paginationDto.Page - 1) * paginationDto.AmountPerPage)
            .Take(paginationDto.AmountPerPage);
      

    }
}