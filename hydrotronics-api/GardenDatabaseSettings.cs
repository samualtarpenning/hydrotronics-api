namespace GardenApi.Models;

public class GardenDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string GardenCollectionName { get; set; } = null!;
}