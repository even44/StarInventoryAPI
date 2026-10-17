using Microsoft.EntityFrameworkCore;

namespace StarInventoryAPI.Db;

public class ItemCacheDb : DbContext
{
    public ItemCacheDb(DbContextOptions<ItemCacheDb> options) : base(options)
    {
    }

    public DbSet<UexItem> CacheItems => Set<UexItem>();
    public DbSet<StarItem> PersonalItems => Set<StarItem>();
    public DbSet<StarLocation> StarLocations => Set<StarLocation>();
    public DbSet<UexCategory> UexCategories => Set<UexCategory>();
    public DbSet<UexItem> UexItems => Set<UexItem>();
    public DbSet<UexPoi> UexPois => Set<UexPoi>();
    public DbSet<UexCity> UexCities => Set<UexCity>();
    public DbSet<UexSpaceStation> UexSpaceStations => Set<UexSpaceStation>();
    public DbSet<User> Users => Set<User>();
    public DbSet<OrgInventoryUser> OrgInventoryUsers => Set<OrgInventoryUser>();
    public DbSet<Recipe> Recipes => Set<Recipe>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StarLocation>()
            .HasIndex(l => new { l.SourceType, l.SourceId })
            .IsUnique();
    }
}
