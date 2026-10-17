using System.ComponentModel.DataAnnotations;
public class StarItem
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Name must not be empty.")]
    public required string Name { get; set; }
    public int UexIdentifier { get; set; }
    public int LocationId { get; set; }
    public required string Username { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
    public int Quantity { get; set; }
    public bool IsSharedWithOrganization { get; set; }


    public StarItem()
    {
    }

    public StarItem(int id, string name, int uexIdentifier, int locationId, string username, int quantity, bool isSharedWithOrganization)
    {
        Id = id;
        Name = name;
        UexIdentifier = uexIdentifier;
        LocationId = locationId;
        Username = username;
        Quantity = quantity;
        IsSharedWithOrganization = isSharedWithOrganization;
    }

}
