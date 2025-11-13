
using Microsoft.EntityFrameworkCore;

public static class CacheDataStore
{
    public static async Task<List<StarLocation>> GetStarLocations(ItemCacheDb db)
    {
        List<StarLocation> locations = await db.StarLocations.ToListAsync();
        return locations;
    }

    public static async Task<List<StarLocation>> SearchStarLocations(ItemCacheDb db, string searchTerm)
    {
        List<StarLocation> locations = await db.StarLocations.Where(loc => loc.Name.ToLower().Contains(searchTerm.ToLower())).Take(10).ToListAsync();
        return locations;
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

    public static async Task<List<UexItem>> SearchUexItems(ItemCacheDb db, string searchTerm)
    {
        List<UexItem> items = await db.UexItems.Where(item => item.Name.ToLower().Contains(searchTerm.ToLower())).Take(10).ToListAsync();
        return items;
    }

    public static async Task<List<UexSpaceStation>> GetStarSpaceStations(ItemCacheDb db)
    {
        List<UexSpaceStation> stations = await db.UexSpaceStations.ToListAsync();
        return stations;
    }
}