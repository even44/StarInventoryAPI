using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace StarInventoryAPI.Store;

public static class RecipeDataStore
{
    public static async Task<List<Recipe>> ListRecipes(ItemCacheDb db)
    {
        var recipes = await db.Recipes.ToListAsync();
        return recipes;
    }

    public static async Task<Results<Created, BadRequest, Conflict>> AddRecipe(ItemCacheDb db, Recipe recipe)
    {

        recipe.Id = 0;

        if (recipe.UexItemIds.Length == 0 || recipe.UexItemIds.Length != recipe.ItemAmounts.Length) return TypedResults.BadRequest();

        var existingRecipe = await db.Recipes.FirstOrDefaultAsync(r =>  r.Name == recipe.Name );
        if (existingRecipe != null) return TypedResults.Conflict();
        
        
        db.Recipes.Add(recipe);
        await db.SaveChangesAsync();

        return TypedResults.Created();
    }

    public static async Task<bool> RemoveRecipe(ItemCacheDb db, int id)
    {
        var existingRecipe = await db.Recipes.FindAsync(id);

        if (existingRecipe == null) return false;

        db.Recipes.Remove(existingRecipe);

        await db.SaveChangesAsync();

        return true;
    }

    public static async Task<bool> UpdateRecipe(ItemCacheDb db, Recipe recipe, int id)
    {
        var existingRecipe = await db.Recipes.FindAsync(id);

        if (existingRecipe == null) return false;

        if (recipe.UexItemIds.Length != recipe.ItemAmounts.Length) return false;

        existingRecipe.UexItemIds = recipe.UexItemIds;
        existingRecipe.ItemAmounts = recipe.ItemAmounts;
        existingRecipe.Name = recipe.Name;

        await db.SaveChangesAsync();
        
        return true;
    }
}