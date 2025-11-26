using Microsoft.EntityFrameworkCore;

public static class RecipeDataStore
{
    public static async Task<List<Recipe>> ListRecepies(ItemCacheDb db)
    {
        List<Recipe> recepies = await db.Recepies.ToListAsync();
        return recepies;
    }

    public static async Task<bool> AddRecipe(ItemCacheDb db, Recipe recipe)
    {

        recipe.Id = 0;
        if (recipe.UexItemIds.Length != recipe.ItemAmounts.Length)
        {
            return false;
        }

        db.Recepies.Add(recipe);
        await db.SaveChangesAsync();

        return true;
    }

    public static async Task<bool> RemoveRecipe(ItemCacheDb db, int id)
    {
        Recipe? existingRecipe = await db.Recepies.FindAsync(id);

        if (existingRecipe == null) return false;

        db.Recepies.Remove(existingRecipe);

        await db.SaveChangesAsync();
        
        return true;
    }

    public static async Task<bool> UpdateRecipe(ItemCacheDb db, Recipe recipe, int id)
    {
        Recipe? existingRecipe = await db.Recepies.FindAsync(id);

        if (existingRecipe == null) return false;

        if (recipe.UexItemIds.Length != recipe.ItemAmounts.Length) return false;

        existingRecipe.UexItemIds = recipe.UexItemIds;
        existingRecipe.ItemAmounts = recipe.ItemAmounts;

        await db.SaveChangesAsync();
        
        return true;
    }
}