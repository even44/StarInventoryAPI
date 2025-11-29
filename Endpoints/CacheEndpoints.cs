public static class CacheEndpoints
{

    public static void MapCacheEndpoints(this IEndpointRouteBuilder app)
    {

        var cacheApi = app.MapGroup("/cache")
            .WithTags("Cached Items")
            .RequireAuthorization("user");

        cacheApi.MapGet("/locations", CacheHandlers.GetLocationsList);

        cacheApi.MapGet("/locations/{searchTerm}", CacheHandlers.GetLocationsListSearch);

        cacheApi.MapGet("/categories", CacheHandlers.GetCategoriesList);

        cacheApi.MapGet("/items", CacheHandlers.GetItemsList);

        cacheApi.MapGet("/items/{searchTerm}", CacheHandlers.GetItemsListSearch);
    }
}
