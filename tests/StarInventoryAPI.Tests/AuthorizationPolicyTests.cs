using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using StarInventoryAPI.Tests.Infrastructure;

namespace StarInventoryAPI.Tests;

public class AuthorizationPolicyTests(StarInventoryApiFactory factory) : IClassFixture<StarInventoryApiFactory>
{
    // One GET per endpoint group, each paired with the role its policy requires
    public static TheoryData<string, string> ProtectedEndpoints => new()
    {
        { "/personal/items", Roles.User },
        { "/cache/locations", Roles.User },
        { "/organization/inventory", Roles.User },
        { "/recipe/", Roles.User },
        { "/admin/users", Roles.Admin },
    };

    [Theory]
    [MemberData(nameof(ProtectedEndpoints))]
    public async Task Request_without_token_is_unauthorized(string url, string _)
    {
        var response = await factory.CreateClient().GetAsync(url);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [MemberData(nameof(ProtectedEndpoints))]
    public async Task Request_with_required_role_is_allowed(string url, string role)
    {
        var response = await factory.CreateClientFor(TestData.NewUsername(), role).GetAsync(url);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [MemberData(nameof(ProtectedEndpoints))]
    public async Task Request_without_required_role_is_forbidden(string url, string role)
    {
        var otherRole = role == Roles.User ? Roles.Admin : Roles.User;

        var response = await factory.CreateClientFor(TestData.NewUsername(), otherRole).GetAsync(url);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Dev_endpoint_is_forbidden_for_non_dev_users()
    {
        var client = factory.CreateClientFor(TestData.NewUsername(), Roles.User, Roles.Org, Roles.Admin);

        var response = await client.GetAsync("/dev/updateCache");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData("POST", "/organization/participatingusers")]
    [InlineData("DELETE", "/organization/participatingusers/someone")]
    [InlineData("POST", "/recipe/")]
    [InlineData("PUT", "/recipe/1")]
    [InlineData("DELETE", "/recipe/1")]
    public async Task Organization_write_endpoints_require_org_role(string method, string url)
    {
        var client = factory.CreateClientFor(TestData.NewUsername(), Roles.User);
        var request = new HttpRequestMessage(new HttpMethod(method), url);
        if (method != "DELETE")
        {
            request.Content = JsonContent.Create(new { username = "x", id = 0, name = "x", uexItemIds = new[] { 1 }, itemAmounts = new[] { 1 } });
        }

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Org_role_alone_does_not_grant_user_endpoints()
    {
        var client = factory.CreateClientFor(TestData.NewUsername(), Roles.Org);

        var response = await client.GetAsync("/organization/inventory");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Token_signed_with_unknown_key_is_rejected()
    {
        var foreignKey = new RsaSecurityKey(RSA.Create(2048)) { KeyId = "test-key" };
        var token = TestJwt.CreateToken(TestData.NewUsername(), [Roles.User], foreignKey, DateTime.UtcNow.AddMinutes(5));

        var response = await SendWithToken(token, "/personal/items");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Expired_token_is_rejected()
    {
        var token = TestJwt.CreateToken(TestData.NewUsername(), [Roles.User], TestJwt.SigningKey, DateTime.UtcNow.AddSeconds(-1));

        var response = await SendWithToken(token, "/personal/items");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Token_from_another_issuer_is_rejected()
    {
        var token = TestJwt.CreateToken(TestData.NewUsername(), [Roles.User], TestJwt.SigningKey, DateTime.UtcNow.AddMinutes(5), issuer: "https://evil.test/");

        var response = await SendWithToken(token, "/personal/items");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Token_without_nickname_claim_is_unauthorized_for_personal_items()
    {
        var token = TestJwt.CreateToken(null, [Roles.User], TestJwt.SigningKey, DateTime.UtcNow.AddMinutes(5));

        var response = await SendWithToken(token, "/personal/items");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Health_endpoint_is_anonymous()
    {
        var response = await factory.CreateClient().GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private Task<HttpResponseMessage> SendWithToken(string token, string url)
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client.GetAsync(url);
    }
}
