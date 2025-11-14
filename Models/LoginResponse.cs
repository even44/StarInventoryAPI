public class LoginResponse
{
    public string Token { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }

    public LoginResponse(string token, string username, string role)
    {
        Token = token;
        Username = username;
        Role = role;
    }
}