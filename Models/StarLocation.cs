using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public enum LocationSourceType
{
    Poi = 1,
    SpaceStation = 2,
    City = 3,
}

public class StarLocation
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    public string Name { get; set; }

    // The UEX entity this location was compiled from. Together they form the
    // immutable identity of the location, so Id never moves between cache syncs.
    // Null only for rows created before stable mapping existed and not yet adopted.
    [JsonIgnore]
    public LocationSourceType? SourceType { get; set; }
    [JsonIgnore]
    public int? SourceId { get; set; }



    public StarLocation()
    {
        Name = "Test Lokasjon";
    }
}
