
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddProblemDetails();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configure cors policies for the app
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

// Configuration for swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithAuth();


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



// Add HttpClient for DI for use with Uex API
builder.Services.AddHttpClient("UexApi", client =>
{
    client.BaseAddress = new Uri("https://api.uexcorp.uk/2.0/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});



// Adding custom services for dependency injection

builder.Services.AddSingleton<PasswordHasher>();

var app = builder.Build();




// Enable CORS Policies and Auth
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();


// Enable swagger if in dev mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });

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

// Mapping endpoints for the api
app.MapAdminEndpoints();
app.MapCacheEndpoints();
app.MapOrgEndpoints();
app.MapDevEndpoints();
app.MapPersonalEndpoints();



app.Run();
// This is a comment
