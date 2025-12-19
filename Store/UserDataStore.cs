using Microsoft.EntityFrameworkCore;

namespace StarInventoryAPI.Store;

public static class UserDataStore
{
    // USER actions

    public static async Task<List<User>> GetUsers(ItemCacheDb db)
    {
        return await db.Users.ToListAsync();
    }

    public static async Task<User?> GetUser(string username, ItemCacheDb db)
    {
        return await db.Users.FindAsync(username);
    }

    public static async Task<bool> CreateUser(string username, ItemCacheDb db)
    {
        User? existingUser = await db.Users.FindAsync(username);
        if (existingUser != null)
        {
            return false;
        }

        User newUser = new User
        {
            Username = username
        };

        db.Users.Add(newUser);
        await db.SaveChangesAsync();

        return true;
    }

    public static async Task<bool> EnsureUserExists(ItemCacheDb db, string username)
    {
        if (null == await UserDataStore.GetUser(username, db))
        {
            await UserDataStore.CreateUser(username, db);
        }
        return true;
    }


}