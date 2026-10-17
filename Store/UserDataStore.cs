using Microsoft.EntityFrameworkCore;
using StarInventoryAPI.Db;

namespace StarInventoryAPI.Store;

public static class UserDataStore
{
    // USER actions

    public static async Task<List<User>> GetUsers(ItemCacheDb db)
    {
        return await db.Users.AsNoTracking().ToListAsync();
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
        // One existence check instead of GetUser + CreateUser's second FindAsync
        if (await db.Users.AnyAsync(u => u.Username == username))
        {
            return true;
        }

        var entry = db.Users.Add(new User { Username = username });
        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // A concurrent request created the user first; stop tracking our duplicate
            entry.State = EntityState.Detached;
        }
        return true;
    }


}