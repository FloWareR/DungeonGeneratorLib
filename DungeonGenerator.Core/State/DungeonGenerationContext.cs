using System;
using System.Collections.Generic;
using Core.Models;

namespace Core.State;

public class DungeonGenerationContext
{
    public DungeonMap Map { get; } = new();
    public DungeonGenerationConfig Config = new();
    public IReadOnlyList<RoomTemplate> Templates { get; }
    public Random Rng { get; }

    public DungeonGenerationContext(DungeonGenerationConfig config, IReadOnlyList<RoomTemplate> templates)
    {
        Config = config;
        Templates = templates;
        Rng = new Random(config.Seed);
    }
}
