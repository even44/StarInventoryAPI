
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS");
            policy.AllowAnyOrigin();
            policy.WithHeaders("Content-Type", "Authorization", "Accept");
        });
});

var connectionString = builder.Configuration.GetConnectionString("MariaDbConnection");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithAuth();


builder.Services.AddHttpClient("UexApi", client =>
{
    client.BaseAddress = new Uri("https://api.uexcorp.uk/2.0/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("dev", policy => policy.RequireClaim(ClaimTypes.Role, "dev"))
    .AddPolicy("admin", policy => policy.RequireClaim(ClaimTypes.Role, "admin"))
    .AddPolicy("organization", policy => policy.RequireClaim(ClaimTypes.Role, "organization"))
    .AddPolicy("user", policy => policy.RequireClaim(ClaimTypes.Role, "user"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddDbContext<ItemCacheDb>(
    opt => opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
    .LogTo(Console.WriteLine, LogLevel.Information)
    .EnableSensitiveDataLogging()
    .EnableDetailedErrors()
);

builder.Services.AddSingleton<TokenProvider>();
builder.Services.AddSingleton<PasswordHasher>();


var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}


var authApi = app.MapGroup("/auth")
    .WithTags("Authentication");

var devApi = app.MapGroup("/dev")
    .WithTags("Development")
    .RequireAuthorization("dev");

var personalApi = app.MapGroup("/personal")
    .WithTags("Personal Items")
    .RequireAuthorization("user");

var orgApi = app.MapGroup("/organization")
    .WithTags("Organization Items")
    .RequireAuthorization("organization");

var cacheApi = app.MapGroup("/cache")
    .WithTags("Cached Items")
    .RequireAuthorization("user");

var adminApi = app.MapGroup("/admin")
    .WithTags("Administration")
    .RequireAuthorization("admin");


// Update the Cache from UEX and compile a list of locations
devApi.MapGet("/updateCache", async (ItemCacheDb db, IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient("UexApi");

    bool catResult = await db.UpdateCategories(db, client);
    if (!catResult)
    {
        Console.WriteLine("Failed to update categories");
        return Results.StatusCode(500);
    }
    bool locResult = await db.UpdatePois(db, client);
    if (!locResult)
    {
        Console.WriteLine("Failed to update pois");
        return Results.StatusCode(500);
    }
    bool spaceStationResult = await db.UpdateSpaceStations(db, client);
    if (!spaceStationResult)
    {
        Console.WriteLine("Failed to update space stations");
        return Results.StatusCode(500);
    }
    bool locationMergeResult = await db.CompileLocations(db);
    if (!locationMergeResult)
    {
        Console.WriteLine("Failed to compile locations");
        return Results.StatusCode(500);
    }

    bool itemResult = await db.UpdateItems(db, client);
    if (!itemResult)
    {
        Console.WriteLine("Failed to update items");
        return Results.StatusCode(500);
    }
    return Results.Ok("Cache Update Done");
});


devApi.MapGet("/randomitem", async (ItemCacheDb db, HttpContext httpContext) =>
{
    string? username = httpContext.User.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value;
    if (username == null)
    {
        return Results.Unauthorized();
    }

    StarItem item = StarItem.RandomItem(db, username);

    StarItem? resultItem = await StarDataStore.AddStarItem(db, item, username);

    if (resultItem == null)
    {
        return Results.InternalServerError("Failed to add random item");
    }

    return Results.Created($"/personal/items/{resultItem.Id}", resultItem);

});


// GET LIST
personalApi.MapGet("/items", async (ItemCacheDb db, HttpContext httpContext) =>
{

    string? username = httpContext.User.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value;
    if (username == null)
    {
        return Results.Unauthorized();
    }

    List<StarItem> items;
    items = await StarDataStore.GetPersonalItems(db, username);
    return Results.Ok(items);
});

personalApi.MapGet("/items/{searchTerm}", async (string searchTerm, ItemCacheDb db, HttpContext httpContext) =>
{

    string? username = httpContext.User.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value;
    if (username == null)
    {
        return Results.Unauthorized();
    }

    List<StarItem> items;
    items = await StarDataStore.SearchPersonalItems(db, username, searchTerm);
    return Results.Ok(items);
});

// ADD ONE
personalApi.MapPost("/items/{id}", async (StarItem item, ItemCacheDb db, HttpContext httpContext) =>
{
    string? username = httpContext.User.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value;
    if (username == null)
    {
        return Results.Unauthorized();
    }
    StarItem? resultItem = await StarDataStore.AddStarItem(db, item, username);
    if (resultItem == null)
    {
        return Results.BadRequest("Invalid Location");
    }
    return Results.Created($"/personal/items/{resultItem.Id}", resultItem);
});

// DELETE ONE
personalApi.MapDelete("/items/{id}", async (int id, ItemCacheDb db, HttpContext httpContext) =>
{
    string? username = httpContext.User.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value;
    if (username == null)
    {
        return Results.Unauthorized();
    }
    bool result = await StarDataStore.DeleteStarItem(db, id, username);
    if (!result)
    {
        return Results.NotFound("Item not found");
    }


    return Results.Ok();
});

personalApi.MapPut("/items/{id}", async (int id, StarItem item, ItemCacheDb db, HttpContext httpContext) =>
{
    string? username = httpContext.User.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value;
    if (username == null)
    {
        return Results.Unauthorized();
    }
    StarItem reusltItem = await StarDataStore.UpdateStarItem(db, id, item, username);

    return Results.Ok(reusltItem);
}).RequireAuthorization();

cacheApi.MapGet("/locations", async (string searchTerm, ItemCacheDb db) =>
{
    List<StarLocation> locations;
    locations = await db.StarLocations.ToListAsync();
    return Results.Ok(locations);
});

cacheApi.MapGet("/locations/{searchTerm}", async (string searchTerm, ItemCacheDb db) =>
{
    List<StarLocation> locations;
    locations = await StarDataStore.SearchStarLocations(db, searchTerm);
    return Results.Ok(locations);
});


cacheApi.MapGet("/categories", async (ItemCacheDb db) =>
{
    List<UexCategory> categories = await StarDataStore.GetUexCategories(db);
    return Results.Ok(categories);
});

cacheApi.MapGet("/items", async (ItemCacheDb db) =>
{
    List<UexItem> items = await StarDataStore.GetUexItems(db);
    return Results.Ok(items);
});

cacheApi.MapGet("/items/{searchTerm}", async (string searchTerm, ItemCacheDb db) =>
{
    List<UexItem> items = await StarDataStore.SearchUexItems(db, searchTerm);
    return Results.Ok(items);
});

adminApi.MapGet("/resetdb", async (ItemCacheDb db) =>
{
    var itemlist = await db.PersonalItems.ToListAsync();
    var locationlist = await db.StarLocations.ToListAsync();

    db.PersonalItems.RemoveRange(itemlist);
    db.StarLocations.RemoveRange(locationlist);

    await db.SaveChangesAsync();

    return Results.Ok("Database Cleared");
}).RequireAuthorization();


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



app.Run();
// This is a comment
