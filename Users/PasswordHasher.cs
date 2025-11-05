

using Microsoft.AspNetCore.Identity;

public class PasswordHasher
{
    public string HashPassword(string password, User user)
    {
        // Simple hash for demonstration purposes. Use a secure hashing algorithm in production.
        var passwordHasher = new PasswordHasher<User>();

        string hashedPassword = passwordHasher.HashPassword(user, password);

        return hashedPassword;
    }

    public bool VerifyPassword(string password, string hashedPassword, User user)
    {
        var passwordHasher = new PasswordHasher<User>();

        var verificationResult = passwordHasher.VerifyHashedPassword(user, hashedPassword, password);

        if (verificationResult == PasswordVerificationResult.Success)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}