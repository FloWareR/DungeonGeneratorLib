using System;
using System.Collections.Generic;

namespace Core.Models;

public class Room
{
    public Guid InstanceId { get; } = Guid.NewGuid();
    public Position GridPosition { get; }
    public RoomTemplate Template { get; private set; }

    public Dictionary<Direction, Guid> Connections { get; } = new();

    public Room(Position gridPosition, RoomTemplate template)
    {
        GridPosition = gridPosition;
        Template = template;
    }


    public void Connect(Direction direction, Room otherRoom)
    {
        Connections[direction] = otherRoom.InstanceId;
    }

    public void SetTemplate(RoomTemplate template)
    {
        Template = template;
    }
}
