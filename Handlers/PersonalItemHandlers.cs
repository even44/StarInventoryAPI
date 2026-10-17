using Microsoft.AspNetCore.Http.HttpResults;
using StarInventoryAPI.Db;
using StarInventoryAPI.Extentions;
using StarInventoryAPI.Store;

namespace StarInventoryAPI.Handlers;

internal static class PersonalItemHandlers
{
    public static async Task<Results<Ok<List<StarItem>>, UnauthorizedHttpResult>> GetAllPersonalItemsList(string? search, ItemCacheDb db, HttpContext httpContext)
    {
        var username = httpContext.User.GetUsername();
        if (username == null)
        {
            return TypedResults.Unauthorized();
        }

        List<StarItem> items = string.IsNullOrWhiteSpace(search)
            ? await PersonalInventoryDataStore.GetPersonalItems(db, username)
            : await PersonalInventoryDataStore.SearchPersonalItems(db, username, search);
        return TypedResults.Ok(items);
    }

    public static async Task<Results<Ok<List<StarItem>>, UnauthorizedHttpResult>> GetAllPersonalItemsListSearch(string searchTerm, ItemCacheDb db, HttpContext httpContext)
    {
        var username = httpContext.User.GetUsername();
        if (username == null)
        {
            return TypedResults.Unauthorized();
        }

        var items = await PersonalInventoryDataStore.SearchPersonalItems(db, username, searchTerm);
        return TypedResults.Ok(items);
    }

    public static async Task<Results<Ok<StarItem>, NotFound, UnauthorizedHttpResult>> GetPersonalItem(int id, ItemCacheDb db, HttpContext httpContext)
    {
        var username = httpContext.User.GetUsername();
        if (username == null)
        {
            return TypedResults.Unauthorized();
        }

        var item = await PersonalInventoryDataStore.GetStarItem(db, id, username);
        if (item == null)
        {
            return TypedResults.NotFound();
        }
        return TypedResults.Ok(item);
    }

    public static async Task<Results<Created<StarItem>, UnauthorizedHttpResult, BadRequest<string>>> AddPersonalItemToInventory(StarItem item, ItemCacheDb db, HttpContext httpContext)
    {
        var username = httpContext.User.GetUsername();
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
        var username = httpContext.User.GetUsername();
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

    // KeyNotFoundException (404) and UnauthorizedAccessException (403) from the store are mapped to ProblemDetails by ApiExceptionHandler
    public static async Task<Results<Ok<StarItem>, UnauthorizedHttpResult>> UpdatePersonalItem(int id, StarItem item, ItemCacheDb db, HttpContext httpContext)
    {
        var username = httpContext.User.GetUsername();
        if (username == null)
        {
            return TypedResults.Unauthorized();
        }
        StarItem resultItem = await PersonalInventoryDataStore.UpdateStarItem(db, id, item, username);

        return TypedResults.Ok(resultItem);
    }

    public static async Task<Results<Ok, UnauthorizedHttpResult>> WipeAllPersonalItems(ItemCacheDb db, HttpContext httpContext)
    {
        var username = httpContext.User.GetUsername();
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
