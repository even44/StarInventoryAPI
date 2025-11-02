using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.AllowAnyMethod();
            policy.AllowAnyOrigin();
            policy.AllowAnyHeader();
        });
});

var connectionString = builder.Configuration.GetConnectionString("MariaDbConnection");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddHttpClient("UexApi", client =>
{
    client.BaseAddress = new Uri("https://api.uexcorp.uk/2.0/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();

builder.Services.AddDbContext<ItemCacheDb>(
    opt => opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
    .LogTo(Console.WriteLine, LogLevel.Information)
    .EnableSensitiveDataLogging()
    .EnableDetailedErrors()
);



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



app.MapGet("/updateCache", async (ItemCacheDb db, IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient("UexApi");
    bool result = await db.UpdateCategories(db, client);
    if (!result)
    {
        return Results.StatusCode(500);
    }
    return Results.Ok("Cache Update Done");
});


app.MapGet("/dev/randomitem", async (ItemCacheDb db) =>
{
    StarItem item = StarItem.RandomItem();

    StarItem resultItem = await StarDataStore.AddStarItem(db, item);

    return Results.Created($"/personal/items/{resultItem.Id}", resultItem);

});


// GET LIST
app.MapGet("/personal/items", async (ItemCacheDb db) =>
{
    List<StarItem> items;
    items = await StarDataStore.GetPersonalItems(db);
    return Results.Ok(items);
});

// ADD ONE
app.MapPost("/personal/items/{id}", async (StarItem item, ItemCacheDb db) =>
{
    StarItem? resultItem = await StarDataStore.AddStarItem(db, item);
    if (resultItem == null)
    {
        return Results.BadRequest("Invalid Location");
    }
    return Results.Created($"/personal/items/{resultItem.Id}", resultItem);
});

// DELETE ONE
app.MapDelete("/personal/items/{id}", async (int id, ItemCacheDb db) =>
{
    var item = await db.PersonalItems.FindAsync(id);
    if (item == null)
    {
        return Results.NotFound();
    }

    db.PersonalItems.Remove(item);
    await db.SaveChangesAsync();

    return Results.Ok();
});

app.MapPut("/personal/items/{id}", async (int id, StarItem item, ItemCacheDb db) =>
{
    StarItem reusltItem = await StarDataStore.UpdateStarItem(db, id, item);

    return Results.Ok(reusltItem);
});

app.MapGet("/locations", async (ItemCacheDb db) =>
{
    List<StarLocation> locations;
    locations = await db.StarLocations.ToListAsync();
    return Results.Ok(locations);
});

app.MapGet("/resetdb", async (ItemCacheDb db) =>
{
    var itemlist = await db.PersonalItems.ToListAsync();
    var locationlist = await db.StarLocations.ToListAsync();

    db.PersonalItems.RemoveRange(itemlist);
    db.StarLocations.RemoveRange(locationlist);

    await db.SaveChangesAsync();

    return Results.Ok("Database Cleared");
});



app.Run();
// This is a comment
