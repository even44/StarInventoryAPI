using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(Username))]
public class User
{
    [Required(ErrorMessage = "Username must not be empty.")]
    public required string Username { get; set; }
}