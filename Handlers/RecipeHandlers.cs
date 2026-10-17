using Microsoft.AspNetCore.Http.HttpResults;
using StarInventoryAPI.Db;
using StarInventoryAPI.Store;

namespace StarInventoryAPI.Handlers;

internal static class RecipeHandlers
{
    public static async Task<Ok<List<Recipe>>> GetAllRecipesList(ItemCacheDb db)
    {
        var recipes = await RecipeDataStore.ListRecipes(db);
        return TypedResults.Ok(recipes);
    }

    public static async Task<Results<Ok<Recipe>, NotFound>> GetRecipe(ItemCacheDb db, int id)
    {
        var recipe = await RecipeDataStore.GetRecipe(db, id);
        if (recipe == null) return TypedResults.NotFound();

        return TypedResults.Ok(recipe);
    }

    public static async Task<Results<Created<Recipe>, Conflict<string>>> AddRecipe(Recipe recipe, ItemCacheDb db)
    {
        var result = await RecipeDataStore.AddRecipe(db, recipe);
        if (result.Status == RecipeWriteStatus.NameConflict) return TypedResults.Conflict("A recipe with this name already exists.");

        return TypedResults.Created($"/recipe/{result.Recipe!.Id}", result.Recipe);
    }

    public static async Task<Results<Ok, NotFound>> RemoveRecipe(ItemCacheDb db, int id)
    {
        var result = await RecipeDataStore.RemoveRecipe(db, id);
        if (!result) return TypedResults.NotFound();

        return TypedResults.Ok();
    }

    public static async Task<Results<Ok<Recipe>, NotFound, Conflict<string>>> EditRecipe(ItemCacheDb db, Recipe recipe, int id)
    {
        var result = await RecipeDataStore.UpdateRecipe(db, recipe, id);
        return result.Status switch
        {
            RecipeWriteStatus.NotFound => TypedResults.NotFound(),
            RecipeWriteStatus.NameConflict => TypedResults.Conflict("A recipe with this name already exists."),
            _ => TypedResults.Ok(result.Recipe!),
        };
    }
}
