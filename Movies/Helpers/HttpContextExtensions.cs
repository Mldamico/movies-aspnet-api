using Microsoft.EntityFrameworkCore;

namespace Movies.Helpers;

public static class HttpContextExtensions
{
    public static async  Task PaginationParameters<T>(this HttpContext httpContext, IQueryable<T> queryable, int amountPerPage)
    {
        double amount = await queryable.CountAsync();
        double amountPage = Math.Ceiling(amount / amountPerPage);
        httpContext.Response.Headers.Add("amountPage", amountPage.ToString());
    }
}