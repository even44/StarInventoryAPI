using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(Id))]
public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ClaimString { get; set; }

    public Role() { }
}

