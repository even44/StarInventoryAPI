using Microsoft.AspNetCore.Http.HttpResults;

class DevHandlers
{
    public static async Task<Results<Ok<CacheUpdateResponse>, InternalServerError>> UpdateCacheFromUex(bool updateItems, bool updateLocations, ItemCacheDb db, IHttpClientFactory httpClientFactory)
    {
        var client = httpClientFactory.CreateClient("UexApi");



        if (updateItems)
        {
            bool catResult = await db.UpdateCategories(db, client);
            if (!catResult)
            {
                Console.WriteLine("Failed to update categories");
                return TypedResults.InternalServerError();
            }
            bool itemResult = await db.UpdateItems(db, client);

            if (!itemResult)
            {
                Console.WriteLine("Failed to update items");
                return TypedResults.InternalServerError();
            }
        }

        if (updateLocations)
        {
            bool poiResult = await db.UpdatePois(db, client);
            if (!poiResult)
            {
                Console.WriteLine("Failed to update pois");
                return TypedResults.InternalServerError();
            }
            bool spaceStationResult = await db.UpdateSpaceStations(db, client);
            if (!spaceStationResult)
            {
                Console.WriteLine("Failed to update space stations");
                return TypedResults.InternalServerError();
            }
            bool cityResult = await db.UpdateCities(db, client);
            if (!cityResult)
            {
                Console.WriteLine("Failed to update cities");
                return TypedResults.InternalServerError();
            }

            bool locationMergeResult = await db.CompileLocations(db);
            if (!locationMergeResult)
            {
                Console.WriteLine("Failed to compile locations");
                return TypedResults.InternalServerError();
            }
        }



        return TypedResults.Ok(await db.GetCacheUpdateResponse(db));
    }
}
