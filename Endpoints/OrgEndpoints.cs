

public static class OrgEndpoints
{
    public static void MapOrgEndpoints(this IEndpointRouteBuilder app)
    {
        var orgApi = app.MapGroup("/organization")
            .WithTags("Organization Items")
            .RequireAuthorization("organization");
    }
}