using System;
using System.Collections.Generic;

namespace Core.Models;

public readonly record struct RoomLimit(int Min, int Max);

public class DungeonGenerationConfig
{
    public int Seed { get; init; } = Environment.TickCount;
    public int MaxTotalRooms { get; init; } = 10;

    public int MinObjectiveDistance { get; init; } = 3;
    public int MaxObjectiveDistance { get; init; } = 10;

    // The algorithms will look for these specific strings to build the main path
    public string StartRoomType { get; init; } = "Start";
    public string ObjectiveRoomType { get; init; } = "Objective";
    public string StandardRoomType { get; init; } = "Standard";

    public Dictionary<string, RoomLimit> TypeLimits { get; init; } = new();
}
