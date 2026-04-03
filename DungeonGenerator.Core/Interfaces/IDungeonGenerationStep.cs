using Core.State;

namespace Core.Interfaces;

public interface IDungeonGenerationStep
{
    void Execute(DungeonGenerationContext context);
}
