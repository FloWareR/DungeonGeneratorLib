using System;
using System.Linq;
using System.Collections.Generic;
using Core.Interfaces;
using Core.State;
using Core.Models;
using Core.Extensions;

namespace Core.Generators.Algorithms;

///<summary>Standard DFS exploration, does not work well with "contrains" if you 
/// need Start / Objective guarantees, use "CriticalPathStrategy" instead
///</summary>
public class RandomizedDfsStrategy : IDungeonGenerationStrategy
{
    public DungeonMap Generate(IReadOnlyList<RoomTemplate> templates, DungeonGenerationConfig config)
    {
        var map = new DungeonMap();
        var random = new Random(config.Seed);

        // Track untried exits for each room to handle backtracking
        var untriedExits = new Dictionary<Guid, List<Direction>>();
        var stack = new Stack<Room>();

        // Intialize with a random starting room
        var startTemplates = templates
          .Where(t => t.Type == config.StartRoomType).ToList();
        var startTemplate = startTemplates.Count > 0
          ? startTemplates[random.Next(startTemplates.Count)]
          : templates[random.Next(templates.Count)];
        var startRoom = new Room(Position.Zero, startTemplate);

        map.AddRoom(startRoom);
        stack.Push(startRoom);
        untriedExits[startRoom.InstanceId] = ShuffleExits(startTemplate.AvailableExits.GetActiveFlags().ToList(), random);

        while (stack.Count > 0 && map.AllRooms.Count < config.MaxTotalRooms)
        {
            var currentRoom = stack.Peek();
            var exitsToTry = untriedExits[currentRoom.InstanceId];

            // If we hit this => Dead end or All paths explore, we backtrack
            if (exitsToTry.Count == 0)
            {
                stack.Pop();
                continue;
            }

            var directionToTry = exitsToTry[^1];
            exitsToTry.RemoveAt(exitsToTry.Count - 1);

            var targetPosition = currentRoom.GridPosition.Move(directionToTry);

            // If it exists => is occupied => try next exit
            if (map.TryGetRoomAt(targetPosition, out _))
            {
                continue;
            }

            var requireIncomingDoor = directionToTry.GetOpposite();

            var validTemplates = templates
              .Where(t => (t.AvailableExits & requireIncomingDoor) != 0)
              .Where(t => CanSpawnType(t.Type, map, config))
              .ToList();

            // No template fits => try next
            if (validTemplates.Count == 0)
            {
                continue;
            }

            var chosenTemplate = validTemplates[random.Next(validTemplates.Count)];
            var newRoom = new Room(targetPosition, chosenTemplate);

            // Connect the doors in our graph
            currentRoom.Connect(directionToTry, newRoom);
            newRoom.Connect(requireIncomingDoor, currentRoom);

            map.AddRoom(newRoom);
            stack.Push(newRoom);

            var newRoomExits = chosenTemplate.AvailableExits.GetActiveFlags().ToList();
            newRoomExits.Remove(requireIncomingDoor);
            untriedExits[newRoom.InstanceId] = ShuffleExits(newRoomExits, random);
        }

        return map;
    }

    private bool CanSpawnType(string type, DungeonMap map, DungeonGenerationConfig config)
    {
        if (!config.TypeLimits.TryGetValue(type, out var limit))
        {
            return true;
        }

        var currentCount = map.AllRooms.Count(t => t.Template.Type == type);
        return currentCount < limit.Max;
    }

    // Helper method to randomize the order we try doors 
    private List<Direction> ShuffleExits(List<Direction> exits, Random random)
    {
        for (int i = exits.Count - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (exits[i], exits[j]) = (exits[j], exits[i]);
        }
        return exits;
    }
}
