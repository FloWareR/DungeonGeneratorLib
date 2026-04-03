using System.Collections.Generic;
using System.Linq;
using Core.Interfaces;
using Core.State;
using Core.Models;
using Core.Extensions;

namespace Core.Generators.Steps.Topology;

public class RandomizedDfsStep : IDungeonGenerationStep
{
    public void Execute(DungeonGenerationContext context)
    {
        if (context.Config.MaxTotalRooms <= 0 | context.Templates.Count == 0)
        {
            return;
        }
        var startTemplates = context.Templates.Where(t => t.Type == context.Config.StartRoomType).ToList();
        var normalTemplates = context.Templates.Where(t => t.Type == context.Config.StandardRoomType).ToList();
        if (startTemplates.Count == 0 || normalTemplates.Count == 0)
        {
            return;
        }
        var startTemplate = startTemplates.GetRandomWeighted(context.Rng);
        var startRoom = new Room(Position.Zero, startTemplate);

        context.Map.AddRoom(startRoom);

        var stack = new Stack<Room>();
        stack.Push(startRoom);

        while (stack.Count > 0 && context.Map.AllRooms.Count < context.Config.MaxTotalRooms)
        {
            var currentRoom = stack.Peek();
            var unvisitedNeighbors = GetUnvisitedNeighbors(currentRoom, context);

            if (unvisitedNeighbors.Count == 0)
            {
                stack.Pop();
                continue;
            }

            var nextMove = unvisitedNeighbors[context.Rng.Next(unvisitedNeighbors.Count)];
            var selectedTemplate = normalTemplates.GetRandomWeighted(context.Rng);
            var newRoom = new Room(nextMove.Position, selectedTemplate);

            currentRoom.Connect(nextMove.Direction, newRoom);
            newRoom.Connect(nextMove.Direction.GetOpposite(), currentRoom);

            context.Map.AddRoom(newRoom);
            stack.Push(newRoom);
        }
    }

    private IReadOnlyList<(Direction Direction, Position Position)> GetUnvisitedNeighbors(Room room, DungeonGenerationContext context)
    {
        var unvisited = new List<(Direction, Position)>();
        var standardDirections = new[] { Direction.North, Direction.South, Direction.East, Direction.West };

        foreach (var direction in standardDirections)
        {
            var targetPosition = room.GridPosition.Move(direction);

            if (!context.Map.TryGetRoomAt(targetPosition, out _))
            {
                unvisited.Add((direction, targetPosition));
            }
        }

        return unvisited;
    }
}


