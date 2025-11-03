using System.ComponentModel.DataAnnotations.Schema;

public class StarLocation
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    public string Name { get; set; }



    public StarLocation()
    {
        Name = "Test Lokasjon";
    }
}
