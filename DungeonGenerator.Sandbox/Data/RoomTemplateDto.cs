namespace DungeonGenerator.Sandbox.Data;

public class RoomTemplateDto
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = "Normal";
    public string AvailableExits { get; set; } = "None";
    public List<string> Tags { get; set; } = new();
    public int SpawnWeight { get; set; } = 1;
}
