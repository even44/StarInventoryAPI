using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

public class ItemCacheDb : DbContext
{
    public ItemCacheDb(DbContextOptions<ItemCacheDb> options) : base(options) { }

    public DbSet<UexItem> CacheItems => Set<UexItem>();
    public DbSet<StarItem> PersonalItems => Set<StarItem>();
    public DbSet<StarLocation> StarLocations => Set<StarLocation>();
    public DbSet<UexCategory> UexCategories => Set<UexCategory>();
    public DbSet<UexItem> UexItems => Set<UexItem>();



    public async Task<bool> UpdateCategories(ItemCacheDb db, HttpClient client)
    {
        var responseTask = await client.GetAsync("https://api.uexcorp.uk/2.0/categories");
            var response = responseTask;
            Console.WriteLine($"UEX Categories Response: {response.StatusCode}");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Parsing UEX Categories Response");
                var categoriesResponse = await response.Content.ReadFromJsonAsync<UexCategoryResponse>();
                if (categoriesResponse != null)
                {
                    Console.WriteLine($"UEX Categories Parsed Response: {categoriesResponse.HttpCode} {categoriesResponse.Status} [{categoriesResponse.Message}]");
                    foreach (UexCategory category in categoriesResponse.Data)
                    {
                        Console.WriteLine($"Processing Category: {category.Id} {category.Name} {category.Section} {category.Type} {category.IsGameRelated} {category.IsMining} {category.DateAdded} {category.DateModified}");
                        var existingCategory = await db.UexCategories.FindAsync(category.Id);
                        if (existingCategory == null)
                        {
                            db.UexCategories.Add(category);
                            Console.WriteLine($"Added New Category: {category.Name}");
                        }
                        else
                        {
                            existingCategory.Type = category.Type;
                            existingCategory.Section = category.Section;
                            existingCategory.Name = category.Name;
                            existingCategory.IsGameRelated = category.IsGameRelated;
                            existingCategory.IsMining = category.IsMining;
                            existingCategory.DateAdded = category.DateAdded;
                            existingCategory.DateModified = category.DateModified;
                        }
                    }
                await SaveChangesAsync();
                Console.WriteLine("UEX Categories Updated Successfully");
                return true;
                }
            Console.WriteLine("UEX Categories Parsing Failed");
            }
        Console.WriteLine("UEX Categories Update Failed");
        return false;
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
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; set; }
    [JsonPropertyName("section")]
    public string Section { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("is_game_related")]
    public int IsGameRelated { get; set; }
    [JsonPropertyName("is_mining")]
    public int IsMining { get; set; }
    [JsonPropertyName("date_added")]
    public int DateAdded { get; set; }
    [JsonPropertyName("date_modified")]
    public int DateModified { get; set; }
}


public class UexCategoryResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; }
    [JsonPropertyName("http_code")]
    public int HttpCode { get; set; }
    [JsonPropertyName("data")]
    public List<UexCategory> Data { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; }
}
