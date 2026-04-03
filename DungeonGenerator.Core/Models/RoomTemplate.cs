using System.Collections.Generic;

namespace Core.Models;

public class RoomTemplate
{
    // Identity
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;

    //Metadata
    public RoomType Type { get; init; } = new("Normal");
    public IReadOnlyList<RoomTag> Tags { get; init; } = new List<RoomTag>();
    public Direction AvailableExits { get; init; } = Direction.None;
    public int SpawnWeight { get; init; } = 1;
}
