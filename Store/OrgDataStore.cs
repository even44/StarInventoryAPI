using Microsoft.EntityFrameworkCore;

public static class OrgDataStore
{
    public static async Task<OrgInventory> GetOrgInventory(ItemCacheDb db)
    {
        var users = await db.OrgInventoryUsers.ToListAsync();
        List<StarItem> items = new List<StarItem>();
        foreach (var user in users)
        {
            var userItems = await db.PersonalItems
                .Where(item => item.Username.ToLower() == user.Username.ToLower() && item.IsSharedWithOrganization)
                .ToListAsync();
            items.AddRange(userItems);
        }

        return new OrgInventory
        {
            Items = items
        };
    }

    public static async Task<bool> AddOrgInventoryUser(string username, ItemCacheDb db)
    {
        var existingUser = await db.OrgInventoryUsers
            .Where(u => u.Username.ToLower() == username.ToLower())
            .FirstOrDefaultAsync();

        if (existingUser != null)
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
        return await db.OrgInventoryUsers.ToListAsync();
    }
}