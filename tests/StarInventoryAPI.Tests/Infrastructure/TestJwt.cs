using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace StarInventoryAPI.Tests.Infrastructure;

/// <summary>
/// Issues JWTs signed with a throwaway RSA key, standing in for the real identity provider's JWKS.
/// </summary>
public static class TestJwt
{
    public const string Issuer = "https://issuer.test/application/o/star-inventory/";
    public const string Audience = "star-inventory-tests";

    public static readonly RsaSecurityKey SigningKey = new(RSA.Create(2048)) { KeyId = "test-key" };

    public static string CreateToken(string username, params string[] roles) =>
        CreateToken(username, roles, signingKey: SigningKey, expires: DateTime.UtcNow.AddMinutes(5));

    public static string CreateToken(string? username, string[] roles, SecurityKey signingKey, DateTime expires, string issuer = Issuer)
    {
        var claims = new Dictionary<string, object>();
        if (username != null)
        {
            claims["nickname"] = username;
        }
        if (roles.Length > 0)
        {
            claims["roles"] = roles;
        }

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = Audience,
            Subject = new ClaimsIdentity(),
            Claims = claims,
            NotBefore = expires.AddMinutes(-10),
            IssuedAt = expires.AddMinutes(-10),
            Expires = expires,
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.RsaSha256),
        };
        return new JsonWebTokenHandler().CreateToken(descriptor);
    }
}
