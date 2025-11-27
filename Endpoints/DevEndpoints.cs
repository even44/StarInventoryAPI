public static class DevEndpoints
{
    public static void MapDevEndpoints(this IEndpointRouteBuilder app)
    {
        var devApi = app.MapGroup("/dev")
            .WithTags("Development")
            .RequireAuthorization("dev");

        // Update the Cache from UEX and compile a list of locations
        devApi.MapGet("/updateCache", DevHandlers.UpdateCacheFromUex);

    }
}
