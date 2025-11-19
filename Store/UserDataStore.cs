using Microsoft.EntityFrameworkCore;

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

    public static async Task<bool> CreateUser(UserLogin newUserLogin, int roleId, ItemCacheDb db, PasswordHasher passwordHasher)
    {
        User? existingUser = await db.Users.FindAsync(newUserLogin.Username);
        if (existingUser != null)
        {
            return false;
        }

        User newUser = new User();

        newUser.Username = newUserLogin.Username;

        db.Users.Add(newUser);
        await db.SaveChangesAsync();

        return true;
    }


}