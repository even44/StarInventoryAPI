public class StarLocation
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int UexIdentifier { get; set; }


    public StarLocation()
    {
        Name = "Test Lokasjon";
        UexIdentifier = 2;
    }
}
