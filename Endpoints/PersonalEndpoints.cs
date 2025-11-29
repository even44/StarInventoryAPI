public static class PersonalEndpoints
{
    public static void MapPersonalEndpoints(this IEndpointRouteBuilder app)
    {
        var personalApi = app.MapGroup("/personal")
            .WithTags("Personal Items")
            .RequireAuthorization("user");

        // GET LIST
        personalApi.MapGet("/items", PersonalItemHandlers.GetAllPersonalItemsList);

        personalApi.MapGet("/items/{searchTerm}", PersonalItemHandlers.GetAllPersonalItemsListSearch);
        // ADD ONE
        personalApi.MapPost("/items", PersonalItemHandlers.AddPersonalItemToInventory);
        // DELETE ONE
        personalApi.MapDelete("/items/{id}", PersonalItemHandlers.RemovePersonalItemFromInventory);

        personalApi.MapPut("/items/{id}", PersonalItemHandlers.GetOnePersonalItemFromId);

        personalApi.MapDelete("/wipePersonalItems", PersonalItemHandlers.WipeAllPersonalItems);



    }
}
