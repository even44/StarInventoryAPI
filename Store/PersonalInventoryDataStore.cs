using Microsoft.EntityFrameworkCore;

public static class PersonalInventoryDataStore
{
        public static async Task<StarItem?> AddStarItem(ItemCacheDb db, StarItem item, string username)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
        {
            Console.WriteLine($"Invalid User when adding item");
            return null;
        }
        else
        {
            item.Username = username;
        }


        var location = await db.StarLocations.FirstOrDefaultAsync(loc => loc.Id == item.LocationId);
        if (location == null)
        {
            Console.WriteLine($"Invalid Location when adding item");
            return null;
        }

        var existingItem = await db.PersonalItems.FirstOrDefaultAsync(i => i.Name == item.Name && i.LocationId == item.LocationId && i.Username == username);
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

    public static async Task<StarItem> UpdateStarItem(ItemCacheDb db, int id, StarItem item, string username)
    {
        StarItem? existingItem = await db.PersonalItems.FindAsync(id);
        if (existingItem == null)
        {
            throw new KeyNotFoundException("Item not found");
        }

        if (existingItem.Username != username)
        {
            throw new UnauthorizedAccessException("You do not have permission to update this item");
        }



        item.Id = id;

        existingItem.Name = item.Name;
        existingItem.IsSharedWithOrganization = item.IsSharedWithOrganization;
        existingItem.Quantity = item.Quantity;

        await db.SaveChangesAsync();

        return existingItem;
    }

    public static async Task<bool> DeleteStarItem(ItemCacheDb db, int id, string username)
    {
        StarItem? existingItem = await db.PersonalItems.FindAsync(id);
        if (existingItem != null)
        {

            if (existingItem.Username != username)
            {
                throw new UnauthorizedAccessException("You do not have permission to update this item");
            }

            db.PersonalItems.Remove(existingItem);
            await db.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public static async Task<List<StarItem>> GetPersonalItems(ItemCacheDb db, string username)
    {
        List<StarItem> items = await db.PersonalItems.Where(user => user.Username.Equals(username)).ToListAsync();
        return items;
    }

    public static async Task<List<StarItem>> SearchPersonalItems(ItemCacheDb db, string username, string searchTerm)
    {
        List<StarItem> items = await db.PersonalItems.Where(item => item.Name.ToLower().Contains(searchTerm.ToLower()) && item.Username == username).Take(10).ToListAsync();
        return items;
    }
}