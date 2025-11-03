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
    public DbSet<UexPoi> UexPois => Set<UexPoi>();
    public DbSet<UexSpaceStation> UexSpaceStations => Set<UexSpaceStation>();



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

    public async Task<bool> UpdateLocations(ItemCacheDb db, HttpClient client)
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

public class UexPoi
{
    /*
    Map Out these properties:
    id int
    id_star_system int
    id_planet int
    id_orbit int
    id_moon int
    id_space_station int
    id_city int
    id_outpost int
    id_faction int
    id_jurisdiction int
    name string
    nickname string
    is_available int // UEX
    is_available_live int // Star Citizen
    is_visible int // UEX (public)
    is_default int
    is_monitored int
    is_armistice int
    is_landable int
    is_decommissioned int
    has_quantum_marker int
    has_trade_terminal int
    has_habitation int
    has_refinery int
    has_cargo_center int
    has_clinic int
    has_food int
    has_shops int
    has_refuel int
    has_repair int
    has_gravity int
    has_loading_dock int
    has_docking_port int
    has_freight_elevator int
    pad_types string|null // XS|S|M|L|XL
    date_added int // timestamp
    date_modified int // timestamp
    star_system_name string|null
    planet_name string|null
    orbit_name string|null
    moon_name string|null
    space_station_name string|null
    outpost_name string|null
    city_name string|null
    faction_name string|null
    jurisdiction_name string|null
    */
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("id_star_system")]
    public int StarSystemId { get; set; }
    [JsonPropertyName("id_planet")]
    public int PlanetId { get; set; }
    [JsonPropertyName("id_orbit")]
    public int OrbitId { get; set; }
    [JsonPropertyName("id_moon")]
    public int MoonId { get; set; }
    [JsonPropertyName("id_space_station")]
    public int SpaceStationId { get; set; }
    [JsonPropertyName("id_city")]
    public int CityId { get; set; }
    [JsonPropertyName("id_outpost")]
    public int OutpostId { get; set; }
    [JsonPropertyName("id_faction")]
    public int FactionId { get; set; }
    [JsonPropertyName("id_jurisdiction")]
    public int JurisdictionId { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("nickname")]
    public string Nickname { get; set; }
    [JsonPropertyName("is_available")]
    public int IsAvailable { get; set; }
    [JsonPropertyName("is_available_live")]
    public int IsAvailableLive { get; set; }
    [JsonPropertyName("is_visible")]
    public int IsVisible { get; set; }
    [JsonPropertyName("is_default")]
    public int IsDefault { get; set; }
    [JsonPropertyName("is_monitored")]
    public int IsMonitored { get; set; }
    [JsonPropertyName("is_armistice")]
    public int IsArmistice { get; set; }
    [JsonPropertyName("is_landable")]
    public int IsLandable { get; set; }
    [JsonPropertyName("is_decommissioned")]
    public int IsDecommissioned { get; set; }
    [JsonPropertyName("has_quantum_marker")]
    public int HasQuantumMarker { get; set; }
    [JsonPropertyName("has_trade_terminal")]
    public int HasTradeTerminal { get; set; }
    [JsonPropertyName("has_habitation")]
    public int HasHabitation { get; set; }
    [JsonPropertyName("has_refinery")]
    public int HasRefinery { get; set; }
    [JsonPropertyName("has_cargo_center")]
    public int HasCargoCenter { get; set; }
    [JsonPropertyName("has_clinic")]
    public int HasClinic { get; set; }
    [JsonPropertyName("has_food")]
    public int HasFood { get; set; }
    [JsonPropertyName("has_shops")]
    public int HasShops { get; set; }
    [JsonPropertyName("has_refuel")]
    public int HasRefuel { get; set; }
    [JsonPropertyName("has_repair")]
    public int HasRepair { get; set; }
    [JsonPropertyName("has_gravity")]
    public int HasGravity { get; set; }
    [JsonPropertyName("has_loading_dock")]
    public int HasLoadingDock { get; set; }
    [JsonPropertyName("has_docking_port")]
    public int HasDockingPort { get; set; }
    [JsonPropertyName("has_freight_elevator")]
    public int HasFreightElevator { get; set; }
    [JsonPropertyName("pad_types")]
    public string? PadTypes { get; set; }
    [JsonPropertyName("date_added")]
    public int DateAdded { get; set; }
    [JsonPropertyName("date_modified")]
    public int DateModified { get; set; }
    [JsonPropertyName("star_system_name")]
    public string? StarSystemName { get; set; }
    [JsonPropertyName("planet_name")]
    public string? PlanetName { get; set; }
    [JsonPropertyName("orbit_name")]
    public string? OrbitName { get; set; }
    [JsonPropertyName("moon_name")]
    public string? MoonName { get; set; }
    [JsonPropertyName("space_station_name")]
    public string? SpaceStationName { get; set; }
    [JsonPropertyName("outpost_name")]
    public string? OutpostName { get; set; }
    [JsonPropertyName("city_name")]
    public string? CityName { get; set; }
    [JsonPropertyName("faction_name")]
    public string? FactionName { get; set; }
    [JsonPropertyName("jurisdiction_name")]
    public string? JurisdictionName { get; set; }

}

