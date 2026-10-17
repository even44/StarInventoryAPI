using System.Threading.Channels;

namespace StarInventoryAPI.Services;

public record ProgressReport(int Current, int Total);

public class CacheSyncJob
{
    public string Id { get; } = Guid.NewGuid().ToString("N");
    public bool UpdateItems { get; }
    public bool UpdateLocations { get; }
    public Channel<object> ProgressChannel { get; }
    public TaskCompletionSource<bool> Completion { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public DateTime CreatedAt { get; } = DateTime.UtcNow;

    private volatile bool _isStarted;
    public bool IsStarted => _isStarted;

    public CacheSyncJob(bool updateItems, bool updateLocations)
    {
        UpdateItems = updateItems;
        UpdateLocations = updateLocations;
        ProgressChannel = Channel.CreateUnbounded<object>(new UnboundedChannelOptions
        {
            SingleWriter = true,
            SingleReader = false
        });
    }

    public void MarkStarted()
    {
        _isStarted = true;
    }

    public void ReportProgress(object data)
    {
        ProgressChannel.Writer.TryWrite(data);
    }

    public void Complete()
    {
        ProgressChannel.Writer.TryComplete();
        Completion.TrySetResult(true);
    }
}

public class CacheSyncQueue
{
    private readonly Channel<CacheSyncJob> _jobs = Channel.CreateUnbounded<CacheSyncJob>(new UnboundedChannelOptions
    {
        SingleWriter = false,
        SingleReader = true
    });

    private int _queuedCount = 0;
    private volatile CacheSyncJob? _currentJob;

    public int QueuedCount => Volatile.Read(ref _queuedCount);
    public bool IsRunning => _currentJob != null;
    public CacheSyncJob? CurrentJob => _currentJob;

    public CacheSyncJob Enqueue(bool updateItems, bool updateLocations)
    {
        var job = new CacheSyncJob(updateItems, updateLocations);
        Interlocked.Increment(ref _queuedCount);
        _jobs.Writer.TryWrite(job);
        return job;
    }

    public async ValueTask<CacheSyncJob> DequeueAsync(CancellationToken cancellationToken)
    {
        var job = await _jobs.Reader.ReadAsync(cancellationToken);
        Interlocked.Decrement(ref _queuedCount);
        return job;
    }

    public void SetCurrentJob(CacheSyncJob? job)
    {
        _currentJob = job;
    }
}
