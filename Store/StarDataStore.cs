


using Microsoft.EntityFrameworkCore;

public static class StarDataStore
{


    public static async Task<StarItem?> AddStarItem(ItemCacheDb db, StarItem item, string username)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
        {
            Console.WriteLine($"Invalid User when adding item");
            return null;
        }
        else
        {
            item.Username = username;
        }


        var location = await db.StarLocations.FirstOrDefaultAsync(loc => loc.Id == item.LocationId);
        if (location == null)
        {
            Console.WriteLine($"Invalid Location when adding item");
            return null;
        }

        var existingItem = await db.PersonalItems.FirstOrDefaultAsync(i => i.Name == item.Name && i.LocationId == item.LocationId);
        if (existingItem == null)
        {
            db.PersonalItems.Add(item);
            await db.SaveChangesAsync();
            return item;
        }
        else
        {
            existingItem.Quantity += item.Quantity;
            await db.SaveChangesAsync();
            return existingItem;
        }

    }

    public static async Task<StarItem> UpdateStarItem(ItemCacheDb db, int id, StarItem item, string username)
    {
        StarItem? existingItem = await db.PersonalItems.FindAsync(id);
        if (existingItem == null)
        {
            throw new KeyNotFoundException("Item not found");
        }

        if (existingItem.Username != username)
        {
            throw new UnauthorizedAccessException("You do not have permission to update this item");
        }



        item.Id = id;

        existingItem.Name = item.Name;
        existingItem.IsSharedWithOrganization = item.IsSharedWithOrganization;
        existingItem.Quantity = item.Quantity;

        await db.SaveChangesAsync();

        return existingItem;
    }

    public static async Task<bool> DeleteStarItem(ItemCacheDb db, int id, string username)
    {
        StarItem? existingItem = await db.PersonalItems.FindAsync(id);
        if (existingItem != null)
        {

            if (existingItem.Username != username)
            {
                throw new UnauthorizedAccessException("You do not have permission to update this item");
            }

            db.PersonalItems.Remove(existingItem);
            await db.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public static async Task<List<StarItem>> GetPersonalItems(ItemCacheDb db, string username)
    {
        List<StarItem> items = await db.PersonalItems.Where(user => user.Username.Equals(username)).ToListAsync();
        return items;
    }

    public static async Task<List<StarItem>> SearchPersonalItems(ItemCacheDb db, string username, string searchTerm)
    {
        List<StarItem> items = await db.PersonalItems.Where(item => item.Name.ToLower().Contains(searchTerm.ToLower()) && item.Username == username).Take(10).ToListAsync();
        return items;
    }

    public static async Task<List<StarLocation>> GetStarLocations(ItemCacheDb db)
    {
        List<StarLocation> locations = await db.StarLocations.ToListAsync();
        return locations;
    }

    public static async Task<List<StarLocation>> SearchStarLocations(ItemCacheDb db, string searchTerm)
    {
        List<StarLocation> locations = await db.StarLocations.Where(loc => loc.Name.ToLower().Contains(searchTerm.ToLower())).Take(10).ToListAsync();
        return locations;
    }

    public static async Task<List<UexPoi>> GetUexPois(ItemCacheDb db)
    {
        List<UexPoi> pois = await db.UexPois.ToListAsync();
        return pois;
    }

    public static async Task<List<UexCategory>> GetUexCategories(ItemCacheDb db)
    {
        List<UexCategory> categories = await db.UexCategories.ToListAsync();
        return categories;
    }

    public static async Task<List<UexItem>> GetUexItems(ItemCacheDb db)
    {
        List<UexItem> items = await db.UexItems.ToListAsync();
        return items;
    }

    public static async Task<List<UexItem>> SearchUexItems(ItemCacheDb db, string searchTerm)
    {
        List<UexItem> items = await db.UexItems.Where(item => item.Name.ToLower().Contains(searchTerm.ToLower())).Take(10).ToListAsync();
        return items;
    }

    public static async Task<List<UexSpaceStation>> GetStarSpaceStations(ItemCacheDb db)
    {
        List<UexSpaceStation> stations = await db.UexSpaceStations.ToListAsync();
        return stations;
    }


    // USER actions

    public static async Task<List<User>> GetUsers(ItemCacheDb db)
    {
        return await db.Users.ToListAsync();
    }

    public static async Task<User?> GetUser(string username, ItemCacheDb db)
    {
        return await db.Users.FindAsync(username);
    }

    public static async Task<bool> CreateUser(UserLogin newUserLogin, int roleId, ItemCacheDb db, PasswordHasher passwordHasher)
    {
        User? existingUser = await db.Users.FindAsync(newUserLogin.Username);
        if (existingUser != null)
        {
            return false;
        }

        User newUser = new User();
        string passwordHash = passwordHasher.HashPassword(newUserLogin.Password, newUser);
        newUser.Username = newUserLogin.Username;
        newUser.PasswordHash = passwordHash;
        newUser.RoleId = roleId;

        db.Users.Add(newUser);
        await db.SaveChangesAsync();

        return true;
    }


    public static async Task<Role?> GetRole(int id, ItemCacheDb db)
    {
        return await db.Roles.FindAsync(id);
    }

    public static async Task<Role?> GetRoleByClaim(string claimString, ItemCacheDb db)
    {
        return await db.Roles.Where(role => role.ClaimString == claimString).FirstOrDefaultAsync();
    }
    public static async Task<Role?> GetRoleByName(string name, ItemCacheDb db)
    {
        return await db.Roles.Where(role => role.Name == name).FirstOrDefaultAsync();
    }

    public static async Task<List<Role>> GetRoles(ItemCacheDb db)
    {
        return await db.Roles.ToListAsync();
    }
    
    public static async Task<bool> CreateRole(string name, string claimString, ItemCacheDb db)
    {

        Role? existingRole = await GetRoleByClaim(claimString, db);
        if (existingRole != null)
        {
            return false;
        }
        existingRole = await GetRoleByName(claimString, db);
        if (existingRole != null)
        {
            return false;
        }

        await db.Roles.AddAsync(new Role
        {
            Id = 0,
            Name = name,
            ClaimString = claimString
        });
        await db.SaveChangesAsync();
        return true;
    }

    public static async Task<bool> ChangeUserRole(string username, string role, ItemCacheDb db)
    {
        
        return true;
    }
}
