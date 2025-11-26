using Microsoft.AspNetCore.Http.HttpResults;

public static class RecipeEndpoints
{
    public static void MapRecipeEndpoints(this IEndpointRouteBuilder app)
    {
        var recipeApi = app.MapGroup("/recipe").WithTags("Recipes").RequireAuthorization("user");
        
        recipeApi.MapGet("/", async Task<Ok<List<Recipe>>> (ItemCacheDb db) =>
        {
            List<Recipe> recipes = await RecipeDataStore.ListRecepies(db);
            return TypedResults.Ok(recipes);
        });

        recipeApi.MapPost("/", async Task<Results<Created, BadRequest>> (Recipe recipe, ItemCacheDb db) =>
        {
            bool result = await RecipeDataStore.AddRecipe(db, recipe);
            if (!result) return TypedResults.BadRequest();

            return TypedResults.Created();
            
        }).RequireAuthorization("org");

        recipeApi.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (ItemCacheDb db, int id) =>
        {
            bool result = await RecipeDataStore.RemoveRecipe(db, id);
            if (!result) return TypedResults.NotFound();

            return TypedResults.Ok();
        }).RequireAuthorization("org");

        recipeApi.MapPut("/{id}", async Task<Results<Ok, NotFound>> (ItemCacheDb db, Recipe recipe, int id) =>
        {
            bool result = await RecipeDataStore.UpdateRecipe(db, recipe, id);
            if (!result) return TypedResults.NotFound();

            return TypedResults.Ok();
        }).RequireAuthorization("org");
    }

}