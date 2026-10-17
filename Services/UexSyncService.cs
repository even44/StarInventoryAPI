using System.Net;
using Microsoft.EntityFrameworkCore;
using StarInventoryAPI.Db;

namespace StarInventoryAPI.Services;

public record CategoryDto(int Id, string Name);

public interface IUexSyncService
{
    Task<List<UexCategory>> UpdateCategoriesAsync(CancellationToken ct = default);
    Task<bool> UpdateItemsFromCategoryAsync(UexCategory category, CancellationToken ct = default);
    Task<bool> UpdatePoisAsync(CancellationToken ct = default);
    Task<bool> UpdateSpaceStationsAsync(CancellationToken ct = default);
    Task<bool> UpdateCitiesAsync(CancellationToken ct = default);
    Task<bool> CompileLocationsAsync(CancellationToken ct = default);
    Task<CacheUpdateResponse> GetCacheUpdateResponseAsync(CancellationToken ct = default);
}

/// <summary>
/// Synchronizes the local cache with the UEX API and compiles StarLocations.
/// </summary>
public class UexSyncService : IUexSyncService
{
    public const string CommoditiesCategoryName = "Commodities";

    // Commodities share UexItems with regular items but have their own UEX id space,
    // so they are stored as Id + offset. The value is persisted in users'
    // PersonalItems.UexIdentifier, so it must never change.
    public const int CommodityIdOffset = 10000;

    private readonly ItemCacheDb _db;
    private readonly HttpClient _client;
    private readonly ILogger<UexSyncService> _logger;

    public UexSyncService(ItemCacheDb db, IHttpClientFactory httpClientFactory, ILogger<UexSyncService> logger)
    {
        _db = db;
        _client = httpClientFactory.CreateClient("UexApi");
        _logger = logger;
    }

    public async Task<List<UexCategory>> UpdateCategoriesAsync(CancellationToken ct = default)
    {
        var categories = await FetchAsync<UexCategoryResponse>("categories", ct);
        if (categories?.Data == null)
        {
            _logger.LogWarning("UEX categories update failed");
            return [];
        }

        await UpsertByIdAsync(categories.Data, c => c.Id, ct);
        _logger.LogInformation("UEX categories updated: {Count}", categories.Data.Count);
        return await _db.UexCategories.AsNoTracking().ToListAsync(ct);
    }

    public async Task<bool> UpdateItemsFromCategoryAsync(UexCategory category, CancellationToken ct = default)
    {
        if (category.Name.Equals(CommoditiesCategoryName, StringComparison.OrdinalIgnoreCase))
        {
            var commodityItems = await GetCommoditiesAsync(category.Id, ct);
            if (commodityItems.Count > 0)
            {
                await UpsertByIdAsync(commodityItems, i => i.Id, ct);
            }
        }

        var items = await FetchAsync<UexItemsResponse>($"items?id_category={category.Id}", ct);
        if (items == null)
        {
            _logger.LogWarning("UEX items update failed for category {CategoryId}", category.Id);
            return false;
        }

        if (items.Data is { Count: > 0 })
        {
            foreach (var item in items.Data)
            {
                item.Name = DecodeHtml(item.Name);
            }

            await UpsertByIdAsync(items.Data, i => i.Id, ct);
            _logger.LogInformation("UEX items for category {CategoryId} ({CategoryName}) updated: {Count}",
                category.Id, category.Name, items.Data.Count);
        }

        return true;
    }

