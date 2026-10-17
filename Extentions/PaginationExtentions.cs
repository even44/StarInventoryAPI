using Microsoft.EntityFrameworkCore;

namespace StarInventoryAPI.Extentions;

public static class PaginationExtentions
{
    public const int MaxPageSize = 500;
    public const string TotalCountHeader = "X-Total-Count";

    // Returns validation errors for the page/pageSize query parameters, or null when they are valid.
    // Pagination is optional: both parameters absent means "return everything".
    public static Dictionary<string, string[]>? ValidatePaging(int? page, int? pageSize)
    {
        var errors = new Dictionary<string, string[]>();
        if (page is < 1) errors["page"] = ["page must be 1 or greater."];
        if (pageSize is < 1 or > MaxPageSize) errors["pageSize"] = [$"pageSize must be between 1 and {MaxPageSize}."];
        return errors.Count == 0 ? null : errors;
    }

    // Applies paging when page or pageSize is given and writes the total (unpaged) count to the X-Total-Count header.
    // The query must already be ordered so pages are stable.
    public static async Task<List<T>> ToPagedListAsync<T>(this IQueryable<T> query, int? page, int? pageSize, HttpResponse response)
    {
        if (page == null && pageSize == null)
        {
            var all = await query.ToListAsync();
            response.Headers[TotalCountHeader] = all.Count.ToString();
            return all;
        }

        var size = pageSize ?? 50;
        var number = page ?? 1;
        response.Headers[TotalCountHeader] = (await query.CountAsync()).ToString();
        return await query.Skip((number - 1) * size).Take(size).ToListAsync();
    }
}
