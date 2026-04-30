using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http.HttpResults;
using StarInventoryAPI.Db;
using StarInventoryAPI.Store;

namespace StarInventoryAPI.Handlers;

internal record ProgressReport(int Current, int Total);

internal static class DevHandlers
{

	public static IResult UpdateCacheFromUex(bool updateItems, bool updateLocations, ItemCacheDb db,
		IHttpClientFactory httpClientFactory, CancellationToken cancellationToken)
	{

		return TypedResults.ServerSentEvents(Stream(cancellationToken));

		async IAsyncEnumerable<object> Stream([EnumeratorCancellation] CancellationToken ct)
		{

			var client = httpClientFactory.CreateClient("UexApi");
			yield return new { status = "started" };

			if (updateItems)
			{
				yield return new { status = "updating_categories" };
				var catResult = await db.UpdateCategories(db, client);
				if (catResult.Count == 0)
				{
					yield return new { status = "error", step = "categories" };
					yield break;
				}



				for (var i = 0; i < catResult.Count; i++)
				{
					var cat = catResult[i];
					yield return new { status = "updating_items", category = new CategoryDto(cat.Id, cat.Name), progress = new ProgressReport(i + 1, catResult.Count) };
					var updateTask = db.UpdateItemsFromCategory(cat, db, client);

					// 3. Keep yielding "processing" every 5 seconds until the task finishes
					while (!updateTask.IsCompleted)
					{
						// Wait for the task to finish OR for 5 seconds to pass
						var delayTask = Task.Delay(5000, ct);
						var completedTask = await Task.WhenAny(updateTask, delayTask);

						if (completedTask == delayTask && !updateTask.IsCompleted)
						{
							// The 5-second timer hit first! Send a heartbeat.
							yield return new { status = "processing", category = cat.Name, note = "still working..." };
						}
					}
					// 4. Now that it's done, get the result
					var itemResult = await updateTask;
					if (!itemResult)
					{
						yield return new { status = "error", step = "items" };
						yield break;
					}
				}


				yield return new { status = "items_updated" };
			}

			if (!updateLocations)
			{
				var resp = await db.GetCacheUpdateResponse(db);
				yield return new { status = "completed", response = resp };
				yield break;
			}

			yield return new { status = "updating_pois" };
			var poiResult = await db.UpdatePois(db, client);
			if (!poiResult)
			{
				yield return new { status = "error", step = "pois" };
				yield break;
			}

			yield return new { status = "updating_space_stations" };
			var spaceStationResult = await db.UpdateSpaceStations(db, client);
			if (!spaceStationResult)
			{
				yield return new { status = "error", step = "space_stations" };
				yield break;
			}

			yield return new { status = "updating_cities" };
			var cityResult = await db.UpdateCities(db, client);
			if (!cityResult)
			{
				yield return new { status = "error", step = "cities" };
				yield break;
			}

			yield return new { status = "compiling_locations" };
			var locationMergeResult = await db.CompileLocations(db);
			if (!locationMergeResult)
			{
				yield return new { status = "error", step = "compile_locations" };
				yield break;
			}

			var final = await db.GetCacheUpdateResponse(db);
			yield return new { status = "completed", response = final };
		}
	}
}
