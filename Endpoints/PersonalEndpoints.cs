using StarInventoryAPI.Filters;
using StarInventoryAPI.Handlers;

namespace StarInventoryAPI.Endpoints
{
    public static class PersonalEndpoints
    {
        public static void MapPersonalEndpoints(this IEndpointRouteBuilder app)
        {
            var personalApi = app.MapGroup("/personal")
                .WithTags("Personal Items")
                .RequireAuthorization("user");

            // GET LIST (optional ?search= filter)
            personalApi.MapGet("/items", PersonalItemHandlers.GetAllPersonalItemsList);
            // GET ONE (int constraint takes precedence over the search route below)
            personalApi.MapGet("/items/{id:int}", PersonalItemHandlers.GetPersonalItem);
            // SEARCH (kept for compatibility, prefer GET /items?search=)
            personalApi.MapGet("/items/{searchTerm}", PersonalItemHandlers.GetAllPersonalItemsListSearch);
            // ADD ONE
            personalApi.MapPost("/items", PersonalItemHandlers.AddPersonalItemToInventory)
                .WithValidation<StarItem>();
            // UPDATE ONE
            personalApi.MapPut("/items/{id:int}", PersonalItemHandlers.UpdatePersonalItem)
                .WithValidation<StarItem>()
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .ProducesProblem(StatusCodes.Status404NotFound);
            // DELETE ONE
            personalApi.MapDelete("/items/{id:int}", PersonalItemHandlers.RemovePersonalItemFromInventory)
                .ProducesProblem(StatusCodes.Status403Forbidden);

            personalApi.MapDelete("/wipePersonalItems", PersonalItemHandlers.WipeAllPersonalItems);

        }
    }
}
