using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

class CacheHandlers
{
    public static async Task<Ok<List<StarLocation>>> GetLocationsList(ItemCacheDb db)
    {
        List<StarLocation> locations;
        locations = await db.StarLocations.ToListAsync();
        return TypedResults.Ok(locations);
    }

    public static async Task<Ok<List<StarLocation>>> GetLocationsListSearch(string searchTerm, ItemCacheDb db)
    {
        List<StarLocation> locations;
        locations = await CacheDataStore.SearchStarLocations(db, searchTerm);
        return TypedResults.Ok(locations);
    }

    public static async Task<Ok<List<UexCategory>>> GetCategoriesList(ItemCacheDb db)
    {
        List<UexCategory> categories = await CacheDataStore.GetUexCategories(db);
        return TypedResults.Ok(categories);
    }

    public static async Task<Ok<List<UexItem>>> GetItemsList(ItemCacheDb db)
    {
        List<UexItem> items = await CacheDataStore.GetUexItems(db);
        return TypedResults.Ok(items);
    }

    public static async Task<Ok<List<UexItem>>> GetItemsListSearch(string searchTerm, ItemCacheDb db)
    {
        List<UexItem> items = await CacheDataStore.SearchUexItems(db, searchTerm);
        return TypedResults.Ok(items);
    }
}
