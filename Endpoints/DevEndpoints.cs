public static class DevEndpoints
{
    public static void MapDevEndpoints(this IEndpointRouteBuilder app)
    {
        var devApi = app.MapGroup("/dev")
            .WithTags("Development")
            .RequireAuthorization("dev");

        // Update the Cache from UEX and compile a list of locations
        devApi.MapGet("/updateCache", async (ItemCacheDb db, IHttpClientFactory httpClientFactory) =>
        {
            var client = httpClientFactory.CreateClient("UexApi");

            bool catResult = await db.UpdateCategories(db, client);
            if (!catResult)
            {
                Console.WriteLine("Failed to update categories");
                return Results.StatusCode(500);
            }
            bool locResult = await db.UpdatePois(db, client);
            if (!locResult)
            {
                Console.WriteLine("Failed to update pois");
                return Results.StatusCode(500);
            }
            bool spaceStationResult = await db.UpdateSpaceStations(db, client);
            if (!spaceStationResult)
            {
                Console.WriteLine("Failed to update space stations");
                return Results.StatusCode(500);
            }
            bool locationMergeResult = await db.CompileLocations(db);
            if (!locationMergeResult)
            {
                Console.WriteLine("Failed to compile locations");
                return Results.StatusCode(500);
            }

            bool itemResult = await db.UpdateItems(db, client);
            if (!itemResult)
            {
                Console.WriteLine("Failed to update items");
                return Results.StatusCode(500);
            }
            return Results.Ok("Cache Update Done");
        });


        devApi.MapGet("/randomitem", async (ItemCacheDb db, HttpContext httpContext) =>
        {
            string? username = httpContext.User.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value;
            if (username == null)
            {
                return Results.Unauthorized();
            }

            StarItem item = StarItem.RandomItem(db, username);

            StarItem? resultItem = await StarDataStore.AddStarItem(db, item, username);

            if (resultItem == null)
            {
                return Results.InternalServerError("Failed to add random item");
            }

            return Results.Created($"/personal/items/{resultItem.Id}", resultItem);

        });



    }
}