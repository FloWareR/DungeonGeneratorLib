using Core.Interfaces;
using Core.State;

namespace Core.Rendering;

public class MapRenderer
{
    private readonly IDungeonRenderer _renderer;

    public MapRenderer(IDungeonRenderer renderer)
    {
        _renderer = renderer;
    }

    public void Render(DungeonMap map)
    {
        _renderer.BeingRendering(map);

        foreach (var room in map.AllRooms)
        {
            _renderer.RenderRoom(room);
        }

        foreach (var room in map.AllRooms)
        {
            foreach (var connection in room.Connections)
            {
                if (room.InstanceId.CompareTo(connection.Value) < 0)
                {
                    if (map.TryGetRoomAt(room.GridPosition.Move(connection.Key), out var targetRoom))
                    {
                        _renderer.RenderConnection(room, targetRoom, connection.Key);
                    }
                }
            }
        }

        _renderer.EndRendering();
    }
}
