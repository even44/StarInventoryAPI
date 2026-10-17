using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace StarInventoryAPI.Services;

public class CacheSyncWorker : BackgroundService
{
    private readonly CacheSyncQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CacheSyncWorker> _logger;

    public CacheSyncWorker(
        CacheSyncQueue queue,
        IServiceScopeFactory scopeFactory,
        ILogger<CacheSyncWorker> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CacheSyncWorker background service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            CacheSyncJob job;
            try
            {
                job = await _queue.DequeueAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }

            _queue.SetCurrentJob(job);
            job.MarkStarted();
            try
            {
                await ProcessJobAsync(job, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing cache sync job {JobId}", job.Id);
                job.ReportProgress(new { status = "error", step = "worker", message = ex.Message });
            }
            finally
            {
                _queue.SetCurrentJob(null);
                job.Complete();
            }
        }

        _logger.LogInformation("CacheSyncWorker background service stopped.");
    }

    private async Task ProcessJobAsync(CacheSyncJob job, CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var sync = scope.ServiceProvider.GetRequiredService<IUexSyncService>();

        _logger.LogInformation("Processing cache sync job {JobId} (UpdateItems: {UpdateItems}, UpdateLocations: {UpdateLocations})",
            job.Id, job.UpdateItems, job.UpdateLocations);

        job.ReportProgress(new { status = "started" });

        if (job.UpdateItems)
        {
            job.ReportProgress(new { status = "updating_categories" });
            var catResult = await sync.UpdateCategoriesAsync(stoppingToken);
            if (catResult.Count == 0)
            {
                _logger.LogWarning("Job {JobId}: Failed to update categories.", job.Id);
                job.ReportProgress(new { status = "error", step = "categories" });
                return;
            }

            for (var i = 0; i < catResult.Count; i++)
            {
                if (stoppingToken.IsCancellationRequested) return;

                var cat = catResult[i];
                job.ReportProgress(new
                {
                    status = "updating_items",
                    category = new CategoryDto(cat.Id, cat.Name),
                    progress = new ProgressReport(i + 1, catResult.Count)
                });

                var updateSuccess = await sync.UpdateItemsFromCategoryAsync(cat, stoppingToken);
                if (!updateSuccess)
                {
                    _logger.LogWarning("Job {JobId}: Failed to update items for category {CatName} ({CatId})",
                        job.Id, cat.Name, cat.Id);
                    job.ReportProgress(new { status = "error", step = "items" });
                    return;
                }
            }

            job.ReportProgress(new { status = "items_updated" });
        }

        if (!job.UpdateLocations)
        {
            var resp = await sync.GetCacheUpdateResponseAsync(stoppingToken);
            job.ReportProgress(new { status = "completed", response = resp });
            return;
        }

        if (stoppingToken.IsCancellationRequested) return;

        job.ReportProgress(new { status = "updating_pois" });
        var poiResult = await sync.UpdatePoisAsync(stoppingToken);
        if (!poiResult)
        {
            _logger.LogWarning("Job {JobId}: Failed to update POIs.", job.Id);
            job.ReportProgress(new { status = "error", step = "pois" });
            return;
        }

        if (stoppingToken.IsCancellationRequested) return;

        job.ReportProgress(new { status = "updating_space_stations" });
        var spaceStationResult = await sync.UpdateSpaceStationsAsync(stoppingToken);
        if (!spaceStationResult)
        {
            _logger.LogWarning("Job {JobId}: Failed to update space stations.", job.Id);
            job.ReportProgress(new { status = "error", step = "space_stations" });
            return;
        }

        if (stoppingToken.IsCancellationRequested) return;

        job.ReportProgress(new { status = "updating_cities" });
        var cityResult = await sync.UpdateCitiesAsync(stoppingToken);
        if (!cityResult)
        {
            _logger.LogWarning("Job {JobId}: Failed to update cities.", job.Id);
            job.ReportProgress(new { status = "error", step = "cities" });
            return;
        }

        if (stoppingToken.IsCancellationRequested) return;

        job.ReportProgress(new { status = "compiling_locations" });
        var locationMergeResult = await sync.CompileLocationsAsync(stoppingToken);
        if (!locationMergeResult)
        {
            _logger.LogWarning("Job {JobId}: Failed to compile locations.", job.Id);
            job.ReportProgress(new { status = "error", step = "compile_locations" });
            return;
        }

        var final = await sync.GetCacheUpdateResponseAsync(stoppingToken);
        _logger.LogInformation("Job {JobId} completed successfully. Total cached: {Total}", job.Id, final.Total);
        job.ReportProgress(new { status = "completed", response = final });
    }
}
