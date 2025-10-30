using Microsoft.EntityFrameworkCore;

public class ItemCacheDb : DbContext
{
    public ItemCacheDb(DbContextOptions<ItemCacheDb> options) : base(options) { }

    public DbSet<UexItem> CacheItems => Set<UexItem>();
    public DbSet<StarItem> PersonalItems => Set<StarItem>();
    public DbSet<StarLocation> StarLocations => Set<StarLocation>();

}


public class UexItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
}
