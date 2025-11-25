
using Microsoft.AspNetCore.Http.HttpResults;

public static class PersonalEndpoints
{
    public static void MapPersonalEndpoints(this IEndpointRouteBuilder app)
    {
        var personalApi = app.MapGroup("/personal")
            .WithTags("Personal Items")
            .RequireAuthorization("user");

            // GET LIST
            personalApi.MapGet("/items", async Task<Results<Ok<List<StarItem>>, UnauthorizedHttpResult>> (ItemCacheDb db, HttpContext httpContext) =>
            {

                string? username = httpContext.User.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value;
                if (username == null)
                {
                    return TypedResults.Unauthorized();
                }

                List<StarItem> items;
                items = await PersonalInventoryDataStore.GetPersonalItems(db, username);
                return TypedResults.Ok(items);
            });

            personalApi.MapGet("/items/{searchTerm}", async Task<Results<Ok<List<StarItem>>, UnauthorizedHttpResult>> (string searchTerm, ItemCacheDb db, HttpContext httpContext) =>
            {

                string? username = httpContext.User.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value;
                if (username == null)
                {
                    return TypedResults.Unauthorized();
                }

                List<StarItem> items;
                items = await PersonalInventoryDataStore.SearchPersonalItems(db, username, searchTerm);
                return TypedResults.Ok(items);
            });

            // ADD ONE
            personalApi.MapPost("/items", async Task<Results<Created<StarItem>, UnauthorizedHttpResult, BadRequest<string>>> (StarItem item, ItemCacheDb db, HttpContext httpContext) =>
            {
                string? username = httpContext.User.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value;
                if (username == null)
                {
                    return TypedResults.Unauthorized();
                }
                StarItem? resultItem = await PersonalInventoryDataStore.AddStarItem(db, item, username);
                if (resultItem == null)
                {
                    return TypedResults.BadRequest("Invalid Location");
                }
                return TypedResults.Created($"/personal/items/{resultItem.Id}", resultItem);
            });

            // DELETE ONE
            personalApi.MapDelete("/items/{id}", async Task<Results<Ok, NotFound<string>, UnauthorizedHttpResult>> (int id, ItemCacheDb db, HttpContext httpContext) =>
            {
                string? username = httpContext.User.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value;
                if (username == null)
                {
                    return TypedResults.Unauthorized();
                }
                bool result = await PersonalInventoryDataStore.DeleteStarItem(db, id, username);
                if (!result)
                {
                    return TypedResults.NotFound("Item not found");
                }


                return TypedResults.Ok();
            });

            personalApi.MapPut("/items/{id}", async Task<Results<Ok<StarItem>, UnauthorizedHttpResult>> (int id, StarItem item, ItemCacheDb db, HttpContext httpContext) =>
            {
                string? username = httpContext.User.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value;
                if (username == null)
                {
                    return TypedResults.Unauthorized();
                }
                StarItem reusltItem = await PersonalInventoryDataStore.UpdateStarItem(db, id, item, username);

                return TypedResults.Ok(reusltItem);
            });

            personalApi.MapGet("/clearinventory", async Task<Results<Ok, UnauthorizedHttpResult>> (ItemCacheDb db, HttpContext httpContext) =>
            {
                string? username = httpContext.User.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value;
                if (username == null)
                {
                    return TypedResults.Unauthorized();
                }
                List<StarItem> items = await PersonalInventoryDataStore.GetPersonalItems(db, username);
                db.PersonalItems.RemoveRange(items);
                await db.SaveChangesAsync();

                return TypedResults.Ok();
            });



    }
}