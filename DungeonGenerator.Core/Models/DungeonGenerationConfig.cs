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

    // The algorithms will look for these specific RoomTypes to build the main path
    public RoomType StartRoomType { get; init; } = new("Start");
    public RoomType ObjectiveRoomType { get; init; } = new("Objective");
    public RoomType StandardRoomType { get; init; } = new("Normal");

    public Dictionary<RoomType, RoomLimit> TypeLimits { get; init; } = new();
}
