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

            string token = tokenProvider.Create(user);

            return Results.Ok(token);
        });
        authApi.MapPost("/register", async (UserLogin register, ItemCacheDb db, PasswordHasher passwordHasher) =>
        {
            User? existingUser = await db.Users.FindAsync(register.Username);
            if (existingUser != null)
            {
                return Results.Conflict("Username already exists");
            }


            User newUser = new User();
            string passwordHash = passwordHasher.HashPassword(register.Password, newUser);
            newUser.Username = register.Username;
            newUser.PasswordHash = passwordHash;
            newUser.Role = "user";

            db.Users.Add(newUser);
            await db.SaveChangesAsync();

            return Results.Created($"/login", newUser.Username);
        });
    }
}