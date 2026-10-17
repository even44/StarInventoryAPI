using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using StarInventoryAPI.Tests.Infrastructure;

namespace StarInventoryAPI.Tests;

public class OrgInventoryTests(StarInventoryApiFactory factory) : IClassFixture<StarInventoryApiFactory>
{
    private HttpClient OrgAdmin => factory.CreateClientFor(TestData.NewUsername(), Roles.User, Roles.Org);

    [Fact]
    public async Task Org_inventory_aggregates_shared_items_of_participating_users_only()
    {
        var alice = TestData.NewUsername();
        var bob = TestData.NewUsername();
        var outsider = TestData.NewUsername();
        await AddItem(alice, "Alice shared", shared: true);
        await AddItem(alice, "Alice private", shared: false);
        await AddItem(bob, "Bob shared", shared: true);
        await AddItem(outsider, "Outsider shared", shared: true);
        await AddParticipant(alice);
        await AddParticipant(bob);

        var inventory = await GetOrgInventory();

        var names = inventory.Select(i => i.Name).ToList();
        names.Should().Contain(["Alice shared", "Bob shared"]);
        names.Should().NotContain(["Alice private", "Outsider shared"]);
    }

    [Fact]
    public async Task Removing_a_participant_removes_their_items_from_the_org_inventory()
    {
        var carol = TestData.NewUsername();
        await AddItem(carol, "Carol shared", shared: true);
        await AddParticipant(carol);

        var response = await OrgAdmin.DeleteAsync($"/organization/participatingusers/{carol}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await GetOrgInventory()).Should().NotContain(i => i.Username == carol);
    }

    [Fact]
    public async Task Adding_a_participant_twice_is_a_conflict_regardless_of_case()
    {
        var dave = TestData.NewUsername();
        await AddParticipant(dave);

        var response = await OrgAdmin.PostAsJsonAsync("/organization/participatingusers", new { username = dave.ToUpperInvariant() });

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Removing_an_unknown_participant_is_not_found()
    {
        var response = await OrgAdmin.DeleteAsync($"/organization/participatingusers/{TestData.NewUsername()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Participating_users_are_listed()
    {
        var erin = TestData.NewUsername();
        await AddParticipant(erin);

        var users = await factory.CreateClientFor(TestData.NewUsername(), Roles.User)
            .GetFromJsonAsync<List<OrgInventoryUser>>("/organization/participatingusers");

        users!.Select(u => u.Username).Should().Contain(erin);
    }

    [Fact]
    public async Task Users_list_contains_users_that_have_used_the_api()
    {
        var frank = TestData.NewUsername();
        await AddItem(frank, "Anything", shared: false);

        var users = await factory.CreateClientFor(TestData.NewUsername(), Roles.User)
            .GetFromJsonAsync<List<User>>("/organization/users");

        users!.Select(u => u.Username).Should().Contain(frank);
    }

    private async Task AddItem(string username, string name, bool shared)
    {
        var response = await factory.CreateClientFor(username, Roles.User)
            .PostAsJsonAsync("/personal/items", TestData.NewItem(name, locationId: 1, shared: shared));
        response.EnsureSuccessStatusCode();
    }

    private async Task AddParticipant(string username)
    {
        var response = await OrgAdmin.PostAsJsonAsync("/organization/participatingusers", new { username });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<List<StarItem>> GetOrgInventory() =>
        (await factory.CreateClientFor(TestData.NewUsername(), Roles.User)
            .GetFromJsonAsync<List<StarItem>>("/organization/inventory"))!;
}
