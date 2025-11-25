using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

public static class CacheEndpoints
{
    public static void MapCacheEndpoints(this IEndpointRouteBuilder app)
    {

        var cacheApi = app.MapGroup("/cache")
            .WithTags("Cached Items")
            .RequireAuthorization("user");

        cacheApi.MapGet("/locations", async Task<Ok<List<StarLocation>>> (ItemCacheDb db) =>
        {
            List<StarLocation> locations;
            locations = await db.StarLocations.ToListAsync();
            return TypedResults.Ok(locations);
        });

        cacheApi.MapGet("/locations/{searchTerm}", async Task<Ok<List<StarLocation>>> (string searchTerm, ItemCacheDb db) =>
        {
            List<StarLocation> locations;
            locations = await CacheDataStore.SearchStarLocations(db, searchTerm);
            return TypedResults.Ok(locations);
        });


        cacheApi.MapGet("/categories", async Task<Ok<List<UexCategory>>> (ItemCacheDb db) =>
        {
            List<UexCategory> categories = await CacheDataStore.GetUexCategories(db);
            return TypedResults.Ok(categories);
        });

        cacheApi.MapGet("/items", async Task<Ok<List<UexItem>>> (ItemCacheDb db) =>
        {
            List<UexItem> items = await CacheDataStore.GetUexItems(db);
            return TypedResults.Ok(items);
        });

        cacheApi.MapGet("/items/{searchTerm}", async Task<Ok<List<UexItem>>> (string searchTerm, ItemCacheDb db) =>
        {
            List<UexItem> items = await CacheDataStore.SearchUexItems(db, searchTerm);
            return TypedResults.Ok(items);
        });
    }
}