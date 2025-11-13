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
        string passwordHash = passwordHasher.HashPassword(newUserLogin.Password, newUser);
        newUser.Username = newUserLogin.Username;
        newUser.PasswordHash = passwordHash;
        newUser.RoleId = roleId;

        db.Users.Add(newUser);
        await db.SaveChangesAsync();

        return true;
    }

    public static async Task<bool> ChangeUserRole(string username, string role, ItemCacheDb db)
    {
        User? user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
        {
            return false;
        }
        Role? existingRole = await db.Roles.FirstOrDefaultAsync(r => r.ClaimString == role);
        if (existingRole == null)
        {
            return false;
        }
        user.RoleId = existingRole.Id;
        await db.SaveChangesAsync();
        return true;
    }
}