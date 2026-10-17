using System.Security.Claims;

namespace StarInventoryAPI.Extentions;

public static class ClaimsPrincipalExtentions
{
    // Claim types checked, in order, when resolving the username of the caller.
    // "nickname" stays first so existing inventories (keyed on nickname) keep resolving to the same user.
    // JwtBearer maps "sub" to ClaimTypes.NameIdentifier by default, so both are checked.
    private static readonly string[] UsernameClaimTypes =
    [
        "nickname",
        "preferred_username",
        "sub",
        ClaimTypes.NameIdentifier,
    ];

    public static string? GetUsername(this ClaimsPrincipal user)
    {
        foreach (var claimType in UsernameClaimTypes)
        {
            var value = user.FindFirst(claimType)?.Value;
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }
        return null;
    }
}
