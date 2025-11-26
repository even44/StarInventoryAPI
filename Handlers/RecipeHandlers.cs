using Microsoft.AspNetCore.Http.HttpResults;

class RecipeHandlers
{
    public static async Task<Ok<List<Recipe>>> GetAllRecipesList(ItemCacheDb db)
    {
        List<Recipe> recipes = await RecipeDataStore.ListRecepies(db);
        return TypedResults.Ok(recipes);
    }

    public static async Task<Results<Created, BadRequest>> AddRecipe(Recipe recipe, ItemCacheDb db)
    {
        bool result = await RecipeDataStore.AddRecipe(db, recipe);
        if (!result) return TypedResults.BadRequest();

        return TypedResults.Created();
    }

    public static async Task<Results<Ok, NotFound>> RemoveRecipe(ItemCacheDb db, int id)
    {
        bool result = await RecipeDataStore.RemoveRecipe(db, id);
        if (!result) return TypedResults.NotFound();

        return TypedResults.Ok();
    }

    public static async Task<Results<Ok, NotFound>> EditRecipe(ItemCacheDb db, Recipe recipe, int id)
    {
        bool result = await RecipeDataStore.UpdateRecipe(db, recipe, id);
        if (!result) return TypedResults.NotFound();

        return TypedResults.Ok();
    }
}
