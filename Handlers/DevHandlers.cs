using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http.HttpResults;
using StarInventoryAPI.Services;

namespace StarInventoryAPI.Handlers;

internal static class DevHandlers
{
	private static readonly TimeSpan HeartbeatInterval = TimeSpan.FromSeconds(5);

	public static IResult UpdateCacheFromUex(
		bool updateItems,
		bool updateLocations,
		CacheSyncQueue queue,
		IUexSyncService sync,
		CancellationToken cancellationToken)
	{
		return TypedResults.ServerSentEvents(Stream(cancellationToken));

		async IAsyncEnumerable<object> Stream([EnumeratorCancellation] CancellationToken ct)
		{
			if (!updateItems && !updateLocations)
			{
				var resp = await sync.GetCacheUpdateResponseAsync(ct);
				yield return new { status = "completed", response = resp };
				yield break;
			}

			var isWaiting = queue.IsRunning || queue.QueuedCount > 0;
			var job = queue.Enqueue(updateItems, updateLocations);

			if (isWaiting)
			{
				yield return new { status = "queued", message = "Sync request queued. Waiting for previous sync to complete..." };
			}

			var reader = job.ProgressChannel.Reader;
			// One pending read is kept across heartbeats, so a message that arrives while a
			// heartbeat is being written is picked up on the next pass instead of waiting on a
			// fresh wait/delay race.
			Task<bool>? readTask = null;

			while (true)
			{
				readTask ??= reader.WaitToReadAsync(ct).AsTask();

				using (var heartbeatCts = CancellationTokenSource.CreateLinkedTokenSource(ct))
				{
					var heartbeatTask = Task.Delay(HeartbeatInterval, heartbeatCts.Token);
					var completed = await Task.WhenAny(readTask, heartbeatTask);
					heartbeatCts.Cancel();
					ct.ThrowIfCancellationRequested();

					if (completed != readTask)
					{
						// Heartbeat after 5 seconds without progress to keep the SSE connection alive
						yield return job.IsStarted
							? (object)new { status = "processing", note = "still working..." }
							: new { status = "queued", message = "Waiting in queue..." };
						continue;
					}
				}

				var hasItems = await readTask;
				readTask = null;
				if (!hasItems) break;

				while (reader.TryRead(out var item))
				{
					yield return item;
				}
			}
		}
	}
}