    private async Task<List<UexItem>> GetCommoditiesAsync(int commodityCategoryId, CancellationToken ct)
    {
        var response = await FetchAsync<UexCommodityResponse>("commodities", ct);
        if (response?.Data == null)
        {
            return [];
        }

        var commodityItems = response.Data
            .Select(commodity => new UexItem
            {
                Id = commodity.Id + CommodityIdOffset,
                Name = DecodeHtml(commodity.Name),
                Slug = commodity.Code,
                categoryId = commodityCategoryId,
                DateAdded = commodity.DateAdded,
                DateModified = commodity.DateModified,
            })
            .ToList();

        // Guard against the offset id range overlapping real UEX item ids, which would
        // silently overwrite an unrelated item.
        var ids = commodityItems.Select(i => i.Id).ToList();
        var collisions = await _db.UexItems
            .Where(i => ids.Contains(i.Id) && i.categoryId != commodityCategoryId)
            .Select(i => i.Id)
            .ToListAsync(ct);
        if (collisions.Count > 0)
        {
            _logger.LogError(
                "Skipping {Count} commodities whose offset ids collide with regular UEX items: {Ids}",
                collisions.Count, string.Join(", ", collisions));
            var collisionSet = collisions.ToHashSet();
            commodityItems.RemoveAll(i => collisionSet.Contains(i.Id));
        }

        return commodityItems;
    }

    public async Task<bool> UpdatePoisAsync(CancellationToken ct = default)
    {
        var pois = await FetchAsync<UexLocationsResponse>("poi", ct);
        if (pois?.Data == null)
        {
            _logger.LogWarning("UEX POI update failed");
            return false;
        }

        await UpsertByIdAsync(pois.Data, p => p.Id, ct);
        _logger.LogInformation("UEX POIs updated: {Count}", pois.Data.Count);
        return true;
    }

    public async Task<bool> UpdateSpaceStationsAsync(CancellationToken ct = default)
    {
        var stations = await FetchAsync<UexSpaceStationResponse>("space_stations", ct);
        if (stations?.Data == null)
        {
            _logger.LogWarning("UEX space stations update failed");
            return false;
        }

        await UpsertByIdAsync(stations.Data, s => s.Id, ct);
        _logger.LogInformation("UEX space stations updated: {Count}", stations.Data.Count);
        return true;
    }

    public async Task<bool> UpdateCitiesAsync(CancellationToken ct = default)
    {
        var cities = await FetchAsync<UexCityResponse>("cities", ct);
        if (cities?.Data == null)
        {
            _logger.LogWarning("UEX cities update failed");
            return false;
        }

        await UpsertByIdAsync(cities.Data, c => c.Id, ct);
        _logger.LogInformation("UEX cities updated: {Count}", cities.Data.Count);
        return true;
    }

