using System.Collections.Generic;
using System.Linq;
using Core.Interfaces;
using Core.Models;
using Core.State;
using Core.Extensions;

namespace Core.Generators.Steps.Topology;

public class SpanningTreeStep : IDungeonGenerationStep
{
    public void Execute(DungeonGenerationContext context)
    {
        if (context.Config.MaxTotalRooms <= 0 || context.Templates.Count == 0) return;

        var startTemplates = context.Templates.Where(t => t.Type == context.Config.StartRoomType).ToList();
        var normalTemplates = context.Templates.Where(t => t.Type == context.Config.StandardRoomType).ToList();

        if (startTemplates.Count == 0 || normalTemplates.Count == 0) return;

        var startRoom = new Room(Position.Zero, startTemplates.GetRandomWeighted(context.Rng));
        context.Map.AddRoom(startRoom);

        // Instead of a Stack, we keep a pool of all rooms that still have space around them
        var activeRooms = new List<Room> { startRoom };

        while (activeRooms.Count > 0 && context.Map.AllRooms.Count < context.Config.MaxTotalRooms)
        {
            // Pick a RANDOM existing room to branch off from, rather than just the newest one
            var currentRoom = activeRooms[context.Rng.Next(activeRooms.Count)];
            var unvisitedNeighbors = GetUnvisitedNeighbors(currentRoom, context);

            if (unvisitedNeighbors.Count == 0)
            {
                activeRooms.Remove(currentRoom);
                continue;
            }

            var nextMove = unvisitedNeighbors[context.Rng.Next(unvisitedNeighbors.Count)];
            var newRoom = new Room(nextMove.Position, normalTemplates.GetRandomWeighted(context.Rng));

            currentRoom.Connect(nextMove.Direction, newRoom);
            newRoom.Connect(nextMove.Direction.GetOpposite(), currentRoom);

            context.Map.AddRoom(newRoom);
            activeRooms.Add(newRoom);
        }
    }

    private IReadOnlyList<(Direction Direction, Position Position)> GetUnvisitedNeighbors(Room room, DungeonGenerationContext context)
    {
        var unvisited = new List<(Direction, Position)>();
        var standardDirections = new[] { Direction.North, Direction.South, Direction.East, Direction.West };

        foreach (var direction in standardDirections)
        {
            var targetPos = room.GridPosition.Move(direction);
            if (!context.Map.TryGetRoomAt(targetPos, out _))
            {
                unvisited.Add((direction, targetPos));
            }
        }
        return unvisited;
    }
}