public class UexSpaceStation
{
    /*
    Implements these properties:
    id int
    id_star_system int
    id_planet int
    id_orbit int
    id_moon int
    id_city int // city next to space station
    id_faction int
    id_jurisdiction int
    name string
    nickname string // our nickname
    is_available int // UEX
    is_available_live int // Star Citizen
    is_visible int // UEX (public)
    is_default int
    is_monitored int
    is_armistice int
    is_landable int
    is_decommissioned int
    is_lagrange int
    is_jump_point int
    has_quantum_marker int
    has_trade_terminal int
    has_habitation int
    has_refinery int
    has_cargo_center int
    has_clinic int
    has_food int
    has_shops int
    has_refuel int
    has_repair int
    has_gravity int
    has_loading_dock int
    has_docking_port int
    has_freight_elevator int
    pad_types string|null // XS|S|M|L|XL
    date_added int // timestamp
    date_modified int // timestamp
    star_system_name string|null
    planet_name string|null
    orbit_name string|null
    city_name string|null
    faction_name string|null
    jurisdiction_name string|null
    */

    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("id_star_system")]
    public int StarSystemId { get; set; }
    [JsonPropertyName("id_planet")]
    public int PlanetId { get; set; }
    [JsonPropertyName("id_orbit")]
    public int OrbitId { get; set; }
    [JsonPropertyName("id_moon")]
    public int MoonId { get; set; }
    [JsonPropertyName("id_city")]
    public int CityId { get; set; }
    [JsonPropertyName("id_faction")]
    public int FactionId { get; set; }
    [JsonPropertyName("id_jurisdiction")]
    public int JurisdictionId { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("nickname")]
    public string Nickname { get; set; }
    [JsonPropertyName("is_available")]
    public int IsAvailable { get; set; }
    [JsonPropertyName("is_available_live")]
    public int IsAvailableLive { get; set; }
    [JsonPropertyName("is_visible")]
    public int IsVisible { get; set; }
    [JsonPropertyName("is_default")]
    public int IsDefault { get; set; }
    [JsonPropertyName("is_monitored")]
    public int IsMonitored { get; set; }
    [JsonPropertyName("is_armistice")]
    public int IsArmistice { get; set; }
    [JsonPropertyName("is_landable")]
    public int IsLandable { get; set; }
    [JsonPropertyName("is_decommissioned")]
    public int IsDecommissioned { get; set; }
    [JsonPropertyName("is_lagrange")]
    public int IsLagrange { get; set; }
    [JsonPropertyName("is_jump_point")]
    public int IsJumpPoint { get; set; }
    [JsonPropertyName("has_quantum_marker")]
    public int HasQuantumMarker { get; set; }
    [JsonPropertyName("has_trade_terminal")]
    public int HasTradeTerminal { get; set; }
    [JsonPropertyName("has_habitation")]
    public int HasHabitation { get; set; }
    [JsonPropertyName("has_refinery")]
    public int HasRefinery { get; set; }
    [JsonPropertyName("has_cargo_center")]
    public int HasCargoCenter { get; set; }
    [JsonPropertyName("has_clinic")]
    public int HasClinic { get; set; }
    [JsonPropertyName("has_food")]
    public int HasFood { get; set; }
    [JsonPropertyName("has_shops")]
    public int HasShops { get; set; }
    [JsonPropertyName("has_refuel")]
    public int HasRefuel { get; set; }
    [JsonPropertyName("has_repair")]
    public int HasRepair { get; set; }
    [JsonPropertyName("has_gravity")]
    public int HasGravity { get; set; }
    [JsonPropertyName("has_loading_dock")]
    public int HasLoadingDock { get; set; }
    [JsonPropertyName("has_docking_port")]
    public int HasDockingPort { get; set; }
    [JsonPropertyName("has_freight_elevator")]
    public int HasFreightElevator { get; set; }
    [JsonPropertyName("pad_types")]
    public string? PadTypes { get; set; }
    [JsonPropertyName("date_added")]
    public int DateAdded { get; set; }
    [JsonPropertyName("date_modified")]
    public int DateModified { get; set; }
    [JsonPropertyName("star_system_name")]
    public string? StarSystemName { get; set; }
    [JsonPropertyName("planet_name")]
    public string? PlanetName { get; set; }
    [JsonPropertyName("orbit_name")]
    public string? OrbitName { get; set; }
    [JsonPropertyName("city_name")]
    public string? CityName { get; set; }
    [JsonPropertyName("faction_name")]
    public string? FactionName { get; set; }
    [JsonPropertyName("jurisdiction_name")]
    public string? JurisdictionName { get; set; }

    
}

public class UexSpaceStationResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; }
    [JsonPropertyName("http_code")]
    public int HttpCode { get; set; }
    [JsonPropertyName("data")]
    public List<UexSpaceStation> Data { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; }
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

public class UexLocationsResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; }
    [JsonPropertyName("http_code")]
    public int HttpCode { get; set; }
    [JsonPropertyName("data")]
    public List<UexPoi> Data { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; }
}
