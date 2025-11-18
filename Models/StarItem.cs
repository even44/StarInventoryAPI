

public class StarItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int UexIdentifier { get; set; }
    public int LocationId { get; set; }
    public string Username { get; set; }
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

    public static StarItem RandomItem(ItemCacheDb db, string username)
    {
        Random random = new Random();


        db.StarLocations.Count();

        int randId = 0;
        string[] nameList = ["Picoball", "Fishtank", "Stefan", "P8-AR", "P4-AR", "Hydroponic Farm", "Quantum Drive", "Cargo Rack", "MedPen", "Nano-Forge", "Shield Generator", "Fuel Scoop", "Afterburner", "Auto-Loader", "Cooler", "Power Plant", "Thruster", "Warp Core", "Quantum Core", "Hull Reinforcement", "Armor Plating", "EMP Launcher", "Missile Rack", "Laser Cannon", "Railgun", "Gauss Cannon", "Plasma Accelerator", "Point Defense", "Mining Laser", "Salvage Arm", "Tractor Beam", "Refinery", "Jump Drive"];
        string randName = random.GetItems<string>(nameList, 1)[0];
        int locationId = random.Next(0, db.StarLocations.Count() - 1);
        string Username = username;
        int randQuantity = (int)(random.NextSingle() * 100.0);
        bool randIsShared = random.GetItems<bool>([true, false], 1)[0];


        return new StarItem(randId, randName, 0, locationId, Username, randQuantity, randIsShared);

    }

}
