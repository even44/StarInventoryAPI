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
            var userList = await StarDataStore.GetUsers(db);

            return Results.Ok(userList);
        });

        adminApi.MapGet("/users/{username}", async (string username, ItemCacheDb db) =>
        {
            User? user = await StarDataStore.GetUser(username, db);
            if (user == null)
            {
                return Results.BadRequest("User does not exist");
            }

            return Results.Ok(user);
        });

        adminApi.MapPost("/register/{role}", async (string role, UserLogin newUserLogin, ItemCacheDb db, PasswordHasher passwordHasher) =>
        {
            if(await StarDataStore.CreateUser(newUserLogin, role, db, passwordHasher))
            {
                return Results.Created($"/login", newUserLogin.Username);
            }
            return Results.BadRequest();
        });
    }
}