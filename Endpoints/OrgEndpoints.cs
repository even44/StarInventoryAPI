using StarInventoryAPI.Handlers;

public static class OrgEndpoints
{
    public static void MapOrgEndpoints(this IEndpointRouteBuilder app)
    {
        var orgApi = app.MapGroup("/organization")
            .WithTags("Organization Items")
            .RequireAuthorization("user");

        orgApi.MapGet("/inventory", OrgHandlers.GetAllSharedItemsFromOrgInventoryUsers);

        orgApi.MapPost("/participatingusers", OrgHandlers.AddUserToOrgInventory).RequireAuthorization("organization");

        orgApi.MapDelete("/participatingusers/{username}", OrgHandlers.RemoveUserFromOrgInventory).RequireAuthorization("organization");

        orgApi.MapGet("/participatingusers", OrgHandlers.GetOrgInventoryUsersList);

        orgApi.MapGet("/users", OrgHandlers.GetAllUsers);

    }
}
