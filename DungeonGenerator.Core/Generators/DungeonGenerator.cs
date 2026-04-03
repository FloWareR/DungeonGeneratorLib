using System.Collections.Generic;
using Core.State;
using Core.Models;
using Core.Interfaces;

namespace Core.Generators;

public class DungeonGenerator
{
    private readonly IEnumerable<IDungeonGenerationStep> _pipeline;

    public DungeonGenerator(IDungeonGenerationStep[] pipeline)
    {
        _pipeline = pipeline;
    }

    public DungeonMap Generate(IReadOnlyList<RoomTemplate> templates, DungeonGenerationConfig config)
    {
        if (templates.Count == 0 || config.MaxTotalRooms <= 0)
        {
            return new DungeonMap();
        }

        var context = new DungeonGenerationContext(config, templates);

        foreach (var step in _pipeline)
        {
            step.Execute(context);
        }

        return context.Map;
    }
}
