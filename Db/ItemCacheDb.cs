using Microsoft.EntityFrameworkCore;

public class ItemCacheDb : DbContext
{
    public ItemCacheDb(DbContextOptions<ItemCacheDb> options) : base(options) { }

    public DbSet<UexItem> CacheItems => Set<UexItem>();
    public DbSet<UexCategory> CacheCategories => Set<UexCategory>();
    public DbSet<StarItem> PersonalItems => Set<StarItem>();
    public DbSet<StarLocation> StarLocations => Set<StarLocation>();



    public void UpdateCategories(HttpClient client)
    {

    }

}

/*

UEX ITEM MODEL
id int // route ID, may change during website updates
id_parent int
id_category int
id_company int
id_vehicle int // if linked to a vehicle
name string
section string|null // coming from categories
category string|null // coming from categories
company_name string|null // coming from companies
vehicle_name string|null // coming from vehicles
slug string // UEX URLs
size string|null
uuid string|null // star citizen uuid
url_store string|null // pledge store URL
is_exclusive_pledge int
is_exclusive_subscriber int
is_exclusive_concierge int
is_commodity int
is_harvestable int
notification json // heads up about an item, such as known bugs, etc.
game_version string
date_added int // timestamp
date_modified int // timestamp
*/

public class UexItem
{
    public int Id { get; set; }
    public int parentId { get; set; }
    public int categoryId { get; set; }
    public int vehicleId { get; set; }
    public string Name { get; set; }
    public string? Section { get; set; }
    public string? Category { get; set; }
    public string? CompanyName { get; set; }
    public string? VehicleName { get; set; }
    public string Slug { get; set; }
    public string? Size { get; set; }
    public string? uuid { get; set; }
    public string? StoreUrl { get; set; }
    public bool is_exclusive_pledge { get; set; }
    public bool is_exclusive_subscriber { get; set; }
    public bool is_exclusive_concierge { get; set; }
    public bool is_commodity { get; set; }
    public bool is_harvestable { get; set; }
    public string? Notification { get; set; }
    public string GameVersion { get; set; }
    public int DateAdded { get; set; }
    public int DateModified { get; set; }
}

/*

UEX CATEGORY MODEL
id int
type string // item, service, contract
section string
name string
is_game_related int // if exists in-game
is_mining int // mining related
date_added int // timestamp
date_modified int // timestamp
*/


public class UexCategory
{
    public int Id { get; set; }
    public string Type { get; set; }
    public string Section { get; set; }
    public string Name { get; set; }
    public bool IsGameRelated { get; set; }
    public bool IsMining { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime DateModified { get; set; }
}
