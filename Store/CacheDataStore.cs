
using Microsoft.EntityFrameworkCore;
using StarInventoryAPI.Db;

namespace StarInventoryAPI.Store;

public static class CacheDataStore
{
    public static async Task<List<StarLocation>> GetStarLocations(ItemCacheDb db)
    {
        var locations = await db.StarLocations.ToListAsync();
        return locations;
    }

    public static async Task<List<StarLocation>> SearchStarLocations(ItemCacheDb db, string searchTerm)
    {
        var locations = await db.StarLocations.Where(loc => loc.Name.ToLower().Contains(searchTerm.ToLower())).Take(10).ToListAsync();
        return locations;
    }

    public static async Task<List<UexPoi>> GetUexPois(ItemCacheDb db)
    {
        var pois = await db.UexPois.ToListAsync();
        return pois;
    }

    public static async Task<List<UexCategory>> GetUexCategories(ItemCacheDb db)
    {
        var categories = await db.UexCategories.ToListAsync();
        return categories;
    }

    public static async Task<List<UexItem>> GetUexItems(ItemCacheDb db)
    {
        var items = await db.UexItems.ToListAsync();
        return items;
    }

    public static async Task<List<UexItem>> SearchUexItems(ItemCacheDb db, string searchTerm)
    {
        var items = await db.UexItems.Where(item => item.Name.ToLower().Contains(searchTerm.ToLower())).Take(10).ToListAsync();
        return items;
    }

    public static async Task<List<UexSpaceStation>> GetStarSpaceStations(ItemCacheDb db)
    {
        var stations = await db.UexSpaceStations.ToListAsync();
        return stations;
    }
    
    public static async Task<List<UexItem>> GetItemsFromCategory(int categoryId, ItemCacheDb db)
    {
        return await db.UexItems.Where(item => item.categoryId == categoryId).ToListAsync();
    }
}