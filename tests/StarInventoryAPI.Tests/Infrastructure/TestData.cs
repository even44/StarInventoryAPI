namespace StarInventoryAPI.Tests.Infrastructure;

public static class TestData
{
    /// <summary>A unique username so tests sharing a database don't see each other's data.</summary>
    public static string NewUsername() => $"pilot-{Guid.NewGuid():N}"[..20];

    /// <summary>A personal item request body. <c>username</c> is required by the model but ignored by the API.</summary>
    public static object NewItem(string name, int locationId, int quantity = 1, int uexIdentifier = 100, bool shared = false) => new
    {
        id = 0,
        name,
        uexIdentifier,
        locationId,
        username = "ignored-by-api",
        quantity,
        isSharedWithOrganization = shared,
    };
}
