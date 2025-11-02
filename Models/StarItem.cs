

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
        string[] nameList = ["Picoball", "Fishtank", "Stefan", "P8-AR", "P4-AR", "Hydroponic Farm", "Quantum Drive", "Cargo Rack", "MedPen", "Nano-Forge", "Shield Generator", "Fuel Scoop", "Afterburner", "Auto-Loader", "Cooler", "Power Plant", "Thruster", "Warp Core", "Quantum Core", "Hull Reinforcement", "Armor Plating", "EMP Launcher", "Missile Rack", "Laser Cannon", "Railgun", "Gauss Cannon", "Plasma Accelerator", "Point Defense", "Mining Laser", "Salvage Arm", "Tractor Beam", "Refinery", "Jump Drive"];
        string randName = random.GetItems<string>(nameList, 1)[0];
        int randomLocation = 67;
        int randQuantity = (int)(random.NextSingle() * 100.0);
        bool randIsShared = random.GetItems<bool>([true, false], 1)[0];


        return new StarItem(randId, randName, 0, randomLocation, randQuantity, randIsShared);

    }

}
