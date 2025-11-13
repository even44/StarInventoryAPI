using Microsoft.EntityFrameworkCore;

public static class CacheEndpoints
{
    public static void MapCacheEndpoints(this IEndpointRouteBuilder app)
    {

        var cacheApi = app.MapGroup("/cache")
            .WithTags("Cached Items")
            .RequireAuthorization("user");

        cacheApi.MapGet("/locations", async (ItemCacheDb db) =>
        {
            List<StarLocation> locations;
            locations = await db.StarLocations.ToListAsync();
            return Results.Ok(locations);
        });

        cacheApi.MapGet("/locations/{searchTerm}", async (string searchTerm, ItemCacheDb db) =>
        {
            List<StarLocation> locations;
            locations = await CacheDataStore.SearchStarLocations(db, searchTerm);
            return Results.Ok(locations);
        });


        cacheApi.MapGet("/categories", async (ItemCacheDb db) =>
        {
            List<UexCategory> categories = await CacheDataStore.GetUexCategories(db);
            return Results.Ok(categories);
        });

        cacheApi.MapGet("/items", async (ItemCacheDb db) =>
        {
            List<UexItem> items = await CacheDataStore.GetUexItems(db);
            return Results.Ok(items);
        });

        cacheApi.MapGet("/items/{searchTerm}", async (string searchTerm, ItemCacheDb db) =>
        {
            List<UexItem> items = await CacheDataStore.SearchUexItems(db, searchTerm);
            return Results.Ok(items);
        });
    }
}