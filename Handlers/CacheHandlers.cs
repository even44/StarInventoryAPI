using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using StarInventoryAPI.Db;
using StarInventoryAPI.Extentions;
using StarInventoryAPI.Store;

namespace StarInventoryAPI.Handlers;

internal static class CacheHandlers
{
    public static async Task<Results<Ok<List<StarLocation>>, ValidationProblem>> GetLocationsList(int? page, int? pageSize, ItemCacheDb db, HttpResponse response)
    {
        var errors = PaginationExtentions.ValidatePaging(page, pageSize);
        if (errors != null) return TypedResults.ValidationProblem(errors);

        var locations = await db.StarLocations.OrderBy(loc => loc.Id).ToPagedListAsync(page, pageSize, response);
        return TypedResults.Ok(locations);
    }

    public static async Task<Ok<List<StarLocation>>> GetLocationsListSearch(string searchTerm, ItemCacheDb db)
    {
        var locations = await CacheDataStore.SearchStarLocations(db, searchTerm);
        return TypedResults.Ok(locations);
    }

    public static async Task<Ok<List<UexCategory>>> GetCategoriesList(ItemCacheDb db)
    {
        var categories = await CacheDataStore.GetUexCategories(db);
        return TypedResults.Ok(categories);
    }

    public static async Task<Results<Ok<List<UexItem>>, ValidationProblem>> GetItemsList(int? page, int? pageSize, ItemCacheDb db, HttpResponse response)
    {
        var errors = PaginationExtentions.ValidatePaging(page, pageSize);
        if (errors != null) return TypedResults.ValidationProblem(errors);

        var items = await db.UexItems.OrderBy(item => item.Id).ToPagedListAsync(page, pageSize, response);
        return TypedResults.Ok(items);
    }

    public static async Task<Ok<List<UexItem>>> GetItemsListSearch(string searchTerm, ItemCacheDb db)
    {
        var items = await CacheDataStore.SearchUexItems(db, searchTerm);
        return TypedResults.Ok(items);
    }
}