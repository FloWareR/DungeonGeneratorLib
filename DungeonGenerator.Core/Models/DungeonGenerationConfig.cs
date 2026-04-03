using System;
using System.Collections.Generic;

namespace Core.Models;

public readonly record struct RoomLimit(int Min, int Max);

public class DungeonGenerationConfig
{
    public int Seed { get; init; } = Environment.TickCount;
    public int MaxTotalRooms { get; init; } = 10;

    public Dictionary<RoomType, RoomLimit> TypeLimits { get; init; } = new();
}
