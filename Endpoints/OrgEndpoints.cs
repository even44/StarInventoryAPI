

public static class OrgEndpoints
{
    public static void MapOrgEndpoints(this IEndpointRouteBuilder app)
    {
        var orgApi = app.MapGroup("/organization")
            .WithTags("Organization Items")
            .RequireAuthorization("user");
    
        orgApi.MapGet("inventory", async (ItemCacheDb db) =>
        {
            return await OrgDataStore.GetOrgInventory(db);
        });

        orgApi.MapPost("participatingusers", async (string username, ItemCacheDb db) =>
        {
            bool result = await OrgDataStore.AddOrgInventoryUser(username, db);
            if (!result)
            {
                return Results.Conflict("User already exists in organization inventory.");
            }
            return Results.Ok();
        }).RequireAuthorization("organization");

        orgApi.MapDelete("participatingusers", async (string username, ItemCacheDb db) =>
        {
            bool result = await OrgDataStore.RemoveOrgInventoryUser(username, db);
            if (!result)
            {
                return Results.NotFound("User not found in organization inventory.");
            }
            return Results.Ok();
        }).RequireAuthorization("organization");

        orgApi.MapGet("participatingusers", async (ItemCacheDb db) =>
        {
            return await OrgDataStore.GetOrgInventoryUsers(db);
        });

        orgApi.MapGet("/users", async (ItemCacheDb db) => {
            return await UserDataStore.GetUsers(db);
        });
        
    }
}