using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

public static class PersonalInventoryDataStore
{
    public static async Task<StarItem?> AddStarItem(ItemCacheDb db, StarItem item, string username)
    {
        await UserDataStore.ensureUserExists(db, username);

        item.Username = username;


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
        await UserDataStore.ensureUserExists(db, username);
        item.Id = id;
        StarItem? existingItem = await db.PersonalItems.FindAsync(id);
        if (existingItem == null)
        {
            throw new KeyNotFoundException("Item not found");
        }

        if (existingItem.Username != username)
        {
            throw new UnauthorizedAccessException("You do not have permission to update this item");
        }


      

        // Merge if location change
        if (existingItem.LocationId != item.LocationId)
        {
            var existingItemAtNewLocation = await db.PersonalItems.Where(i => i.LocationId == item.LocationId && i.UexIdentifier == item.UexIdentifier).FirstOrDefaultAsync();
            if (existingItemAtNewLocation == null)
            {
                existingItem.LocationId = item.LocationId;
                existingItem.IsSharedWithOrganization = item.IsSharedWithOrganization;
                existingItem.Quantity = item.Quantity;
            } else
            {

                existingItemAtNewLocation.Quantity += item.Quantity;
                existingItemAtNewLocation.IsSharedWithOrganization = existingItemAtNewLocation.IsSharedWithOrganization || item.IsSharedWithOrganization;


                existingItem.Quantity -= item.Quantity;
                if (existingItem.Quantity <= 0)
                {
                    db.PersonalItems.Remove(existingItem);
                }

                await db.SaveChangesAsync();
                return existingItemAtNewLocation;
            }
        } else
        {
            existingItem.IsSharedWithOrganization = item.IsSharedWithOrganization;
            existingItem.Quantity = item.Quantity;
            
        }

        await db.SaveChangesAsync();
        return existingItem;
        

        
    }

    public static async Task<bool> DeleteStarItem(ItemCacheDb db, int id, string username)
    {
        await UserDataStore.ensureUserExists(db, username);
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
        await UserDataStore.ensureUserExists(db, username);
        List<StarItem> items = await db.PersonalItems.Where(user => user.Username.Equals(username)).ToListAsync();
        return items;
    }

    public static async Task<List<StarItem>> SearchPersonalItems(ItemCacheDb db, string username, string searchTerm)
    {
        await UserDataStore.ensureUserExists(db, username);
        List<StarItem> items = await db.PersonalItems.Where(item => item.Name.ToLower().Contains(searchTerm.ToLower()) && item.Username == username).Take(10).ToListAsync();
        return items;
    }


    
}