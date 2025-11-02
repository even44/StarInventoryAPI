

public class StarItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int UexIdentifier { get; set; }
    public int LocationId { get; set; }
    public int Quantity { get; set; }
    public bool IsSharedWithOrganization { get; set; }


    public StarItem()
    {
    }

    public StarItem(int id, string name, int uexIdentifier, int location, int quantity, bool isSharedWithOrganization)
    {
        Id = id;
        Name = name;
        UexIdentifier = uexIdentifier;
        LocationId = location;
        Quantity = quantity;
        IsSharedWithOrganization = isSharedWithOrganization;
    }

    public static StarItem RandomItem()
    {
        Random random = new Random();

        int randId = 0;
        string[] nameList = ["Picoball", "Fishtank", "Stefan", "P8-AR", "P4-AR"];
        string randName = random.GetItems<string>(nameList, 1)[0];
        int randomLocation = 0;
        int randQuantity = (int)(random.NextSingle() * 100.0);
        bool randIsShared = random.GetItems<bool>([true, false], 1)[0];


        return new StarItem(randId, randName, 0, randomLocation, randQuantity, randIsShared);

    }

}
