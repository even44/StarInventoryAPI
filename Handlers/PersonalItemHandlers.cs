using Microsoft.AspNetCore.Http.HttpResults;

namespace StarInventoryAPI.Handlers;

internal static class PersonalItemHandlers
{
    public static async Task<Results<Ok<List<StarItem>>, UnauthorizedHttpResult>> GetAllPersonalItemsList(ItemCacheDb db, HttpContext httpContext)
    {
        var username = httpContext.User.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value;
        if (username == null)
        {
            return TypedResults.Unauthorized();
        }

        List<StarItem> items;
        items = await PersonalInventoryDataStore.GetPersonalItems(db, username);
        return TypedResults.Ok(items);
    }

    public static async Task<Results<Ok<List<StarItem>>, UnauthorizedHttpResult>> GetAllPersonalItemsListSearch(string searchTerm, ItemCacheDb db, HttpContext httpContext)
    {
        var username = httpContext.User.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value;
        if (username == null)
        {
            return TypedResults.Unauthorized();
        }

        var items = await PersonalInventoryDataStore.SearchPersonalItems(db, username, searchTerm);
        return TypedResults.Ok(items);
    }

    public static async Task<Results<Created<StarItem>, UnauthorizedHttpResult, BadRequest<string>>> AddPersonalItemToInventory(StarItem item, ItemCacheDb db, HttpContext httpContext)
    {
        var username = httpContext.User.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value;
        if (username == null)
        {
            return TypedResults.Unauthorized();
        }
        var resultItem = await PersonalInventoryDataStore.AddStarItem(db, item, username);
        if (resultItem == null)
        {
            return TypedResults.BadRequest("Invalid Location");
        }
        return TypedResults.Created($"/personal/items/{resultItem.Id}", resultItem);
    }

    public static async Task<Results<Ok, NotFound<string>, UnauthorizedHttpResult>> RemovePersonalItemFromInventory(int id, ItemCacheDb db, HttpContext httpContext)
    {
        var username = httpContext.User.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value;
        if (username == null)
        {
            return TypedResults.Unauthorized();
        }
        var result = await PersonalInventoryDataStore.DeleteStarItem(db, id, username);
        if (!result)
        {
            return TypedResults.NotFound("Item not found");
        }


        return TypedResults.Ok();
    }

    public static async Task<Results<Ok<StarItem>, UnauthorizedHttpResult>> GetOnePersonalItemFromId(int id, StarItem item, ItemCacheDb db, HttpContext httpContext)
    {
        string? username = httpContext.User.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value;
        if (username == null)
        {
            return TypedResults.Unauthorized();
        }
        StarItem reusltItem = await PersonalInventoryDataStore.UpdateStarItem(db, id, item, username);

        return TypedResults.Ok(reusltItem);
    }

    public static async Task<Results<Ok, UnauthorizedHttpResult>> WipeAllPersonalItems(ItemCacheDb db, HttpContext httpContext)
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
    }

}