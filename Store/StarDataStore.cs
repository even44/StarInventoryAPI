


using Microsoft.EntityFrameworkCore;

public static class StarDataStore
{


    public static async Task<StarItem?> AddStarItem(ItemCacheDb db, StarItem item, string username)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
        {
            Console.WriteLine($"Invalid User when adding item");
            return null;
        } else
        {
            item.Username = username;
        }


        var location = await db.StarLocations.FirstOrDefaultAsync(loc => loc.Id == item.LocationId);
        if (location == null)
        {
            Console.WriteLine($"Invalid Location when adding item");
            return null;
        }

        var existingItem = await db.PersonalItems.FirstOrDefaultAsync(i => i.Name == item.Name && i.LocationId == item.LocationId);
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

    public static async Task<List<UexPoi>> GetUexPois(ItemCacheDb db)
    {
        List<UexPoi> pois = await db.UexPois.ToListAsync();
        return pois;
    }

    public static async Task<List<UexCategory>> GetUexCategories(ItemCacheDb db)
    {
        List<UexCategory> categories = await db.UexCategories.ToListAsync();
        return categories;
    }

    public static async Task<List<UexItem>> GetUexItems(ItemCacheDb db)
    {
        List<UexItem> items = await db.UexItems.ToListAsync();
        return items;
    }

    public static async Task<List<UexSpaceStation>> GetStarSpaceStations(ItemCacheDb db)
    {
        List<UexSpaceStation> stations = await db.UexSpaceStations.ToListAsync();
        return stations;
    }


    // CACHE METHODS


}