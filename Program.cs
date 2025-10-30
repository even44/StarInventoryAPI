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






app.MapGet("/dev/randomitem", async (ItemCacheDb db) =>
{
    StarItem item = StarItem.RandomItem();

    var existingItem = await db.PersonalItems.Include(item => item.Location).FirstOrDefaultAsync(i => i.Name == item.Name && i.Location.Name == item.Location.Name);
    if (existingItem == null)
    {
        await db.PersonalItems.AddAsync(item);
        await db.SaveChangesAsync();
        return Results.Created($"/personal/items/{item.Id}", item);
    }
    else
    {
        existingItem.Quantity += item.Quantity;

        await db.SaveChangesAsync();

        return Results.Created($"/personal/items/{existingItem.Id}", existingItem);
    }


});


// GET LIST
app.MapGet("/personal/items", async (ItemCacheDb db) =>
{
    List<StarItem> items;
    items = await db.PersonalItems.Include(item => item.Location).ToListAsync();
    return Results.Ok(items);
});

// ADD ONE
app.MapPost("/personal/items/{id}", async (StarItem item, ItemCacheDb db) =>
{
    // Check if a record with the same name and location already exists
    var existingItem = await db.PersonalItems.Include(item => item.Location).FirstOrDefaultAsync(i => i.Name == item.Name && i.Location == item.Location);
    if (existingItem == null)
    {
        db.PersonalItems.Add(item);

        await db.SaveChangesAsync();

        return Results.Created($"/personal/items/{item.Id}", item);
    }
    else
    {
        existingItem.Quantity += item.Quantity;

        await db.SaveChangesAsync();

        return Results.Created($"/personal/items/{item.Id}", existingItem);
    }



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
    StarItem? existingItem = await db.PersonalItems.FindAsync(item.Id);
    if (existingItem == null)
    {
        return Results.NotFound();
    }


    item.Id = id;

    existingItem.Name = item.Name;

    await db.SaveChangesAsync();

    return Results.Ok();
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
