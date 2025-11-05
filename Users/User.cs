using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(Username))]
public class User
{

    public string Username { get; set; }
    public string PasswordHash { get; set; }



}