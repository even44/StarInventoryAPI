using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

class TokenProvider(IConfiguration configuration)
{


    public async Task<string> Create(User user, ItemCacheDb db)
    {
        string secretKey = configuration["Jwt:Key"]!;
        if (secretKey == null)
        {
            throw new InvalidOperationException("JWT Secret Key is not configured.");
        }
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var role = await RoleDataStore.GetRole(user.RoleId, db);
        if (role == null)
        {
            throw new InvalidOperationException("User role not found.");
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                [
                    new Claim(JwtRegisteredClaimNames.Nickname, user.Username),
                    new Claim("role", role.ClaimString)
                ]),
            Expires = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:ExpirationInMinutes")),
            SigningCredentials = credentials,
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"]
        };

        var handler = new JsonWebTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);

        return token;
    }

    
}