using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using StarInventoryAPI.Tests.Infrastructure;

namespace StarInventoryAPI.Tests;

public class RecipeTests(StarInventoryApiFactory factory) : IClassFixture<StarInventoryApiFactory>
{
    private HttpClient OrgMember => factory.CreateClientFor(TestData.NewUsername(), Roles.User, Roles.Org);
    private HttpClient Member => factory.CreateClientFor(TestData.NewUsername(), Roles.User);

    [Fact]
    public async Task Created_recipe_is_listed_for_all_users()
    {
        var name = NewRecipeName();

        var response = await OrgMember.PostAsJsonAsync("/recipe/", NewRecipe(name, [1, 2], [3, 4]));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var recipe = (await ListRecipes()).Should().ContainSingle(r => r.Name == name).Subject;
        recipe.UexItemIds.Should().Equal(1, 2);
        recipe.ItemAmounts.Should().Equal(3, 4);
    }

    [Fact]
    public async Task Recipe_with_duplicate_name_is_a_conflict()
    {
        var name = NewRecipeName();
        await OrgMember.PostAsJsonAsync("/recipe/", NewRecipe(name, [1], [1]));

        var response = await OrgMember.PostAsJsonAsync("/recipe/", NewRecipe(name, [2], [2]));

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Theory]
    [InlineData(new int[0], new int[0])]
    [InlineData(new[] { 1, 2 }, new[] { 1 })]
    public async Task Recipe_with_invalid_ingredients_is_a_bad_request(int[] itemIds, int[] amounts)
    {
        var response = await OrgMember.PostAsJsonAsync("/recipe/", NewRecipe(NewRecipeName(), itemIds, amounts));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Editing_a_recipe_replaces_name_and_ingredients()
    {
        var recipe = await CreateRecipe(NewRecipeName());
        var newName = NewRecipeName();

        var response = await OrgMember.PutAsJsonAsync($"/recipe/{recipe.Id}", NewRecipe(newName, [7, 8, 9], [1, 1, 2]));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var edited = (await ListRecipes()).Single(r => r.Id == recipe.Id);
        edited.Name.Should().Be(newName);
        edited.UexItemIds.Should().Equal(7, 8, 9);
        edited.ItemAmounts.Should().Equal(1, 1, 2);
    }

    [Fact]
    public async Task Editing_a_missing_recipe_is_not_found()
    {
        var response = await OrgMember.PutAsJsonAsync("/recipe/987654", NewRecipe(NewRecipeName(), [1], [1]));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Deleting_a_recipe_removes_it()
    {
        var recipe = await CreateRecipe(NewRecipeName());

        var response = await OrgMember.DeleteAsync($"/recipe/{recipe.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await ListRecipes()).Should().NotContain(r => r.Id == recipe.Id);
        (await OrgMember.DeleteAsync($"/recipe/{recipe.Id}")).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<Recipe> CreateRecipe(string name)
    {
        var response = await OrgMember.PostAsJsonAsync("/recipe/", NewRecipe(name, [1], [1]));
        response.EnsureSuccessStatusCode();
        return (await ListRecipes()).Single(r => r.Name == name);
    }

    private async Task<List<Recipe>> ListRecipes() =>
        (await Member.GetFromJsonAsync<List<Recipe>>("/recipe/"))!;

    private static string NewRecipeName() => $"Recipe {Guid.NewGuid():N}";

    private static object NewRecipe(string name, int[] uexItemIds, int[] itemAmounts) =>
        new { id = 0, name, uexItemIds, itemAmounts };
}
