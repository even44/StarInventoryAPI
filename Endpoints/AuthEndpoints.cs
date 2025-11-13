public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var authApi = app.MapGroup("/auth")
        .WithTags("Authentication");

        authApi.MapPost("/login", async (UserLogin login, ItemCacheDb db, TokenProvider tokenProvider, PasswordHasher passwordHasher) =>
        {
            User? user = await db.Users.FindAsync(login.Username);
            if (user == null)
            {
                return Results.Unauthorized();
            }

            bool passwordValid = passwordHasher.VerifyPassword(login.Password, user.PasswordHash, user);
            if (!passwordValid)
            {
                return Results.Unauthorized();
            }

            string token = await tokenProvider.Create(user, db);

            return Results.Ok(token);
        });
        authApi.MapPost("/register", async (UserLogin register, ItemCacheDb db, PasswordHasher passwordHasher) =>
        {
            var role = await RoleDataStore.GetRoleByClaim("user", db);
            if (role == null)
            {
                return Results.InternalServerError();
            }
            if(await UserDataStore.CreateUser(register, role.Id, db, passwordHasher))
            {
                return Results.Created($"/login", register.Username);
            }
            return Results.BadRequest();
        });
    }
}