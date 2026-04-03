using Core.Generators.Steps.Topology;
using Core.Generators.Steps.Mutators;
using Core.Interfaces;
using Core.Models;
using Core.Diagnostics;

namespace DungeonGenerator.Sandbox;

public class Program
{
    public static void Main()
    {
        Console.WriteLine(" --- Starting Pipeline Generation ---");

        var templates = new List<RoomTemplate>
        {
            // Start Rooms
            new RoomTemplate { Id = "LoadingBay", Type = "Start", AvailableExits = Direction.North, Tags = new[] { "SafeZone", "Tutorial" }, SpawnWeight = 10 },
            new RoomTemplate { Id = "LoadingBayAlt", Type = "Start", AvailableExits = Direction.East | Direction.South | Direction.North, Tags = new[] { "SafeZone" }, SpawnWeight = 10 },
            new RoomTemplate { Id = "InsertionPoint", Type = "Start", AvailableExits = Direction.South, Tags = new[] { "Stealth", "Entry" }, SpawnWeight = 5 },

            // Objectives
            new RoomTemplate { Id = "DropOffZone", Type = "Objective", AvailableExits = Direction.South | Direction.East,  Tags = new[] { "HighSecurity" }, SpawnWeight = 10 },
            new RoomTemplate { Id = "ExtractionZone", Type = "Objective", AvailableExits = Direction.North, Tags = new[] { "Escape" }, SpawnWeight = 10 },
            new RoomTemplate { Id = "CoreReactor", Type = "Objective", AvailableExits = Direction.West, Tags = new[] { "Critical", "Destroy" }, SpawnWeight = 5 },

            // Keys
            new RoomTemplate { Id = "SecurityCheckpoint", Type = "Key", AvailableExits = Direction.East | Direction.West, Tags = new[] { "Locked", "LaserTraps" }, SpawnWeight = 10 },

            // Normal - Hubs
            new RoomTemplate { Id = "MainHub", Type = "Normal", AvailableExits = Direction.North | Direction.South | Direction.East | Direction.West, SpawnWeight = 20 },
            new RoomTemplate { Id = "MainHubLarge", Type = "Normal", AvailableExits = Direction.North | Direction.South | Direction.East | Direction.West, Tags = new[] { "Large" }, SpawnWeight = 10 },
            new RoomTemplate { Id = "MainHubBroken", Type = "Normal", AvailableExits = Direction.North | Direction.South | Direction.East, Tags = new[] { "Damaged" }, SpawnWeight = 5 },

            // Normal - Hallways
            new RoomTemplate { Id = "HallwayNS", Type = "Normal", AvailableExits = Direction.North | Direction.South, SpawnWeight = 50 },
            new RoomTemplate { Id = "HallwayEW", Type = "Normal", AvailableExits = Direction.East | Direction.West, SpawnWeight = 50 },

            // Normal - Corners 
            new RoomTemplate { Id = "CornerNE", Type = "Normal", AvailableExits = Direction.North | Direction.East, SpawnWeight = 30 },
            new RoomTemplate { Id = "CornerNW", Type = "Normal", AvailableExits = Direction.North | Direction.West, SpawnWeight = 30 },
            new RoomTemplate { Id = "CornerSE", Type = "Normal", AvailableExits = Direction.South | Direction.East, SpawnWeight = 30 },
            new RoomTemplate { Id = "CornerSW", Type = "Normal", AvailableExits = Direction.South | Direction.West, SpawnWeight = 30 },

            // Normal - T-Junctions
            new RoomTemplate { Id = "TJunctionN", Type = "Normal", AvailableExits = Direction.North | Direction.East | Direction.West, SpawnWeight = 15 },
            new RoomTemplate { Id = "TJunctionS", Type = "Normal", AvailableExits = Direction.South | Direction.East | Direction.West, SpawnWeight = 15 },
            new RoomTemplate { Id = "TJunctionE", Type = "Normal", AvailableExits = Direction.North | Direction.South | Direction.East, SpawnWeight = 15 },
            new RoomTemplate { Id = "TJunctionW", Type = "Normal", AvailableExits = Direction.North | Direction.South | Direction.West, SpawnWeight = 15 },

            // Normal - Crossroad
            new RoomTemplate { Id = "Crossroad", Type = "Normal", AvailableExits = Direction.North | Direction.South | Direction.East | Direction.West, SpawnWeight = 10 },

            // Normal - Dead Ends (Low Weight)
            new RoomTemplate { Id = "DeadEndN", Type = "Normal", AvailableExits = Direction.North, SpawnWeight = 5 },
            new RoomTemplate { Id = "DeadEndS", Type = "Normal", AvailableExits = Direction.South, SpawnWeight = 5 },
            new RoomTemplate { Id = "DeadEndE", Type = "Normal", AvailableExits = Direction.East, SpawnWeight = 5 },
            new RoomTemplate { Id = "DeadEndW", Type = "Normal", AvailableExits = Direction.West, SpawnWeight = 5 },

            // Normal - Special
            new RoomTemplate { Id = "StorageCloset", Type = "Normal", AvailableExits = Direction.West, Tags = new[] { "Loot" }, SpawnWeight = 5 },

            // Hazards
            new RoomTemplate { Id = "LaserHall", Type = "Hazard", AvailableExits = Direction.North | Direction.South, Tags = new[] { "Laser", "Damage" }, SpawnWeight = 10 },
            new RoomTemplate { Id = "SpikeTrapRoom", Type = "Hazard", AvailableExits = Direction.East | Direction.West, Tags = new[] { "Spikes", "Timing" }, SpawnWeight = 10 },
            new RoomTemplate { Id = "GasChamber", Type = "Hazard", AvailableExits = Direction.North | Direction.East, Tags = new[] { "Poison" }, SpawnWeight = 5 },

            // Puzzles
            new RoomTemplate { Id = "SwitchPuzzle", Type = "Puzzle", AvailableExits = Direction.North | Direction.South, Tags = new[] { "Switch" }, SpawnWeight = 10 },
            new RoomTemplate { Id = "PressurePlatePuzzle", Type = "Puzzle", AvailableExits = Direction.East | Direction.West, Tags = new[] { "Weight" }, SpawnWeight = 10 },
            new RoomTemplate { Id = "CodeLockRoom", Type = "Puzzle", AvailableExits = Direction.North, Tags = new[] { "Code" }, SpawnWeight = 5 }
        };

        var config = new DungeonGenerationConfig
        {
            Seed = 0,
            MaxTotalRooms = 12,

            StartRoomType = "Start",
            ObjectiveRoomType = "Objective",
            StandardRoomType = "Normal",

            MinObjectiveDistance = 7,
            MaxObjectiveDistance = 14,

            TypeLimits = new Dictionary<string, RoomLimit>
            {
                { "Start", new RoomLimit(1, 1) },
                { "Objective", new RoomLimit(1, 1) },
                { "Puzzle", new RoomLimit(0, 2)},
                { "Hazard", new RoomLimit(0, 2)},
                { "Key", new RoomLimit(0, 2)}
            }
        };

        var generator = new Core.Generators.DungeonGenerator(
                    new IDungeonGenerationStep[]
                    {
                      new RandomizedDfsStep(),
                      new ApplyConstraintsStep()
                    }
                );

        Console.WriteLine($"Generating max {config.MaxTotalRooms} rooms using RandomizedDfsStep...");
        var map = generator.Generate(templates, config);

        Console.WriteLine($"Generation Complete. Total Rooms: {map.AllRooms.Count}\n");

        foreach (var room in map.AllRooms)
        {
            string tags = room.Template.Tags.Count > 0
                ? $" [{string.Join(", ", room.Template.Tags)}]"
                : "";

            Console.WriteLine($"[{room.InstanceId.ToString()[..8]}] {room.Template.Type}: {room.Template.Id}{tags}");
            Console.WriteLine($"      Position: (X:{room.GridPosition.X}, Y:{room.GridPosition.Y}, Z:{room.GridPosition.Z})");
            Console.WriteLine(new string('-', 40));
        }

        var symbolLegend = new Dictionary<string, char>
        {
            { "Start", 'S' },
            { "Objective", 'O' }, { "Key", 'K' },
            { "Hazard", 'H' },
            { "Puzzle", '?' },
            { "Normal", '+' }
        };

        Console.WriteLine(map.ToAsciiString(0, symbolLegend));
    }
}
