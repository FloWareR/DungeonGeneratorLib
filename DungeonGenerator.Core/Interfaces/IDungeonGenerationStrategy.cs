using System.Collections.Generic;
using Core.Models;
using Core.State;

namespace Core.Interfaces;

///<summary>Defines a generation algorithm</summary>
public interface IDungeonGenerationStrategy
{
    DungeonMap Generate(IReadOnlyList<RoomTemplate> templates, DungeonGenerationConfig config);
}
