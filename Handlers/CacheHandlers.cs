using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using StarInventoryAPI.Db;
using StarInventoryAPI.Store;

namespace StarInventoryAPI.Handlers;

internal static class CacheHandlers
{
    public static async Task<Ok<List<StarLocation>>> GetLocationsList(ItemCacheDb db)
    {
        var locations = await db.StarLocations.ToListAsync();
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

    public static async Task<Ok<List<UexItem>>> GetItemsList(ItemCacheDb db)
    {
        var items = await CacheDataStore.GetUexItems(db);
        return TypedResults.Ok(items);
    }

    public static async Task<Ok<List<UexItem>>> GetItemsListSearch(string searchTerm, ItemCacheDb db)
    {
        var items = await CacheDataStore.SearchUexItems(db, searchTerm);
        return TypedResults.Ok(items);
    }
}