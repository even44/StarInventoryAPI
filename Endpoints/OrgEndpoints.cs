

using Microsoft.AspNetCore.Http.HttpResults;

public static class OrgEndpoints
{
    public static void MapOrgEndpoints(this IEndpointRouteBuilder app)
    {
        var orgApi = app.MapGroup("/organization")
            .WithTags("Organization Items")
            .RequireAuthorization("user");
    
        orgApi.MapGet("/inventory", async Task<Ok<List<StarItem>>> (ItemCacheDb db) =>
        {
            return TypedResults.Ok(await OrgDataStore.GetOrgInventory(db));
        });

        orgApi.MapPost("/participatingusers", async Task<Results<Ok, Conflict<string>>> (User user, ItemCacheDb db) =>
        {
            bool result = await OrgDataStore.AddOrgInventoryUser(user.Username, db);
            if (!result)
            {
                return TypedResults.Conflict("User already exists in organization inventory.");
            }
            return TypedResults.Ok();
        }).RequireAuthorization("organization");

        orgApi.MapDelete("/participatingusers/{username}", async Task<Results<Ok, NotFound<string>>> (string username, ItemCacheDb db) =>
        {
            bool result = await OrgDataStore.RemoveOrgInventoryUser(username, db);
            if (!result)
            {
                return TypedResults.NotFound("User not found in organization inventory.");
            }
            return TypedResults.Ok();
        }).RequireAuthorization("organization");

        orgApi.MapGet("/participatingusers", async Task<Ok<List<OrgInventoryUser>>> (ItemCacheDb db) =>
        {
            List<OrgInventoryUser> users;
            users = await OrgDataStore.GetOrgInventoryUsers(db);
            return TypedResults.Ok(users);
        });

        orgApi.MapGet("/users", async Task<IResult> (ItemCacheDb db) => {
            return TypedResults.Ok(await UserDataStore.GetUsers(db));
        });
        
    }
}