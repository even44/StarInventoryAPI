
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";


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
    .AddPolicy("dev", policy => policy.RequireClaim(ClaimTypes.Role, ["dev"]))
    .AddPolicy("admin", policy => policy.RequireClaim(ClaimTypes.Role, ["admin", "dev"]))
    .AddPolicy("organization", policy => policy.RequireClaim(ClaimTypes.Role, ["org", "admin", "dev"]))
    .AddPolicy("user", policy => policy.RequireClaim(ClaimTypes.Role, ["user", "org", "admin", "dev"]));


// Define the JWT Authentication Scheme
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
builder.Services.AddSingleton<TokenProvider>();
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
}

// Mapping endpoints for the api
app.MapAuthEndpoints();
app.MapAdminEndpoints();
app.MapCacheEndpoints();
app.MapOrgEndpoints();
app.MapDevEndpoints();
app.MapPersonalEndpoints();



app.Run();
// This is a comment
