using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using Core.Models;

namespace Core.State;

public class DungeonMap
{
    private readonly Dictionary<Position, Room> _rooms = new();

    public IReadOnlyCollection<Room> AllRooms => _rooms.Values;

    ///<summary>Returns the first room matching a specific type</summary>
    public Room? GetRoomByType(RoomType type)
    {
        return _rooms.Values.FirstOrDefault(t => t.Template.Type == type);
    }

    ///<summary>Returns all rooms containing a specific tag</summary>
    public IEnumerable<Room> GetRoomsByTag(RoomTag tag)
    {
        return _rooms.Values.Where(t => t.Template.Tags.Contains(tag));
    }

    ///<summary>Returns a room at a specific position (Can be null)</summary>
    public bool TryGetRoomAt(Position position, [NotNullWhen(true)] out Room? room)
    {
        return _rooms.TryGetValue(position, out room);
    }

    ///<summary>Adds an edge to a node in the graph</summary>
    public bool AddRoom(Room room)
    {
        if (_rooms.ContainsKey(room.GridPosition))
        {
            return false;
        }
        _rooms.Add(room.GridPosition, room);
        return true;
    }
}
