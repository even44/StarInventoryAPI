using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(Username))]
public class User
{

    public string Username { get; set; }
    [JsonIgnore]
    public string PasswordHash { get; set; }
    public string Role { get; set; }

}