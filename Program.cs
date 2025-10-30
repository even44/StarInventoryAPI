using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);



var connectionString = builder.Configuration.GetConnectionString("MariaDbConnection");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ItemCacheDb>(
    opt => opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
    .LogTo(Console.WriteLine, LogLevel.Information)
    .EnableSensitiveDataLogging()
    .EnableDetailedErrors()
);



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}


app.MapGet("/refreshcache", async (ItemCacheDb db) =>
{
    var items = await db.CacheItems.ToListAsync();
    return Results.Ok(items);
});

app.MapGet("/stefan", () => "Stefan er en tulling! Og veldig drit i star citizen!");

app.MapGet("/dev/randomitem", async (ItemCacheDb db) =>
{
    StarItem item = new StarItem();
    await db.PersonalItems.AddAsync(item);
    await db.SaveChangesAsync();
    return Results.Ok(item);
});

app.MapGet("/personal/items", async (ItemCacheDb db) =>
{
    List<StarItem> items;
    items = await db.PersonalItems.ToListAsync();
    return Results.Ok(items);
});

app.Run();
// This is a comment
