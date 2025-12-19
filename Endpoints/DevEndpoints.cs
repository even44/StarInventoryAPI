using StarInventoryAPI.Handlers;

public static class DevEndpoints
{
    public static void MapDevEndpoints(this IEndpointRouteBuilder app)
    {
        var devApi = app.MapGroup("/dev")
            .WithTags("Development")
            .RequireAuthorization("dev");

        // Update the Cache from UEX and compile a list of locations
        devApi.MapGet("/updateCache", DevHandlers.UpdateCacheFromUex);

        // SSE endpoint with real-time status updates (no auth required - token in query string)
        app.MapGet("/dev/updateCache/stream", DevHandlers.UpdateCacheFromUexWithStatus)
            .WithTags("Development");

    }
}
