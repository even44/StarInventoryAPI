using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StarInventoryAPI.Tests.Infrastructure;

namespace StarInventoryAPI.Tests;

public class PersonalInventoryTests(StarInventoryApiFactory factory) : IClassFixture<StarInventoryApiFactory>
{
    private readonly string _username = TestData.NewUsername();
    private HttpClient Client => factory.CreateClientFor(_username, Roles.User);

    [Fact]
    public async Task Adding_an_item_stores_it_for_the_token_user()
    {
        var response = await Client.PostAsJsonAsync("/personal/items", TestData.NewItem("Pyro RYT Multi-Tool", locationId: 1, quantity: 2));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<StarItem>();
        created!.Id.Should().BePositive();
        created.Username.Should().Be(_username, "the username comes from the token, not the request body");
        created.Quantity.Should().Be(2);
        response.Headers.Location!.ToString().Should().Be($"/personal/items/{created.Id}");
    }

    [Fact]
    public async Task Adding_an_item_registers_the_user()
    {
        await Client.PostAsJsonAsync("/personal/items", TestData.NewItem("Medpen", locationId: 1));

        await factory.WithDbAsync(async db =>
            (await db.Users.AnyAsync(u => u.Username == _username)).Should().BeTrue());
    }

    [Fact]
    public async Task Adding_an_item_to_an_unknown_location_is_a_bad_request()
    {
        var response = await Client.PostAsJsonAsync("/personal/items", TestData.NewItem("Medpen", locationId: 999));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Adding_the_same_item_at_the_same_location_merges_quantities()
    {
        await Client.PostAsJsonAsync("/personal/items", TestData.NewItem("Hydrogen Fuel", locationId: 2, quantity: 3));
        await Client.PostAsJsonAsync("/personal/items", TestData.NewItem("Hydrogen Fuel", locationId: 2, quantity: 4));

        var items = await GetItems(Client);

        items.Should().ContainSingle(i => i.Name == "Hydrogen Fuel").Which.Quantity.Should().Be(7);
    }

    [Fact]
    public async Task Listing_items_only_returns_the_callers_items()
    {
        var other = factory.CreateClientFor(TestData.NewUsername(), Roles.User);
        await Client.PostAsJsonAsync("/personal/items", TestData.NewItem("Mine", locationId: 1));
        await other.PostAsJsonAsync("/personal/items", TestData.NewItem("Theirs", locationId: 1));

        var items = await GetItems(Client);

        items.Should().OnlyContain(i => i.Username == _username);
        items.Select(i => i.Name).Should().Contain("Mine").And.NotContain("Theirs");
    }

    [Fact]
    public async Task Searching_items_matches_name_case_insensitively()
    {
        await Client.PostAsJsonAsync("/personal/items", TestData.NewItem("Quantainium", locationId: 1));
        await Client.PostAsJsonAsync("/personal/items", TestData.NewItem("Laranite", locationId: 1));

        var items = await Client.GetFromJsonAsync<List<StarItem>>("/personal/items/QUANT");

        items.Should().ContainSingle().Which.Name.Should().Be("Quantainium");
    }

    [Fact]
    public async Task Updating_an_item_changes_quantity_and_sharing()
    {
        var item = await AddItem("Scrap", locationId: 1, quantity: 5);

        var response = await Client.PutAsJsonAsync($"/personal/items/{item.Id}",
            TestData.NewItem("Scrap", locationId: 1, quantity: 9, shared: true));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = (await GetItems(Client)).Single(i => i.Id == item.Id);
        updated.Quantity.Should().Be(9);
        updated.IsSharedWithOrganization.Should().BeTrue();
    }

    [Fact]
    public async Task Moving_an_item_to_a_new_location_moves_it()
    {
        var item = await AddItem("Armor", locationId: 1, quantity: 1, uexIdentifier: 501);

        await Client.PutAsJsonAsync($"/personal/items/{item.Id}",
            TestData.NewItem("Armor", locationId: 3, quantity: 1, uexIdentifier: 501));

        (await GetItems(Client)).Single(i => i.Id == item.Id).LocationId.Should().Be(3);
    }

    [Fact]
    public async Task Moving_part_of_a_stack_onto_an_existing_stack_merges_them()
    {
        var source = await AddItem("Ammo", locationId: 1, quantity: 10, uexIdentifier: 777);
        var target = await AddItem("Ammo", locationId: 2, quantity: 5, uexIdentifier: 777);

        var response = await Client.PutAsJsonAsync($"/personal/items/{source.Id}",
            TestData.NewItem("Ammo", locationId: 2, quantity: 4, uexIdentifier: 777));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await GetItems(Client);
        items.Single(i => i.Id == source.Id).Quantity.Should().Be(6);
        items.Single(i => i.Id == target.Id).Quantity.Should().Be(9);
    }

    [Fact]
    public async Task Moving_a_whole_stack_onto_an_existing_stack_removes_the_source()
    {
        var source = await AddItem("Gem", locationId: 1, quantity: 2, uexIdentifier: 888);
        var target = await AddItem("Gem", locationId: 2, quantity: 1, uexIdentifier: 888);

        await Client.PutAsJsonAsync($"/personal/items/{source.Id}",
            TestData.NewItem("Gem", locationId: 2, quantity: 2, uexIdentifier: 888));

        var items = await GetItems(Client);
        items.Should().NotContain(i => i.Id == source.Id);
        items.Single(i => i.Id == target.Id).Quantity.Should().Be(3);
    }

    [Fact]
    public async Task Deleting_an_item_removes_it()
    {
        var item = await AddItem("Trash", locationId: 1);

        var response = await Client.DeleteAsync($"/personal/items/{item.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await GetItems(Client)).Should().NotContain(i => i.Id == item.Id);
    }

    [Fact]
    public async Task Deleting_a_missing_item_is_not_found()
    {
        var response = await Client.DeleteAsync("/personal/items/987654");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Users_cannot_delete_or_update_other_users_items()
    {
        var item = await AddItem("Precious", locationId: 1, quantity: 3);
        var intruder = factory.CreateClientFor(TestData.NewUsername(), Roles.User);

        var delete = await StatusOf(() => intruder.DeleteAsync($"/personal/items/{item.Id}"));
        var update = await StatusOf(() => intruder.PutAsJsonAsync($"/personal/items/{item.Id}", TestData.NewItem("Precious", locationId: 1, quantity: 0)));

        delete.Should().NotBe(HttpStatusCode.OK);
        update.Should().NotBe(HttpStatusCode.OK);
        (await GetItems(Client)).Single(i => i.Id == item.Id).Quantity.Should().Be(3);
    }

    [Fact]
    public async Task Wiping_personal_items_only_removes_the_callers_items()
    {
        var other = factory.CreateClientFor(TestData.NewUsername(), Roles.User);
        await Client.PostAsJsonAsync("/personal/items", TestData.NewItem("Mine", locationId: 1));
        await other.PostAsJsonAsync("/personal/items", TestData.NewItem("Theirs", locationId: 1));

        var response = await Client.DeleteAsync("/personal/wipePersonalItems");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await GetItems(Client)).Should().BeEmpty();
        (await GetItems(other)).Should().ContainSingle(i => i.Name == "Theirs");
    }

    private async Task<StarItem> AddItem(string name, int locationId, int quantity = 1, int uexIdentifier = 100)
    {
        var response = await Client.PostAsJsonAsync("/personal/items", TestData.NewItem(name, locationId, quantity, uexIdentifier));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<StarItem>())!;
    }

    // The API currently throws UnauthorizedAccessException for foreign items (a 500 under Kestrel);
    // TestServer rethrows it to the caller instead, so treat a thrown exception as a 500
    private static async Task<HttpStatusCode> StatusOf(Func<Task<HttpResponseMessage>> request)
    {
        try
        {
            return (await request()).StatusCode;
        }
        catch (UnauthorizedAccessException)
        {
            return HttpStatusCode.InternalServerError;
        }
    }

    private static async Task<List<StarItem>> GetItems(HttpClient client) =>
        (await client.GetFromJsonAsync<List<StarItem>>("/personal/items"))!;
}
