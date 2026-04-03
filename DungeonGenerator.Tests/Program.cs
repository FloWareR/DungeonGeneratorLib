using Core.Generators.Algorithms;
using Core.Models;
using Core.Diagnostics;

namespace DungeonGenerator.Tests;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("--- Starting Metadata & Config Test ---");

        var templates = new List<RoomTemplate>
        {
            new RoomTemplate { Id = "LoadingBay", Type = RoomType.Start, AvailableExits = Direction.North, Tags = new[] { "SafeZone", "Tutorial" } },
            new RoomTemplate { Id = "LoadingBayAlt", Type = RoomType.Start, AvailableExits = Direction.East | Direction.South | Direction.North, Tags = new[] { "SafeZone" } },
            new RoomTemplate { Id = "InsertionPoint", Type = RoomType.Start, AvailableExits = Direction.South, Tags = new[] { "Stealth", "Entry" } },

            new RoomTemplate { Id = "DropOffZone", Type = RoomType.Objective, AvailableExits = Direction.South | Direction.East,  Tags = new[] { "HighSecurity" } },
            new RoomTemplate { Id = "ExtractionZone", Type = RoomType.Objective, AvailableExits = Direction.North, Tags = new[] { "Escape" } },
            new RoomTemplate { Id = "CoreReactor", Type = RoomType.Objective, AvailableExits = Direction.West, Tags = new[] { "Critical", "Destroy" } },

            new RoomTemplate { Id = "SecurityCheckpoint", Type = RoomType.Key, AvailableExits = Direction.East | Direction.West, Tags = new[] { "Locked", "LaserTraps" } },

            new RoomTemplate { Id = "MainHub", Type = RoomType.Normal, AvailableExits = Direction.North | Direction.South | Direction.East | Direction.West },
            new RoomTemplate { Id = "MainHubLarge", Type = RoomType.Normal, AvailableExits = Direction.North | Direction.South | Direction.East | Direction.West, Tags = new[] { "Large" } },
            new RoomTemplate { Id = "MainHubBroken", Type = RoomType.Normal, AvailableExits = Direction.North | Direction.South | Direction.East, Tags = new[] { "Damaged" } },

            new RoomTemplate { Id = "HallwayNS", Type = RoomType.Normal, AvailableExits = Direction.North | Direction.South },
            new RoomTemplate { Id = "HallwayEW", Type = RoomType.Normal, AvailableExits = Direction.East | Direction.West },

            new RoomTemplate { Id = "CornerNE", Type = RoomType.Normal, AvailableExits = Direction.North | Direction.East },
            new RoomTemplate { Id = "CornerNW", Type = RoomType.Normal, AvailableExits = Direction.North | Direction.West },
            new RoomTemplate { Id = "CornerSE", Type = RoomType.Normal, AvailableExits = Direction.South | Direction.East },
            new RoomTemplate { Id = "CornerSW", Type = RoomType.Normal, AvailableExits = Direction.South | Direction.West },

            new RoomTemplate { Id = "TJunctionN", Type = RoomType.Normal, AvailableExits = Direction.North | Direction.East | Direction.West },
            new RoomTemplate { Id = "TJunctionS", Type = RoomType.Normal, AvailableExits = Direction.South | Direction.East | Direction.West },
            new RoomTemplate { Id = "TJunctionE", Type = RoomType.Normal, AvailableExits = Direction.North | Direction.South | Direction.East },
            new RoomTemplate { Id = "TJunctionW", Type = RoomType.Normal, AvailableExits = Direction.North | Direction.South | Direction.West },

            new RoomTemplate { Id = "Crossroad", Type = RoomType.Normal, AvailableExits = Direction.North | Direction.South | Direction.East | Direction.West },

            new RoomTemplate { Id = "DeadEndN", Type = RoomType.Normal, AvailableExits = Direction.North },
            new RoomTemplate { Id = "DeadEndS", Type = RoomType.Normal, AvailableExits = Direction.South },
            new RoomTemplate { Id = "DeadEndE", Type = RoomType.Normal, AvailableExits = Direction.East },
            new RoomTemplate { Id = "DeadEndW", Type = RoomType.Normal, AvailableExits = Direction.West },

            new RoomTemplate { Id = "StorageCloset", Type = RoomType.Normal, AvailableExits = Direction.West, Tags = new[] { "Loot" } },

            new RoomTemplate { Id = "LaserHall", Type = RoomType.Hazard, AvailableExits = Direction.North | Direction.South, Tags = new[] { "Laser", "Damage" } },
            new RoomTemplate { Id = "SpikeTrapRoom", Type = RoomType.Hazard, AvailableExits = Direction.East | Direction.West, Tags = new[] { "Spikes", "Timing" } },
            new RoomTemplate { Id = "GasChamber", Type = RoomType.Hazard, AvailableExits = Direction.North | Direction.East, Tags = new[] { "Poison" } },

            new RoomTemplate { Id = "SwitchPuzzle", Type = RoomType.Puzzle, AvailableExits = Direction.North | Direction.South, Tags = new[] { "Switch" } },
            new RoomTemplate { Id = "PressurePlatePuzzle", Type = RoomType.Puzzle, AvailableExits = Direction.East | Direction.West, Tags = new[] { "Weight" } },
            new RoomTemplate { Id = "CodeLockRoom", Type = RoomType.Puzzle, AvailableExits = Direction.North, Tags = new[] { "Code" } }
        };

        var config = new DungeonGenerationConfig
        {
            Seed = 09983,
            MaxTotalRooms = 9,
            TypeLimits = new Dictionary<RoomType, RoomLimit>
            {
                { RoomType.Start, new RoomLimit(1, 1) },
                { RoomType.Objective, new RoomLimit(1, 1) },
                { RoomType.Key, new RoomLimit(1, 3) },
                { RoomType.Puzzle, new RoomLimit(2, 3)},
                { RoomType.Hazard, new RoomLimit(1, 3)}
            }
        };

        var strategy = new CriticalPathStrategy();
        var generator = new Core.Generators.DungeonGenerator(strategy);

        Console.WriteLine($"Generating max {config.MaxTotalRooms} rooms... using {strategy.GetType()}");
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

        Console.WriteLine(map.ToAsciiString());
    }
}
