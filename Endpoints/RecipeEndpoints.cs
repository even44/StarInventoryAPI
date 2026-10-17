using StarInventoryAPI.Filters;
using StarInventoryAPI.Handlers;

namespace StarInventoryAPI.Endpoints
{
    public static class RecipeEndpoints
    {
        public static void MapRecipeEndpoints(this IEndpointRouteBuilder app)
        {
            var recipeApi = app.MapGroup("/recipe").WithTags("Recipes").RequireAuthorization("user");

            recipeApi.MapGet("/", RecipeHandlers.GetAllRecipesList);

            recipeApi.MapGet("/{id:int}", RecipeHandlers.GetRecipe);

            recipeApi.MapPost("/", RecipeHandlers.AddRecipe).WithValidation<Recipe>().RequireAuthorization("organization");

            recipeApi.MapDelete("/{id:int}", RecipeHandlers.RemoveRecipe).RequireAuthorization("organization");

            recipeApi.MapPut("/{id:int}", RecipeHandlers.EditRecipe).WithValidation<Recipe>().RequireAuthorization("organization");
        }

    }

}
