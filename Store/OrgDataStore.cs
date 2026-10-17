using Microsoft.EntityFrameworkCore;
using StarInventoryAPI.Db;

namespace StarInventoryAPI.Store;

public static class OrgDataStore
{
    public static async Task<List<StarItem>> GetOrgInventory(ItemCacheDb db)
    {
        // Single JOIN instead of one query per org user. Relies on the case-insensitive
        // default collation of the Username columns (same semantics as the old ToLower() compare).
        return await db.PersonalItems
            .AsNoTracking()
            .Where(item => item.IsSharedWithOrganization)
            .Join(db.OrgInventoryUsers,
                item => item.Username,
                user => user.Username,
                (item, user) => item)
            .ToListAsync();
    }

    public static async Task<bool> AddOrgInventoryUser(string username, ItemCacheDb db)
    {
        var userExists = await db.OrgInventoryUsers
            .AnyAsync(u => u.Username.ToLower() == username.ToLower());

        if (userExists)
        {
            return false;
        }

        await db.OrgInventoryUsers.AddAsync(new OrgInventoryUser
        {
            Username = username
        });
        await db.SaveChangesAsync();
        return true;
    }

    public static async Task<bool> RemoveOrgInventoryUser(string username, ItemCacheDb db)
    {
        var existingUser = await db.OrgInventoryUsers
            .Where(u => u.Username.ToLower() == username.ToLower())
            .FirstOrDefaultAsync();

        if (existingUser == null)
        {
            return false;
        }

        db.OrgInventoryUsers.Remove(existingUser);
        await db.SaveChangesAsync();
        return true;
    }

    public static async Task<List<OrgInventoryUser>> GetOrgInventoryUsers(ItemCacheDb db)
    {
        return await db.OrgInventoryUsers.AsNoTracking().ToListAsync();
    }
}