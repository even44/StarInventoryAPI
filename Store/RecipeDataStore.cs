using Microsoft.EntityFrameworkCore;
using StarInventoryAPI.Db;

namespace StarInventoryAPI.Store;

public enum RecipeWriteStatus
{
    Success,
    NotFound,
    NameConflict,
}

public record RecipeWriteResult(RecipeWriteStatus Status, Recipe? Recipe = null);

public static class RecipeDataStore
{
    public static async Task<List<Recipe>> ListRecipes(ItemCacheDb db)
    {
        var recipes = await db.Recipes.ToListAsync();
        return recipes;
    }

    public static async Task<Recipe?> GetRecipe(ItemCacheDb db, int id)
    {
        return await db.Recipes.FindAsync(id);
    }

    public static async Task<RecipeWriteResult> AddRecipe(ItemCacheDb db, Recipe recipe)
    {
        recipe.Id = 0;

        var existingRecipe = await db.Recipes.FirstOrDefaultAsync(r => r.Name == recipe.Name);
        if (existingRecipe != null) return new RecipeWriteResult(RecipeWriteStatus.NameConflict);

        db.Recipes.Add(recipe);
        await db.SaveChangesAsync();

        return new RecipeWriteResult(RecipeWriteStatus.Success, recipe);
    }

    public static async Task<bool> RemoveRecipe(ItemCacheDb db, int id)
    {
        var existingRecipe = await db.Recipes.FindAsync(id);

        if (existingRecipe == null) return false;

        db.Recipes.Remove(existingRecipe);

        await db.SaveChangesAsync();

        return true;
    }

    public static async Task<RecipeWriteResult> UpdateRecipe(ItemCacheDb db, Recipe recipe, int id)
    {
        var existingRecipe = await db.Recipes.FindAsync(id);

        if (existingRecipe == null) return new RecipeWriteResult(RecipeWriteStatus.NotFound);

        var nameTaken = await db.Recipes.AnyAsync(r => r.Id != id && r.Name == recipe.Name);
        if (nameTaken) return new RecipeWriteResult(RecipeWriteStatus.NameConflict);

        existingRecipe.UexItemIds = recipe.UexItemIds;
        existingRecipe.ItemAmounts = recipe.ItemAmounts;
        existingRecipe.Name = recipe.Name;

        await db.SaveChangesAsync();

        return new RecipeWriteResult(RecipeWriteStatus.Success, existingRecipe);
    }
}
