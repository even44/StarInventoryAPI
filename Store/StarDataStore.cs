


public static class StarDataStore
{
    public static async Task<StarItem> AddStarItem(ItemCacheDb db, StarItem item)
    {
        db.PersonalItems.Add(item);
        await db.SaveChangesAsync();
        return item;
    }
}