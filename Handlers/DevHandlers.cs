using Microsoft.AspNetCore.Http.HttpResults;
using StarInventoryAPI.Db;

namespace StarInventoryAPI.Handlers;

internal static class DevHandlers
{

    public static async Task<Results<Ok<CacheUpdateResponse>, InternalServerError>> UpdateCacheFromUex(bool updateItems, bool updateLocations, ItemCacheDb db, IHttpClientFactory httpClientFactory)
    {
        var client = httpClientFactory.CreateClient("UexApi");

        if (updateItems)
        {
            var catResult = await db.UpdateCategories(db, client);
            if (!catResult)
            {
                Console.WriteLine("Failed to update categories");
                return TypedResults.InternalServerError();
            }
            var itemResult = await db.UpdateItems(db, client);

            if (!itemResult)
            {
                Console.WriteLine("Failed to update items");
                return TypedResults.InternalServerError();
            }
        }

        if (!updateLocations) return TypedResults.Ok(await db.GetCacheUpdateResponse(db));
        var poiResult = await db.UpdatePois(db, client);
        if (!poiResult)
        {
            Console.WriteLine("Failed to update pois");
            return TypedResults.InternalServerError();
        }
        var spaceStationResult = await db.UpdateSpaceStations(db, client);
        if (!spaceStationResult)
        {
            Console.WriteLine("Failed to update space stations");
            return TypedResults.InternalServerError();
        }
        var cityResult = await db.UpdateCities(db, client);
        if (!cityResult)
        {
            Console.WriteLine("Failed to update cities");
            return TypedResults.InternalServerError();
        }

        var locationMergeResult = await db.CompileLocations(db);
        if (locationMergeResult) return TypedResults.Ok(await db.GetCacheUpdateResponse(db));
        Console.WriteLine("Failed to compile locations");
        return TypedResults.InternalServerError();



    }
}