
using Microsoft.AspNetCore.Http.HttpResults;
using StarInventoryAPI.Store;

namespace StarInventoryAPI.Handlers;

internal static class OrgHandlers
{
    public static async Task<Ok<List<StarItem>>> GetAllSharedItemsFromOrgInventoryUsers(ItemCacheDb db)
    {
        var items = await OrgDataStore.GetOrgInventory(db);
        return TypedResults.Ok(items);
    }

    public static async Task<Results<Ok, Conflict<string>>> AddUserToOrgInventory(User user, ItemCacheDb db)
    {
        var result = await OrgDataStore.AddOrgInventoryUser(user.Username, db);
        if (!result)
        {
            return TypedResults.Conflict("User already exists in organization inventory.");
        }
        return TypedResults.Ok();
    }

    public static async Task<Results<Ok, NotFound<string>>> RemoveUserFromOrgInventory(string username, ItemCacheDb db)
    {
        var result = await OrgDataStore.RemoveOrgInventoryUser(username, db);
        if (!result)
        {
            return TypedResults.NotFound("User not found in organization inventory.");
        }
        return TypedResults.Ok();
    }

    public static async Task<Ok<List<OrgInventoryUser>>> GetOrgInventoryUsersList(ItemCacheDb db)
    {
        var users = await OrgDataStore.GetOrgInventoryUsers(db);
        return TypedResults.Ok(users);
    }

    public static async Task<Ok<List<User>>> GetAllUsers(ItemCacheDb db)
    {
        var users = await UserDataStore.GetUsers(db);
        return TypedResults.Ok(users);
    }
}