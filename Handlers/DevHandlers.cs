using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http.HttpResults;
using StarInventoryAPI.Db;
using StarInventoryAPI.Services;

namespace StarInventoryAPI.Handlers;

internal static class DevHandlers
{
	public static IResult UpdateCacheFromUex(
		bool updateItems,
		bool updateLocations,
		CacheSyncQueue queue,
		ItemCacheDb db,
		CancellationToken cancellationToken)
	{
		return TypedResults.ServerSentEvents(Stream(cancellationToken));

		async IAsyncEnumerable<object> Stream([EnumeratorCancellation] CancellationToken ct)
		{
			if (!updateItems && !updateLocations)
			{
				var resp = await db.GetCacheUpdateResponse(db);
				yield return new { status = "completed", response = resp };
				yield break;
			}

			var isWaiting = queue.IsRunning || queue.QueuedCount > 0;
			var job = queue.Enqueue(updateItems, updateLocations);

			if (isWaiting)
			{
				yield return new { status = "queued", message = "Sync request queued. Waiting for previous sync to complete..." };
			}

			while (!job.ProgressChannel.Reader.Completion.IsCompleted)
			{
				var readTask = job.ProgressChannel.Reader.WaitToReadAsync(ct).AsTask();
				var delayTask = Task.Delay(5000, ct);

				var completed = await Task.WhenAny(readTask, delayTask);
				if (completed == readTask)
				{
					var hasItems = await readTask;
					if (!hasItems) break;

					while (job.ProgressChannel.Reader.TryRead(out var item))
					{
						yield return item;
					}
				}
				else
				{
					// Heartbeat every 5 seconds to keep the SSE connection alive
					if (queue.IsRunning && queue.QueuedCount > 0)
					{
						yield return new { status = "queued", message = "Waiting in queue..." };
					}
					else
					{
						yield return new { status = "processing", note = "still working..." };
					}
				}
			}

			while (job.ProgressChannel.Reader.TryRead(out var item))
			{
				yield return item;
			}
		}
	}
}
