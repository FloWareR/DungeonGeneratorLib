using Core.Models;

namespace DungeonGenerator.Sandbox.Data;

public static class RoomTypes
{
    public static readonly RoomType Start = new("Start");
    public static readonly RoomType Objective = new("Objective");
    public static readonly RoomType Default = new("Default");
    public static readonly RoomType Key = new("Key");
    public static readonly RoomType Hazard = new("Hazard");
    public static readonly RoomType Puzzle = new("Puzzle");
}

