
using Microsoft.EntityFrameworkCore;

public static class RoleDataStore
{
    public static async Task<Role?> GetRole(int id, ItemCacheDb db)
    {
        return await db.Roles.FindAsync(id);
    }

    public static async Task<Role?> GetRoleByClaim(string claimString, ItemCacheDb db)
    {
        return await db.Roles.Where(role => role.ClaimString == claimString).FirstOrDefaultAsync();
    }
    public static async Task<Role?> GetRoleByName(string name, ItemCacheDb db)
    {
        return await db.Roles.Where(role => role.Name == name).FirstOrDefaultAsync();
    }

    public static async Task<List<Role>> GetRoles(ItemCacheDb db)
    {
        return await db.Roles.ToListAsync();
    }
    
    public static async Task<bool> CreateRole(string name, string claimString, ItemCacheDb db)
    {

        Role? existingRole = await GetRoleByClaim(claimString, db);
        if (existingRole != null)
        {
            return false;
        }
        existingRole = await GetRoleByName(claimString, db);
        if (existingRole != null)
        {
            return false;
        }

        await db.Roles.AddAsync(new Role
        {
            Id = 0,
            Name = name,
            ClaimString = claimString
        });
        await db.SaveChangesAsync();
        return true;
    }

    
}