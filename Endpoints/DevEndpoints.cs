using Microsoft.AspNetCore.Http.HttpResults;

public static class DevEndpoints
{
    public static void MapDevEndpoints(this IEndpointRouteBuilder app)
    {
        var devApi = app.MapGroup("/dev")
            .WithTags("Development")
            .RequireAuthorization("dev");

        // Update the Cache from UEX and compile a list of locations
        devApi.MapGet("/updateCache", async Task<Results<Ok, InternalServerError>> (ItemCacheDb db, IHttpClientFactory httpClientFactory) =>
        {
            var client = httpClientFactory.CreateClient("UexApi");

            bool catResult = await db.UpdateCategories(db, client);
            if (!catResult)
            {
                Console.WriteLine("Failed to update categories");
                return TypedResults.InternalServerError();
            }
            bool locResult = await db.UpdatePois(db, client);
            if (!locResult)
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
            bool locationMergeResult = await db.CompileLocations(db);
            if (!locationMergeResult)
            {
                Console.WriteLine("Failed to compile locations");
                return TypedResults.InternalServerError();
            }

            bool itemResult = await db.UpdateItems(db, client);
            if (!itemResult)
            {
                Console.WriteLine("Failed to update items");
                return TypedResults.InternalServerError();
            }
            return TypedResults.Ok();
        });


        devApi.MapGet("/randomitem", async Task<Results<Created<StarItem>, UnauthorizedHttpResult, InternalServerError<string>>> (ItemCacheDb db, HttpContext httpContext) =>
        {
            string? username = httpContext.User.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value;
            if (username == null)
            {
                return TypedResults.Unauthorized();
            }

            StarItem item = StarItem.RandomItem(db, username);

            StarItem? resultItem = await PersonalInventoryDataStore.AddStarItem(db, item, username);

            if (resultItem == null)
            {
                return TypedResults.InternalServerError("Failed to add random item");
            }

            return TypedResults.Created($"/personal/items/{resultItem.Id}", resultItem);

        });



    }
}