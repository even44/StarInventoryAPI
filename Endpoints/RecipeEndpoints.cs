using StarInventoryAPI.Handlers;

public static class RecipeEndpoints
{
    public static void MapRecipeEndpoints(this IEndpointRouteBuilder app)
    {
        var recipeApi = app.MapGroup("/recipe").WithTags("Recipes").RequireAuthorization("user");
        
        recipeApi.MapGet("/", RecipeHandlers.GetAllRecipesList);

        recipeApi.MapPost("/", RecipeHandlers.AddRecipe).RequireAuthorization("organization");

        recipeApi.MapDelete("/{id}", RecipeHandlers.RemoveRecipe).RequireAuthorization("organization");

        recipeApi.MapPut("/{id}", RecipeHandlers.EditRecipe).RequireAuthorization("organization");
    }

}