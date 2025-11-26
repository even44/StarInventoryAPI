using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

class AdminHandlers
{
    public static async Task<Ok<string>> WipeAllUsersPersonalItems(ItemCacheDb db)
    {
        var itemlist = await db.PersonalItems.ToListAsync();

        db.PersonalItems.RemoveRange(itemlist);

        await db.SaveChangesAsync();

        return TypedResults.Ok("Database Cleared");
    }
    
    public static async Task<Ok<List<User>>> GetUsersList(ItemCacheDb db){
    	var userList = await UserDataStore.GetUsers(db);

            return TypedResults.Ok(userList);
    }
}
