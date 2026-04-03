using Core.Generators.Steps.Topology;
using Core.Generators.Steps.Mutators;
using Core.Interfaces;
using Core.Models;
using Core.Diagnostics;
using DungeonGenerator.Sandbox.Data;

namespace DungeonGenerator.Sandbox;

public class Program
{
    public static void Main()
    {
        Console.WriteLine(" --- Starting Pipeline Generation ---");

        var path = Path.Combine(AppContext.BaseDirectory, "Data", "templates.json");
        var templates = TemplateMapper.LoadFromJson(path);

        Console.WriteLine($"[Debug] Total Templates Loaded: {templates.Count}");
        Console.WriteLine($"[Debug] Start Templates Found: {templates.Count(t => t.Type == RoomTypes.Start)}");
        Console.WriteLine($"[Debug] Default Templates Found: {templates.Count(t => t.Type == RoomTypes.Default)}");

        var config = new DungeonGenerationConfig
        {
            Seed = 01,
            MaxTotalRooms = 9,

            StartRoomType = RoomTypes.Start,
            ObjectiveRoomType = RoomTypes.Objective,
            StandardRoomType = RoomTypes.Default,

            MinObjectiveDistance = 5,
            MaxObjectiveDistance = 8,

            TypeLimits = new Dictionary<RoomType, RoomLimit>
            {
                { RoomTypes.Start, new RoomLimit(1, 1) },
                { RoomTypes.Objective, new RoomLimit(1, 1) },
                { RoomTypes.Key, new RoomLimit(0, 2)},
                { RoomTypes.Hazard, new RoomLimit(0, 2)},
                { RoomTypes.Puzzle, new RoomLimit(0, 2)}
            }
        };

        var generator = new Core.Generators.DungeonGenerator(
                    new IDungeonGenerationStep[]
                    {
                      //new RandomizedDfsStep(),
                      new SpanningTreeStep(),
                      new ApplyConstraintsStep()
                    }
                );

        Console.WriteLine(
            $"Generating max {config.MaxTotalRooms} rooms using: {string.Join(", ", generator.Steps.Select(s => s.GetType().Name))}"
            );

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

        var symbolLegend = new Dictionary<RoomType, char>
        {
            { RoomTypes.Start, 'S' },
            { RoomTypes.Objective, 'O' },
            { RoomTypes.Key, 'K' },
            { RoomTypes.Hazard, 'H' },
            { RoomTypes.Puzzle, '?' },
            { RoomTypes.Default, '+' }
        };

        Console.WriteLine(map.ToAsciiString(0, symbolLegend));
    }
}
