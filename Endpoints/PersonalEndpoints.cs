
public static class PersonalEndpoints
{
    public static void MapPersonalEndpoints(this IEndpointRouteBuilder app)
    {
        var personalApi = app.MapGroup("/personal")
            .WithTags("Personal Items")
            .RequireAuthorization("user");

            // GET LIST
            personalApi.MapGet("/items", async (ItemCacheDb db, HttpContext httpContext) =>
            {

                string? username = httpContext.User.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value;
                if (username == null)
                {
                    return Results.Unauthorized();
                }

                List<StarItem> items;
                items = await PersonalInventoryDataStore.GetPersonalItems(db, username);
                return Results.Ok(items);
            });

            personalApi.MapGet("/items/{searchTerm}", async (string searchTerm, ItemCacheDb db, HttpContext httpContext) =>
            {

                string? username = httpContext.User.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value;
                if (username == null)
                {
                    return Results.Unauthorized();
                }

                List<StarItem> items;
                items = await PersonalInventoryDataStore.SearchPersonalItems(db, username, searchTerm);
                return Results.Ok(items);
            });

            // ADD ONE
            personalApi.MapPost("/items/{id}", async (StarItem item, ItemCacheDb db, HttpContext httpContext) =>
            {
                string? username = httpContext.User.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value;
                if (username == null)
                {
                    return Results.Unauthorized();
                }
                StarItem? resultItem = await PersonalInventoryDataStore.AddStarItem(db, item, username);
                if (resultItem == null)
                {
                    return Results.BadRequest("Invalid Location");
                }
                return Results.Created($"/personal/items/{resultItem.Id}", resultItem);
            });

            // DELETE ONE
            personalApi.MapDelete("/items/{id}", async (int id, ItemCacheDb db, HttpContext httpContext) =>
            {
                string? username = httpContext.User.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value;
                if (username == null)
                {
                    return Results.Unauthorized();
                }
                bool result = await PersonalInventoryDataStore.DeleteStarItem(db, id, username);
                if (!result)
                {
                    return Results.NotFound("Item not found");
                }


                return Results.Ok();
            });

            personalApi.MapPut("/items/{id}", async (int id, StarItem item, ItemCacheDb db, HttpContext httpContext) =>
            {
                string? username = httpContext.User.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value;
                if (username == null)
                {
                    return Results.Unauthorized();
                }
                StarItem reusltItem = await PersonalInventoryDataStore.UpdateStarItem(db, id, item, username);

                return Results.Ok(reusltItem);
            });

            personalApi.MapGet("/clearinventory", async (ItemCacheDb db, HttpContext httpContext) =>
            {
                string? username = httpContext.User.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value;
                if (username == null)
                {
                    return Results.Unauthorized();
                }
                List<StarItem> items = await PersonalInventoryDataStore.GetPersonalItems(db, username);
                db.PersonalItems.RemoveRange(items);
                await db.SaveChangesAsync();

                return Results.Ok();
            });



    }
}