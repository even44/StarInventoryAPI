using Microsoft.EntityFrameworkCore;

public static class HostExtensions
{
    public static IHost CheckDatabaseConnection(this IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var dbContext = services.GetRequiredService<ItemCacheDb>(); // Replace with your DbContext
                if (!dbContext.Database.CanConnect())
                {
                    // Log or throw an exception indicating connection failure
                    Console.WriteLine("Error: Could not connect to the database on startup.");
                    throw new Exception("Database connection failed on startup.");
                }
                Console.WriteLine("Database connection successful.");
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log it, exit the application)
                Console.WriteLine($"An error occurred while connecting to the database: {ex.Message}");
                throw; // Re-throw to prevent application from starting with a broken connection
            }
        }
        return host;
    }

    public static IHost MigrateDatabase(this IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var dbContext = services.GetRequiredService<ItemCacheDb>();
                dbContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log it, exit the application)
                Console.WriteLine($"An error occurred while migrating the database: {ex.Message}");
                throw; // Re-throw to prevent application from starting with a broken connection
            }
        }
        return host;
    }
}