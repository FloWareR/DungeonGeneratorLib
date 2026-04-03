using System.Collections.Generic;

namespace Core.Models;

public class RoomTemplate
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;

    public RoomType Type { get; init; } = RoomType.Normal;
    public IReadOnlyList<string> Tags { get; init; } = new List<string>();
    public Direction AvailableExits { get; init; } = Direction.None;
    public int SpawnWeight { get; init; } = 1;
}
