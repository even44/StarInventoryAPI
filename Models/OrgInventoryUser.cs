using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(Username))]
public class OrgInventoryUser
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required string Username { get; set; }
}