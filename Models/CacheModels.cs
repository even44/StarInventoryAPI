using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
public class UexItem
{
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
    [JsonPropertyName("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    [JsonPropertyName("id_parent")]
    public int parentId { get; set; }
    [JsonPropertyName("id_category")]
    public int categoryId { get; set; }
    [JsonPropertyName("id_vehicle")]
    public int? vehicleId { get; set; }
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("section")]
    public string? Section { get; set; }
    [JsonPropertyName("category")]
    public string? Category { get; set; }
    [JsonPropertyName("company_name")]
    public string? CompanyName { get; set; }
    [JsonPropertyName("vehicle_name")]
    public string? VehicleName { get; set; }
    [JsonPropertyName("slug")]
    public required string Slug { get; set; }
    [JsonPropertyName("size")]
    public string? Size { get; set; }
    [JsonPropertyName("uuid")]
    public string? Uuid { get; set; }
    [JsonPropertyName("url_store")]
    public string? StoreUrl { get; set; }
    [JsonPropertyName("is_exclusive_pledge")]
    public int is_exclusive_pledge { get; set; }
    [JsonPropertyName("is_exclusive_subscriber")]
    public int is_exclusive_subscriber { get; set; }
    [JsonPropertyName("is_exclusive_concierge")]
    public int is_exclusive_concierge { get; set; }
    [JsonPropertyName("is_commodity")]
    public int is_commodity { get; set; }
    [JsonPropertyName("is_harvestable")]
    public int is_harvestable { get; set; }
    [JsonPropertyName("notification")]
    public string? Notification { get; set; }
    [JsonPropertyName("game_version")]
    public string? GameVersion { get; set; }
    [JsonPropertyName("date_added")]
    public int DateAdded { get; set; }
    [JsonPropertyName("date_modified")]
    public int DateModified { get; set; }
}
public class UexCategory
{
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
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("type")]
    public required string Type { get; set; }
    [JsonPropertyName("section")]
    public required string Section { get; set; }
    [JsonPropertyName("name")]
    public required string Name { get; set; }
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
    public required string Name { get; set; }
    [JsonPropertyName("nickname")]
    public required string Nickname { get; set; }
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
    public required string Name { get; set; }
    [JsonPropertyName("nickname")]
    public required string Nickname { get; set; }
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
    public required string Status { get; set; }
    [JsonPropertyName("http_code")]
    public int HttpCode { get; set; }
    [JsonPropertyName("data")]
    public required List<UexSpaceStation> Data { get; set; }
    [JsonPropertyName("message")]
    public required string Message { get; set; }
}
public class UexCategoryResponse
{
    [JsonPropertyName("status")]
    public required string Status { get; set; }
    [JsonPropertyName("http_code")]
    public int HttpCode { get; set; }
    [JsonPropertyName("data")]
    public required List<UexCategory> Data { get; set; }
    [JsonPropertyName("message")]
    public required string Message { get; set; }
}
public class UexLocationsResponse
{
    [JsonPropertyName("status")]
    public required string Status { get; set; }
    [JsonPropertyName("http_code")]
    public int HttpCode { get; set; }
    [JsonPropertyName("data")]
    public required List<UexPoi> Data { get; set; }
    [JsonPropertyName("message")]
    public required string Message { get; set; }
}
public class UexItemsResponse
{
    [JsonPropertyName("status")]
    public required string Status { get; set; }
    [JsonPropertyName("http_code")]
    public int HttpCode { get; set; }
    [JsonPropertyName("data")]
    public required List<UexItem> Data { get; set; }
    [JsonPropertyName("message")]
    public required string Message { get; set; }
}