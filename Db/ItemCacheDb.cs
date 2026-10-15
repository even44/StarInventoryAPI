using Microsoft.EntityFrameworkCore;

namespace StarInventoryAPI.Db;

public record CategoryDto(int Id, string Name);

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


    public async Task<List<UexCategory>> UpdateCategories(ItemCacheDb db, HttpClient client)
    {
        var response = await client.GetAsync("https://api.uexcorp.uk/2.0/categories");
        Console.WriteLine($"UEX Categories Response: {response.StatusCode}");
        if (response.IsSuccessStatusCode)
        {
            var categoriesResponse = await response.Content.ReadFromJsonAsync<UexCategoryResponse>();
            if (categoriesResponse?.Data != null)
            {
                var incomingCategories = categoriesResponse.Data;
                var ids = incomingCategories.Select(c => c.Id).ToList();
                var existingCategories = await db.UexCategories
                    .Where(c => ids.Contains(c.Id))
                    .ToDictionaryAsync(c => c.Id);

                foreach (var category in incomingCategories)
                {
                    if (existingCategories.TryGetValue(category.Id, out var existingCategory))
                    {
                        db.Entry(existingCategory).CurrentValues.SetValues(category);
                    }
                    else
                    {
                        db.UexCategories.Add(category);
                    }
                }

                await db.SaveChangesAsync();
                Console.WriteLine($"UEX Categories Updated Successfully: {incomingCategories.Count} categories");
                return await db.UexCategories.AsNoTracking().ToListAsync();
            }

            Console.WriteLine("UEX Categories Parsing Failed");
        }

        Console.WriteLine("UEX Categories Update Failed");
        return [];
    }

    public async Task<bool> UpdatePois(ItemCacheDb db, HttpClient client)
    {
        var response = await client.GetAsync("https://api.uexcorp.uk/2.0/poi");
        Console.WriteLine($"UEX Locations Response: {response.StatusCode}");
        if (response.IsSuccessStatusCode)
        {
            var locations = await response.Content.ReadFromJsonAsync<UexLocationsResponse>();
            if (locations?.Data != null)
            {
                var incomingPois = locations.Data;
                var ids = incomingPois.Select(l => l.Id).ToList();
                var existingPois = await db.UexPois
                    .Where(p => ids.Contains(p.Id))
                    .ToDictionaryAsync(p => p.Id);

                foreach (var location in incomingPois)
                {
                    if (existingPois.TryGetValue(location.Id, out var existingLocation))
                    {
                        db.Entry(existingLocation).CurrentValues.SetValues(location);
                    }
                    else
                    {
                        db.UexPois.Add(location);
                    }
                }

                await db.SaveChangesAsync();
                Console.WriteLine($"UEX Locations Updated Successfully: {incomingPois.Count} locations");
                return true;
            }

            Console.WriteLine("UEX Locations Parsing Failed");
        }

        return false;
    }

    public async Task<bool> UpdateSpaceStations(ItemCacheDb db, HttpClient client)
    {
        var response = await client.GetAsync("https://api.uexcorp.uk/2.0/space_stations");
        Console.WriteLine($"UEX Space Stations Response: {response.StatusCode}");
        if (response.IsSuccessStatusCode)
        {
            var stationsResponse = await response.Content.ReadFromJsonAsync<UexSpaceStationResponse>();
            if (stationsResponse?.Data != null)
            {
                var incomingStations = stationsResponse.Data;
                var ids = incomingStations.Select(s => s.Id).ToList();
                var existingStations = await db.UexSpaceStations
                    .Where(s => ids.Contains(s.Id))
                    .ToDictionaryAsync(s => s.Id);

                foreach (var station in incomingStations)
                {
                    if (existingStations.TryGetValue(station.Id, out var existingStation))
                    {
                        db.Entry(existingStation).CurrentValues.SetValues(station);
                    }
                    else
                    {
                        db.UexSpaceStations.Add(station);
                    }
                }

                await db.SaveChangesAsync();
                Console.WriteLine($"UEX Space Stations Updated Successfully: {incomingStations.Count} stations");
                return true;
            }

            Console.WriteLine("UEX Space Stations Parsing Failed");
        }

        Console.WriteLine("UEX Space Stations Update Failed");
        return false;
    }

    public async Task<bool> UpdateCities(ItemCacheDb db, HttpClient client)
    {
        var response = await client.GetAsync("https://api.uexcorp.uk/2.0/cities");
        if (response.IsSuccessStatusCode)
        {
            var citiesResponse = await response.Content.ReadFromJsonAsync<UexCityResponse>();
            if (citiesResponse?.Data != null)
            {
                var incomingCities = citiesResponse.Data;
                var ids = incomingCities.Select(c => c.Id).ToList();
                var existingCities = await db.UexCities
                    .Where(c => ids.Contains(c.Id))
                    .ToDictionaryAsync(c => c.Id);

                foreach (var city in incomingCities)
                {
                    if (existingCities.TryGetValue(city.Id, out var existingCity))
                    {
                        db.Entry(existingCity).CurrentValues.SetValues(city);
                    }
                    else
                    {
                        db.UexCities.Add(city);
                    }
                }

                await db.SaveChangesAsync();
                Console.WriteLine($"UEX Cities Updated Successfully: {incomingCities.Count} cities");
                return true;
            }
        }

        return false;
    }

    public async Task<bool> CompileLocations(ItemCacheDb db)
    {
        var pois = await db.UexPois.AsNoTracking().ToListAsync();
        var stations = await db.UexSpaceStations.AsNoTracking().ToListAsync();
        var cities = await db.UexCities.AsNoTracking().ToListAsync();

        var existingLocations = await db.StarLocations.ToDictionaryAsync(l => l.Id);
        int i = 0;

        void AddOrUpdateLocation(string name)
        {
            if (existingLocations.TryGetValue(i, out var existingLocation))
            {
                existingLocation.Name = name;
            }
            else
            {
                db.StarLocations.Add(new StarLocation
                {
                    Id = i,
                    Name = name,
                });
            }

            i++;
        }

        foreach (var poi in pois)
        {
            AddOrUpdateLocation(poi.Name);
        }

        foreach (var station in stations)
        {
            AddOrUpdateLocation(station.Name);
        }

        foreach (var city in cities)
        {
            AddOrUpdateLocation(city.Name);
        }

        var stale = existingLocations.Values.Where(l => l.Id >= i).ToList();
        if (stale.Count > 0)
        {
            db.StarLocations.RemoveRange(stale);
        }

        await db.SaveChangesAsync();

        int totalExpected = pois.Count + stations.Count + cities.Count;
        int totalActual = await db.StarLocations.CountAsync();
        if (totalActual != totalExpected)
        {
            Console.WriteLine(
                $"Location count mismatch! {totalActual} != {totalExpected} i={i}");
            return false;
        }

        Console.WriteLine(
            $"Compiled {pois.Count} + {stations.Count} + {cities.Count} = {totalActual} locations");
        return true;
    }

    public async Task<bool> UpdateItemsFromCategory(UexCategory cat, ItemCacheDb db, HttpClient client)
    {
        if (cat.Name.Equals("Commodities"))
        {
            var commodityItems = await GetCommodities(db, client);
            if (commodityItems.Count > 0)
            {
                var commodityIds = commodityItems.Select(c => c.Id).ToList();
                var existingCommodities = await db.UexItems
                    .Where(i => commodityIds.Contains(i.Id))
                    .ToDictionaryAsync(i => i.Id);

                foreach (var commodity in commodityItems)
                {
                    if (existingCommodities.TryGetValue(commodity.Id, out var existing))
                    {
                        db.Entry(existing).CurrentValues.SetValues(commodity);
                    }
                    else
                    {
                        db.UexItems.Add(commodity);
                    }
                }

                await db.SaveChangesAsync();
            }
        }

        var response = await client.GetAsync($"https://api.uexcorp.uk/2.0/items?id_category={cat.Id}");
        Console.WriteLine($"UEX Items Response for Category {cat.Id}: {response.StatusCode}");
        if (response.IsSuccessStatusCode)
        {
            var itemsResponse = await response.Content.ReadFromJsonAsync<UexItemsResponse>();
            if (itemsResponse?.Data != null && itemsResponse.Data.Count > 0)
            {
                var incomingItems = itemsResponse.Data;
                foreach (var item in incomingItems)
                {
                    item.Name = RemoveHtmlArtifactsFromString(item.Name);
                }

                var itemIds = incomingItems.Select(i => i.Id).ToList();
                var existingItems = await db.UexItems
                    .Where(i => itemIds.Contains(i.Id))
                    .ToDictionaryAsync(i => i.Id);

                foreach (var item in incomingItems)
                {
                    if (existingItems.TryGetValue(item.Id, out var existingItem))
                    {
                        db.Entry(existingItem).CurrentValues.SetValues(item);
                    }
                    else
                    {
                        db.UexItems.Add(item);
                    }
                }

                await db.SaveChangesAsync();
                Console.WriteLine($"UEX Items for Category {cat.Id} ({cat.Name}) Updated Successfully: {incomingItems.Count} items");
            }

            return true;
        }

        Console.WriteLine($"UEX Items Update Failed for Category {cat.Id}");
        return false;
    }

    public async Task<List<UexItem>> GetCommodities(ItemCacheDb db, HttpClient client)
    {
        List<UexItem> commodityItems = [];
        var response = await client.GetAsync("https://api.uexcorp.uk/2.0/commodities");
        if (response.IsSuccessStatusCode)
        {
            var itemsResponse = await response.Content.ReadFromJsonAsync<UexCommodityResponse>();
            if (itemsResponse?.Data != null)
            {
                foreach (UexCommodity commodity in itemsResponse.Data)
                {
                    var commodityItem = new UexItem
                    {
                        Id = commodity.Id + 10000,
                        Name = commodity.Name,
                        Slug = commodity.Code,
                        categoryId = 36,
                        DateAdded = commodity.DateAdded,
                        DateModified = commodity.DateModified,
                    };
                    commodityItems.Add(commodityItem);
                }
            }
        }

        return commodityItems;
    }


    public async Task<CacheUpdateResponse> GetCacheUpdateResponse(ItemCacheDb db)
    {
        var totalItems = await db.UexItems.CountAsync();
        var totalPois = await db.UexPois.CountAsync();
        var totalStations = await db.UexSpaceStations.CountAsync();
        var totalCities = await db.UexCities.CountAsync();

        return new CacheUpdateResponse
        {
            TotalItems = totalItems,
            TotalPois = totalPois,
            TotalStations = totalStations,
            TotalCities = totalCities,
            Total = totalItems + totalPois + totalCities + totalStations
        };
    }


    private static string FixQuotes(string dirtyName)
    {
        var name = dirtyName.Replace("&quot;", "\"");
        return name;
    }

    private static string FixAmp(string dirtyName)
    {
        var name = dirtyName.Replace("&amp;", String.Empty);
        return name;
    }

    private static string FixApos(string dirtyName)
    {
        var name = dirtyName.Replace("&apos;", "\'");
        return name;
    }

    private static string RemoveHtmlArtifactsFromString(string dirtyName)
    {
        var name = FixQuotes(dirtyName);
        name = FixAmp(name);
        name = FixApos(name);
        return name;
    }
}