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

    public static async Task<Results<Created, BadRequest, Conflict>> AddRecipe(Recipe recipe, ItemCacheDb db)
    {
        var result = await RecipeDataStore.AddRecipe(db, recipe);
        return result;
    }

    public static async Task<Results<Ok, NotFound>> RemoveRecipe(ItemCacheDb db, int id)
    {
        var result = await RecipeDataStore.RemoveRecipe(db, id);
        if (!result) return TypedResults.NotFound();

        return TypedResults.Ok();
    }

    public static async Task<Results<Ok, NotFound>> EditRecipe(ItemCacheDb db, Recipe recipe, int id)
    {
        var result = await RecipeDataStore.UpdateRecipe(db, recipe, id);
        if (!result) return TypedResults.NotFound();

        return TypedResults.Ok();
    }
}