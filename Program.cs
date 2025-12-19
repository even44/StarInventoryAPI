using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StarInventoryAPI.Db;

var builder = WebApplication.CreateBuilder(args);

// Configuration for swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithAuth();

builder.Services.AddProblemDetails();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();



// Configure cors policies for the app
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

// Build the different policies and defining requred claims for authorization
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("dev", policy => policy.RequireRole(["Dev"]))
    .AddPolicy("admin", policy => policy.RequireRole(["Admin"]))
    .AddPolicy("organization", policy => policy.RequireRole(["Org"]))
    .AddPolicy("user", policy => policy.RequireRole(["User"]));


// Define the JWT Authentication Scheme
var tempHttp = new HttpClient();
var jwksJson = await tempHttp.GetAsync(builder.Configuration["Jwt:Issuer"] + "jwks/");
jwksJson.EnsureSuccessStatusCode();
var signingKey = JsonWebKeySet.Create(await jwksJson.Content.ReadAsStringAsync()).Keys.First();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.TokenValidationParameters = new TokenValidationParameters

        {
            IssuerSigningKey = signingKey,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:ClientId"],
            ClockSkew = TimeSpan.Zero
        };
    });


// Define the database context for the app
var connectionString = builder.Configuration.GetConnectionString("MariaDbConnection");
builder.Services.AddDbContext<ItemCacheDb>(
    opt => opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
    .LogTo(Console.WriteLine, LogLevel.Information)
    .EnableSensitiveDataLogging()
    .EnableDetailedErrors()
);
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ItemCacheDb>();


// Add HttpClient for DI for use with Uex API
builder.Services.AddHttpClient("UexApi", client =>
{
    client.BaseAddress = new Uri("https://api.uexcorp.uk/2.0/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});



// Adding custom services for dependency injection
var app = builder.Build();

app.CheckDatabaseConnection();
app.MigrateDatabase();



app.Logger.LogInformation("Database connection verified");

app.Logger.LogInformation("Enableing Middleware");

// Enable CORS Policies and Auth
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();


// Enable swagger if in dev mode
if (app.Environment.IsDevelopment())
{
    app.Logger.LogInformation("Enabeling Swagger");
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });

    app.Logger.LogInformation("Enabeling Status Code Pages");
    app.UseStatusCodePages(statusCodeHandlerApp =>
    {
        statusCodeHandlerApp.Run(async httpContext =>
        {
            var pds = httpContext.RequestServices.GetService<IProblemDetailsService>();
            if (pds == null
                || !await pds.TryWriteAsync(new() { HttpContext = httpContext }))
            {
                await httpContext.Response.WriteAsync("Fallback: An error occurred.");
            }
        });
    });
}

app.Logger.LogInformation("Mapping Endpoints");

app.MapHealthChecks("/health");
// Mapping endpoints for the api
app.MapAdminEndpoints();
app.MapCacheEndpoints();
app.MapOrgEndpoints();
app.MapDevEndpoints();
app.MapPersonalEndpoints();
app.MapRecipeEndpoints();


app.Logger.LogInformation("Starting App");
app.Run();
// This is a comment
