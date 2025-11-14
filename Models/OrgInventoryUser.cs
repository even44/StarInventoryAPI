using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(Username))]
public class OrgInventoryUser
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Username { get; set; }
}