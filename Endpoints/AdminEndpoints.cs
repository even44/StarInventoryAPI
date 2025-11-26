using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;




public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var adminApi = app.MapGroup("/admin")
        .WithTags("Administration")
        .RequireAuthorization("admin");

        adminApi.MapDelete("/wipePersonalItems", AdminHandlers.WipeAllUsersPersonalItems);

        adminApi.MapGet("/users", AdminHandlers.GetUsersList);

    }
}
