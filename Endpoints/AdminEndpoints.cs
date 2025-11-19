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

        adminApi.MapGet("/resetdb", async (ItemCacheDb db) =>
        {
            var itemlist = await db.PersonalItems.ToListAsync();
            var locationlist = await db.StarLocations.ToListAsync();

            db.PersonalItems.RemoveRange(itemlist);
            db.StarLocations.RemoveRange(locationlist);

            await db.SaveChangesAsync();

            return Results.Ok("Database Cleared");
        });

        adminApi.MapGet("/users", async (ItemCacheDb db) =>
        {
            var userList = await UserDataStore.GetUsers(db);

            return Results.Ok(userList);
        });

    }
}