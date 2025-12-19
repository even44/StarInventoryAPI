using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace StarInventoryAPI.Handlers;

internal static class DevHandlers
{

    public static async Task<Results<Ok<CacheUpdateResponse>, InternalServerError>> UpdateCacheFromUex(bool updateItems, bool updateLocations, ItemCacheDb db, IHttpClientFactory httpClientFactory)
    {
        var client = httpClientFactory.CreateClient("UexApi");

        if (updateItems)
        {
            var catResult = await db.UpdateCategories(db, client);
            if (!catResult)
            {
                Console.WriteLine("Failed to update categories");
                return TypedResults.InternalServerError();
            }
            var itemResult = await db.UpdateItems(db, client);

            if (!itemResult)
            {
                Console.WriteLine("Failed to update items");
                return TypedResults.InternalServerError();
            }
        }

        if (!updateLocations) return TypedResults.Ok(await db.GetCacheUpdateResponse(db));
        var poiResult = await db.UpdatePois(db, client);
        if (!poiResult)
        {
            Console.WriteLine("Failed to update pois");
            return TypedResults.InternalServerError();
        }
        var spaceStationResult = await db.UpdateSpaceStations(db, client);
        if (!spaceStationResult)
        {
            Console.WriteLine("Failed to update space stations");
            return TypedResults.InternalServerError();
        }
        var cityResult = await db.UpdateCities(db, client);
        if (!cityResult)
        {
            Console.WriteLine("Failed to update cities");
            return TypedResults.InternalServerError();
        }

        var locationMergeResult = await db.CompileLocations(db);
        if (locationMergeResult) return TypedResults.Ok(await db.GetCacheUpdateResponse(db));
        Console.WriteLine("Failed to compile locations");
        return TypedResults.InternalServerError();
    }

    // Server-Sent Events endpoint for cache update with status messages
    public static async Task UpdateCacheFromUexWithStatus(HttpContext context, bool updateItems, bool updateLocations, string? token, ItemCacheDb db, IHttpClientFactory httpClientFactory)
    {
        context.Response.Headers.Append("Content-Type", "text/event-stream");
        context.Response.Headers.Append("Cache-Control", "no-cache");
        context.Response.Headers.Append("Connection", "keep-alive");

        await context.Response.Body.FlushAsync();

        try
        {
            // Validate token from query parameter
            if (string.IsNullOrWhiteSpace(token))
            {
                await SendErrorEvent(context, "Authentication token is required");
                return;
            }

            // Validate the JWT token
            var isValid = await ValidateTokenAsync(context, token);
            if (!isValid)
            {
                await SendErrorEvent(context, "Invalid or expired authentication token");
                return;
            }
            var client = httpClientFactory.CreateClient("UexApi");
            int currentStep = 0;
            int totalSteps = (updateItems ? 2 : 0) + (updateLocations ? 4 : 0);

            if (updateItems)
            {
                currentStep++;
                await SendStatusUpdate(context, $"Updating categories... ({currentStep}/{totalSteps})", (currentStep * 100) / totalSteps);
                var catResult = await db.UpdateCategories(db, client);
                if (!catResult)
                {
                    await SendErrorEvent(context, "Failed to update categories");
                    return;
                }

                currentStep++;
                await SendStatusUpdate(context, $"Updating items... ({currentStep}/{totalSteps})", (currentStep * 100) / totalSteps);
                var itemResult = await db.UpdateItems(db, client);
                if (!itemResult)
                {
                    await SendErrorEvent(context, "Failed to update items");
                    return;
                }
            }

            if (updateLocations)
            {
                currentStep++;
                await SendStatusUpdate(context, $"Updating POIs... ({currentStep}/{totalSteps})", (currentStep * 100) / totalSteps);
                var poiResult = await db.UpdatePois(db, client);
                if (!poiResult)
                {
                    await SendErrorEvent(context, "Failed to update pois");
                    return;
                }

                currentStep++;
                await SendStatusUpdate(context, $"Updating space stations... ({currentStep}/{totalSteps})", (currentStep * 100) / totalSteps);
                var spaceStationResult = await db.UpdateSpaceStations(db, client);
                if (!spaceStationResult)
                {
                    await SendErrorEvent(context, "Failed to update space stations");
                    return;
                }

                currentStep++;
                await SendStatusUpdate(context, $"Updating cities... ({currentStep}/{totalSteps})", (currentStep * 100) / totalSteps);
                var cityResult = await db.UpdateCities(db, client);
                if (!cityResult)
                {
                    await SendErrorEvent(context, "Failed to update cities");
                    return;
                }

                currentStep++;
                await SendStatusUpdate(context, $"Compiling locations... ({currentStep}/{totalSteps})", (currentStep * 100) / totalSteps);
                var locationMergeResult = await db.CompileLocations(db);
                if (!locationMergeResult)
                {
                    await SendErrorEvent(context, "Failed to compile locations");
                    return;
                }
            }

            // Get the final response
            var response = await db.GetCacheUpdateResponse(db);
            
            // Send completion status
            await SendStatusUpdate(context, "Cache update completed successfully", 100);
            
            // Send the actual data
            await SendDataEvent(context, response);
        }
        catch (Exception ex)
        {
            await SendErrorEvent(context, ex.Message);
        }
    }

    private static async Task SendStatusUpdate(HttpContext context, string message, int progress)
    {
        var statusData = new { status = "progress", message, progress };
        var json = JsonSerializer.Serialize(statusData);
        await context.Response.WriteAsync($"event: status\ndata: {json}\n\n");
        await context.Response.Body.FlushAsync();
    }

    private static async Task SendDataEvent(HttpContext context, CacheUpdateResponse data)
    {
        var json = JsonSerializer.Serialize(data);
        await context.Response.WriteAsync($"event: complete\ndata: {json}\n\n");
        await context.Response.Body.FlushAsync();
    }

    private static async Task SendErrorEvent(HttpContext context, string error)
    {
        var errorData = new { status = "error", message = error };
        var json = JsonSerializer.Serialize(errorData);
        await context.Response.WriteAsync($"event: error\ndata: {json}\n\n");
        await context.Response.Body.FlushAsync();
    }

    private static async Task<bool> ValidateTokenAsync(HttpContext context, string token)
    {
        try
        {
            var configuration = context.RequestServices.GetRequiredService<IConfiguration>();
            
            // Get the JWKS from the issuer
            var httpClientFactory = context.RequestServices.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient();
            var jwksJson = await httpClient.GetAsync(configuration["Jwt:Issuer"] + "jwks/");
            
            if (!jwksJson.IsSuccessStatusCode)
            {
                return false;
            }
            
            var jwksContent = await jwksJson.Content.ReadAsStringAsync();
            var signingKey = JsonWebKeySet.Create(jwksContent).Keys.First();
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = signingKey,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:ClientId"],
                ClockSkew = TimeSpan.Zero,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true
            };
            
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            
            // Check if user has the required "Dev" role
            if (!principal.IsInRole("Dev"))
            {
                return false;
            }
            
            // Set the user in the HttpContext so it can be used elsewhere if needed
            context.User = principal;
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token validation failed: {ex.Message}");
            return false;
        }
    }
}