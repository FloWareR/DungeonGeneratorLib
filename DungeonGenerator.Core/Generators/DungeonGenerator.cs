using System.Collections.Generic;
using Core.State;
using Core.Models;
using Core.Interfaces;

namespace Core.Generators;

public class DungeonGenerator
{
    private readonly IDungeonGenerationStrategy _strategy;

    public DungeonGenerator(IDungeonGenerationStrategy strategy)
    {
        _strategy = strategy;
    }

    public DungeonMap Generate(IReadOnlyList<RoomTemplate> templates, DungeonGenerationConfig config)
    {
        if (templates.Count == 0 || config.MaxTotalRooms <= 0)
        {
            return new DungeonMap();
        }

        return _strategy.Generate(templates, config);
    }
}
