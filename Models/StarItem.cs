

public class StarItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int UexIdentifier { get; set; }
    public StarLocation Location { get; set; }
    public int Quantity { get; set; }
    public bool IsSharedWithOrganization { get; set; }


    public StarItem()
    {
        Id = 0;
        Name = "Test item";
        UexIdentifier = 0;
        Location = new StarLocation();
        Quantity = 5;
        IsSharedWithOrganization = false;
    }

    public static StarItem RandomItem()
    {
        Random random = new Random();



        return new StarItem();

    }

}
