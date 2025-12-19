using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using StarInventoryAPI.Db;
using StarInventoryAPI.Store;

namespace StarInventoryAPI.Handlers;

internal static class AdminHandlers
{
    public static async Task<Ok<string>> WipeAllUsersPersonalItems(ItemCacheDb db)
    {
        var itemList = await db.PersonalItems.ToListAsync();

        db.PersonalItems.RemoveRange(itemList);

        await db.SaveChangesAsync();

        return TypedResults.Ok("Database Cleared");
    }
    
    public static async Task<Ok<List<User>>> GetUsersList(ItemCacheDb db){
        var userList = await UserDataStore.GetUsers(db);

        return TypedResults.Ok(userList);
    }
}