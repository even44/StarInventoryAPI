


using Microsoft.EntityFrameworkCore;

public static class StarDataStore
{
    public static async Task<StarItem?> AddStarItem(ItemCacheDb db, StarItem item)
    {

        var location = await db.StarLocations.FirstOrDefaultAsync(loc => loc.Name == item.Location.Name);
        if (location == null)
        {
            return null;
        }

        var existingItem = await db.PersonalItems.Include(item => item.Location).FirstOrDefaultAsync(i => i.Name == item.Name && i.Location.Name == item.Location.Name);
        if (existingItem == null)
        {
            db.PersonalItems.Add(item);
            await db.SaveChangesAsync();
            return item;
        }
        else
        {
            existingItem.Quantity += item.Quantity;
            await db.SaveChangesAsync();
            return existingItem;
        }

    }

    public static async Task<StarItem> UpdateStarItem(ItemCacheDb db, int id, StarItem item)
    {
        StarItem? existingItem = await db.PersonalItems.FindAsync(id);
        if (existingItem == null)
        {
            throw new KeyNotFoundException("Item not found");
        }

        item.Id = id;

        existingItem.Name = item.Name;
        existingItem.IsSharedWithOrganization = item.IsSharedWithOrganization;
        existingItem.Quantity = item.Quantity;

        await db.SaveChangesAsync();

        return existingItem;
    }

    public static async Task<List<StarItem>> GetPersonalItems(ItemCacheDb db)
    {
        List<StarItem> items = await db.PersonalItems.Include(item => item.Location).ToListAsync();
        return items;
    }
}