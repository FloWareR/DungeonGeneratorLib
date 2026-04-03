using Core.State;
using Core.Models;

namespace Core.Interfaces;

public interface IDungeonRenderer
{
    void BeingRendering(DungeonMap map);
    void RenderRoom(Room room);
    void RenderConnection(Room from, Room to, Direction direction);
    void EndRendering();
}
