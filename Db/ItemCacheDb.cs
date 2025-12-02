
using Microsoft.EntityFrameworkCore;

public class ItemCacheDb : DbContext
{
    public ItemCacheDb(DbContextOptions<ItemCacheDb> options) : base(options) { }

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

    public async Task<bool> UpdatePois(ItemCacheDb db, HttpClient client)
    {
        var responseTask = await client.GetAsync("https://api.uexcorp.uk/2.0/poi");
        var response = responseTask;
        Console.WriteLine($"UEX Locations Response: {response.StatusCode}");
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Parsing UEX Locations Response");
            var locations = await response.Content.ReadFromJsonAsync<UexLocationsResponse>();
            if (locations != null)
            {
                Console.WriteLine($"UEX Locations Parsed Response: {locations.Data.Count} locations");
                foreach (UexPoi location in locations.Data)
                {
                    Console.WriteLine($"Processing Location: {location.Id} {location.Name}");
                    var existingLocation = await db.UexPois.FindAsync(location.Id);
                    if (existingLocation == null)
                    {
                        db.UexPois.Add(location);
                        Console.WriteLine($"Added New Location: {location.Name}");
                    }
                    else
                    {
                        existingLocation.StarSystemId = location.StarSystemId;
                        existingLocation.PlanetId = location.PlanetId;
                        existingLocation.OrbitId = location.OrbitId;
                        existingLocation.MoonId = location.MoonId;
                        existingLocation.SpaceStationId = location.SpaceStationId;
                        existingLocation.CityId = location.CityId;
                        existingLocation.OutpostId = location.OutpostId;
                        existingLocation.FactionId = location.FactionId;
                        existingLocation.JurisdictionId = location.JurisdictionId;
                        existingLocation.Name = location.Name;
                        existingLocation.Nickname = location.Nickname;
                        existingLocation.IsAvailable = location.IsAvailable;
                        existingLocation.IsAvailableLive = location.IsAvailableLive;
                        existingLocation.IsVisible = location.IsVisible;
                        existingLocation.IsDefault = location.IsDefault;
                        existingLocation.IsMonitored = location.IsMonitored;
                        existingLocation.IsArmistice = location.IsArmistice;
                        existingLocation.IsLandable = location.IsLandable;
                        existingLocation.IsDecommissioned = location.IsDecommissioned;
                        existingLocation.HasQuantumMarker = location.HasQuantumMarker;
                        existingLocation.HasTradeTerminal = location.HasTradeTerminal;
                        existingLocation.HasHabitation = location.HasHabitation;
                        existingLocation.HasRefinery = location.HasRefinery;
                        existingLocation.HasCargoCenter = location.HasCargoCenter;
                        existingLocation.HasClinic = location.HasClinic;
                        existingLocation.HasFood = location.HasFood;
                        existingLocation.HasShops = location.HasShops;
                        existingLocation.HasRefuel = location.HasRefuel;
                        existingLocation.HasRepair = location.HasRepair;
                        existingLocation.HasGravity = location.HasGravity;
                        existingLocation.HasLoadingDock = location.HasLoadingDock;
                        existingLocation.HasDockingPort = location.HasDockingPort;
                        existingLocation.HasFreightElevator = location.HasFreightElevator;
                        existingLocation.PadTypes = location.PadTypes;
                        existingLocation.DateAdded = location.DateAdded;
                        existingLocation.DateModified = location.DateModified;
                        existingLocation.StarSystemName = location.StarSystemName;
                        existingLocation.PlanetName = location.PlanetName;
                        existingLocation.OrbitName = location.OrbitName;
                        existingLocation.MoonName = location.MoonName;
                        existingLocation.SpaceStationName = location.SpaceStationName;
                        existingLocation.OutpostName = location.OutpostName;
                        existingLocation.CityName = location.CityName;
                        existingLocation.FactionName = location.FactionName;
                        existingLocation.JurisdictionName = location.JurisdictionName;
                    }
                }
                await SaveChangesAsync();
                Console.WriteLine("UEX Locations Updated Successfully");
                return true;
            }
            Console.WriteLine("UEX Locations Parsing Failed");
        }
        return false;
    }

    public async Task<bool> UpdateSpaceStations(ItemCacheDb db, HttpClient client)
    {
        var responseTask = await client.GetAsync("https://api.uexcorp.uk/2.0/space_stations");
        var response = responseTask;
        Console.WriteLine($"UEX Space Stations Response: {response.StatusCode}");
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Parsing UEX Space Stations Response");
            var stationsResponse = await response.Content.ReadFromJsonAsync<UexSpaceStationResponse>();
            if (stationsResponse != null)
            {
                Console.WriteLine($"UEX Space Stations Parsed Response: {stationsResponse.HttpCode} {stationsResponse.Status} [{stationsResponse.Message}]");
                foreach (UexSpaceStation station in stationsResponse.Data)
                {
                    Console.WriteLine($"Processing Space Station: {station.Id} {station.Name}");
                    var existingStation = await db.UexSpaceStations.FindAsync(station.Id);
                    if (existingStation == null)
                    {
                        db.UexSpaceStations.Add(station);
                        Console.WriteLine($"Added New Space Station: {station.Name}");
                    }
                    else
                    {
                        // Update all properties as needed
                        existingStation.StarSystemId = station.StarSystemId;
                        existingStation.PlanetId = station.PlanetId;
                        existingStation.OrbitId = station.OrbitId;
                        existingStation.MoonId = station.MoonId;
                        existingStation.CityId = station.CityId;
                        existingStation.FactionId = station.FactionId;
                        existingStation.JurisdictionId = station.JurisdictionId;
                        existingStation.Name = station.Name;
                        existingStation.Nickname = station.Nickname;
                        existingStation.IsAvailable = station.IsAvailable;
                        existingStation.IsAvailableLive = station.IsAvailableLive;
                        existingStation.IsVisible = station.IsVisible;
                        existingStation.IsDefault = station.IsDefault;
                        existingStation.IsMonitored = station.IsMonitored;
                        existingStation.IsArmistice = station.IsArmistice;
                        existingStation.IsLandable = station.IsLandable;
                        existingStation.IsDecommissioned = station.IsDecommissioned;
                        existingStation.IsLagrange = station.IsLagrange;
                        existingStation.IsJumpPoint = station.IsJumpPoint;
                        existingStation.HasQuantumMarker = station.HasQuantumMarker;
                        existingStation.HasTradeTerminal = station.HasTradeTerminal;
                        existingStation.HasHabitation = station.HasHabitation;
                        existingStation.HasRefinery = station.HasRefinery;
                        existingStation.HasCargoCenter = station.HasCargoCenter;
                        existingStation.HasClinic = station.HasClinic;
                        existingStation.HasFood = station.HasFood;
                        existingStation.HasShops = station.HasShops;
                        existingStation.HasRefuel = station.HasRefuel;
                        existingStation.HasRepair = station.HasRepair;
                        existingStation.HasGravity = station.HasGravity;
                        existingStation.HasLoadingDock = station.HasLoadingDock;
                        existingStation.HasDockingPort = station.HasDockingPort;
                        existingStation.HasFreightElevator = station.HasFreightElevator;
                        existingStation.PadTypes = station.PadTypes;
                        existingStation.DateAdded = station.DateAdded;
                        existingStation.DateModified = station.DateModified;
                        existingStation.StarSystemName = station.StarSystemName;
                        existingStation.PlanetName = station.PlanetName;
                        existingStation.OrbitName = station.OrbitName;
                        existingStation.CityName = station.CityName;
                        existingStation.FactionName = station.FactionName;
                        existingStation.JurisdictionName = station.JurisdictionName;
                    }
                }
                await SaveChangesAsync();
                Console.WriteLine("UEX Space Stations Updated Successfully");
                return true;
            }
            Console.WriteLine("UEX Space Stations Parsing Failed");
        }
        Console.WriteLine("UEX Space Stations Update Failed");
        return false;
    }

    public async Task<bool> UpdateCities(ItemCacheDb db, HttpClient client)
    {
        var responseTask = await client.GetAsync("https://api.uexcorp.uk/2.0/cities");
        var response = responseTask;
        if (response.IsSuccessStatusCode)
        {
            var citiesResponse = await response.Content.ReadFromJsonAsync<UexCityResponse>();
            if (citiesResponse != null)
            {
                foreach (UexCity city in citiesResponse.Data)
                {
                    var existingCity = await db.UexCities.FindAsync(city.Id);
                    if (existingCity == null)
                    {
                        db.UexCities.Add(city);
                    }
                    else
                    {
                        existingCity = city;
                    }
                    
                }
                await SaveChangesAsync();
                return true;
            }
        }
        return false;
    }

    public async Task<bool> CompileLocations(ItemCacheDb db)
    {
        // Get total count of Pois
        int totalPois = await db.UexPois.CountAsync();
        // Get all Pois
        List<UexPoi> pois = await db.UexPois.ToListAsync();

        // Get total count of Space Stations
        int totalStations = await db.UexSpaceStations.CountAsync();
        // Get all Space Stations
        List<UexSpaceStation> stations = await db.UexSpaceStations.ToListAsync();

        // Get total count of Space Stations
        int totalCities = await db.UexCities.CountAsync();
        // Get all Space Stations
        List<UexCity> cities = await db.UexCities.ToListAsync();
        int i = 0;

        foreach (UexPoi poi in pois)
        {
            StarLocation? existingLocation = await db.StarLocations.FindAsync(i);
            if (existingLocation == null)
            {
                StarLocation newLocation = new StarLocation
                {
                    Id = i,
                    Name = poi.Name,
                };
                db.StarLocations.Add(newLocation);
            }
            else
            {
                existingLocation.Name = poi.Name;
            }
            i++;
        }

        foreach (UexSpaceStation station in stations)
        {
            StarLocation? existingLocation = await db.StarLocations.FindAsync(i);
            if (existingLocation == null)
            {
                StarLocation newLocation = new StarLocation
                {
                    Id = i,
                    Name = station.Name,
                };
                db.StarLocations.Add(newLocation);
            }
            else
            {
                existingLocation.Name = station.Name;
            }
            i++;
        }

        foreach (UexCity city in cities)
        {
            StarLocation? existingLocation = await db.StarLocations.FindAsync(i);
            if (existingLocation == null)
            {
                StarLocation newLocation = new StarLocation
                {
                    Id = i,
                    Name = city.Name,
                };
                db.StarLocations.Add(newLocation);
            }
            else
            {
                existingLocation.Name = city.Name;
            }
            i++;
        }
        await SaveChangesAsync();


        if (db.StarLocations.Count() != (totalPois + totalStations + totalCities))
        {
            Console.WriteLine($"Location count mismatch! {db.StarLocations.Count()} != {totalPois} + {totalStations} + {totalCities} i={i}");
            return false;
        }
        Console.WriteLine($"Compiled {totalPois} + {totalStations} + {totalCities} = {totalPois+totalStations+totalCities} locations");
        return true;
    }

    public async Task<bool> UpdateItems(ItemCacheDb db, HttpClient client){

        List<UexCategory> categories = await db.UexCategories.Where(c => c.Type.Equals("item")).ToListAsync();

        foreach (UexCategory category in categories)
        {
            if (category.Id < 12) continue;
            //Get items from category
            var responseTask = await client.GetAsync($"https://api.uexcorp.uk/2.0/items?id_category={category.Id}");
            var response = responseTask;
            Console.WriteLine($"UEX Items Response for Category {category.Id}: {response.StatusCode}");
            if (response.IsSuccessStatusCode){
                Console.WriteLine($"Parsing UEX Items Response for Category {category.Id}");
                var itemsResponse = await response.Content.ReadFromJsonAsync<UexItemsResponse>();
                if (itemsResponse != null)
                {
                    Console.WriteLine($"UEX Items Parsed Response for Category {category.Id}: {itemsResponse.HttpCode} {itemsResponse.Status} [{itemsResponse.Message}]");
                    if (itemsResponse.Data == null) continue;
                    foreach (UexItem item in itemsResponse.Data)
                    {
                        item.Name = FixWierdHTMLThing(item.Name);
                        Console.WriteLine($"Processing Item: {item.Id} {item.Name}");
                        var existingItem = await db.UexItems.FindAsync(item.Id);
                        if (existingItem == null)
                        {
                            db.UexItems.Add(item);
                            Console.WriteLine($"Added New Item: {item.Name}");
                        }
                        else
                        {
                            existingItem.parentId = item.parentId;
                            existingItem.categoryId = item.categoryId;
                            existingItem.vehicleId = item.vehicleId;
                            existingItem.Name = item.Name;
                            existingItem.Section = item.Section;
                            existingItem.Category = item.Category;
                            existingItem.CompanyName = item.CompanyName;
                            existingItem.VehicleName = item.VehicleName;
                            existingItem.Slug = item.Slug;
                            existingItem.Size = item.Size;
                            existingItem.Uuid = item.Uuid;
                            existingItem.StoreUrl = item.StoreUrl;
                            existingItem.is_exclusive_pledge = item.is_exclusive_pledge;
                            existingItem.is_exclusive_subscriber = item.is_exclusive_subscriber;
                            existingItem.is_exclusive_concierge = item.is_exclusive_concierge;
                            existingItem.is_commodity = item.is_commodity;
                            existingItem.is_harvestable = item.is_harvestable;
                            existingItem.Notification = item.Notification;
                            existingItem.GameVersion = item.GameVersion;
                            existingItem.DateAdded = item.DateAdded;
                            existingItem.DateModified = item.DateModified;
                        }
                    }
                    await SaveChangesAsync();
                    Console.WriteLine("UEX Items Updated Successfully");
                }
                else
                {
                    Console.WriteLine("UEX Items Parsing Failed");
                }
            }
            else
            {
                Console.WriteLine("UEX Items Update Failed");
            }
        }
        
        return true;
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


    public static string FixQuotes(string dirtyName)
    {
        var name = dirtyName.Replace("&quot;", "\"");
        return name;
    }

    public static string FixApos(string dirtyName)
    {
        var name = dirtyName.Replace("&apos;", "\'");
        return name;
    }

    public static string FixAmp(string dirtyName)
    {
        var name = dirtyName.Replace("&amp;", String.Empty);
        return name;
    }

    public static string FixWierdHTMLThing(string dirtyName)
    {
        var name = FixApos(dirtyName);
        name = FixQuotes(name);
        name = FixAmp(name);
        return name;
    }
}