    /// <summary>
    /// Merges POIs, space stations and cities into StarLocations. Each location is keyed
    /// by its UEX source (type + id), so an existing location keeps its Id across syncs
    /// and PersonalItems.LocationId keeps pointing at the same place.
    /// </summary>
    public async Task<bool> CompileLocationsAsync(CancellationToken ct = default)
    {
        var pois = await _db.UexPois.AsNoTracking()
            .OrderBy(p => p.Id)
            .Select(p => new { p.Id, p.Name })
            .ToListAsync(ct);
        var stations = await _db.UexSpaceStations.AsNoTracking()
            .OrderBy(s => s.Id)
            .Select(s => new { s.Id, s.Name })
            .ToListAsync(ct);
        var cities = await _db.UexCities.AsNoTracking()
            .OrderBy(c => c.Id)
            .Select(c => new { c.Id, c.Name })
            .ToListAsync(ct);

        // Same order as the legacy positional compile, so legacy rows are adopted in order.
        var sources = pois.Select(p => (Type: LocationSourceType.Poi, p.Id, p.Name))
            .Concat(stations.Select(s => (Type: LocationSourceType.SpaceStation, s.Id, s.Name)))
            .Concat(cities.Select(c => (Type: LocationSourceType.City, c.Id, c.Name)))
            .ToList();

        var locations = await _db.StarLocations.ToListAsync(ct);
        var bySource = locations
            .Where(l => l.SourceType != null && l.SourceId != null)
            .ToDictionary(l => (l.SourceType!.Value, l.SourceId!.Value));
        // Rows compiled before stable mapping existed are claimed by name, lowest Id first.
        var legacyByName = locations
            .Where(l => l.SourceType == null || l.SourceId == null)
            .OrderBy(l => l.Id)
            .GroupBy(l => l.Name)
            .ToDictionary(g => g.Key, g => new Queue<StarLocation>(g));

        var nextId = locations.Count == 0 ? 0 : locations.Max(l => l.Id) + 1;
        var liveIds = new HashSet<int>();
        int added = 0, adopted = 0;

        foreach (var source in sources)
        {
            if (!bySource.TryGetValue((source.Type, source.Id), out var location))
            {
                if (legacyByName.TryGetValue(source.Name, out var legacy) && legacy.Count > 0)
                {
                    location = legacy.Dequeue();
                    location.SourceType = source.Type;
                    location.SourceId = source.Id;
                    adopted++;
                }
                else
                {
                    location = new StarLocation
                    {
                        Id = nextId++,
                        Name = source.Name,
                        SourceType = source.Type,
                        SourceId = source.Id,
                    };
                    _db.StarLocations.Add(location);
                    added++;
                }

                bySource[(source.Type, source.Id)] = location;
            }

            location.Name = source.Name;
            liveIds.Add(location.Id);
        }

        // Locations that disappeared from UEX are only deleted when no user item points at them.
        var stale = locations.Where(l => !liveIds.Contains(l.Id)).ToList();
        var staleIds = stale.Select(l => l.Id).ToList();
        var referencedIds = (await _db.PersonalItems
                .Where(i => staleIds.Contains(i.LocationId))
                .Select(i => i.LocationId)
                .Distinct()
                .ToListAsync(ct))
            .ToHashSet();
        var removable = stale.Where(l => !referencedIds.Contains(l.Id)).ToList();
        _db.StarLocations.RemoveRange(removable);

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Compiled {Pois} + {Stations} + {Cities} = {Live} locations ({Added} added, {Adopted} adopted, {Removed} removed, {Retained} retained because referenced by user items)",
            pois.Count, stations.Count, cities.Count, liveIds.Count, added, adopted, removable.Count, referencedIds.Count);
        return true;
    }

    public async Task<CacheUpdateResponse> GetCacheUpdateResponseAsync(CancellationToken ct = default)
    {
        var totalItems = await _db.UexItems.CountAsync(ct);
        var totalPois = await _db.UexPois.CountAsync(ct);
        var totalStations = await _db.UexSpaceStations.CountAsync(ct);
        var totalCities = await _db.UexCities.CountAsync(ct);

        return new CacheUpdateResponse
        {
            TotalItems = totalItems,
            TotalPois = totalPois,
            TotalStations = totalStations,
            TotalCities = totalCities,
            Total = totalItems + totalPois + totalCities + totalStations
        };
    }

    private async Task<T?> FetchAsync<T>(string path, CancellationToken ct) where T : class
    {
        var response = await _client.GetAsync(path, ct);
        _logger.LogInformation("UEX {Path} response: {StatusCode}", path, response.StatusCode);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<T>(ct);
    }

    private async Task UpsertByIdAsync<T>(IEnumerable<T> incoming, Func<T, int> getId, CancellationToken ct)
        where T : class
    {
        var rows = incoming.DistinctBy(getId).ToList();
        var ids = rows.Select(getId).ToList();
        var set = _db.Set<T>();
        var existing = await set
            .Where(e => ids.Contains(EF.Property<int>(e, "Id")))
            .ToDictionaryAsync(getId, ct);

        foreach (var row in rows)
        {
            if (existing.TryGetValue(getId(row), out var current))
            {
                _db.Entry(current).CurrentValues.SetValues(row);
            }
            else
            {
                set.Add(row);
            }
        }

        await _db.SaveChangesAsync(ct);
    }

    internal static string DecodeHtml(string value) => WebUtility.HtmlDecode(value);
}
