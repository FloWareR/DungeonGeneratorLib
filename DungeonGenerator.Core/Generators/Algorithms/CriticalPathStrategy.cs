using System;
using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using Core.Interfaces;
using Core.Models;
using Core.State;

namespace Core.Generators.Algorithms;

public class CriticalPathStrategy : IDungeonGenerationStrategy
{
    public DungeonMap Generate(IReadOnlyList<RoomTemplate> templates, DungeonGenerationConfig config)
    {
        var map = new DungeonMap();
        var random = new Random(config.Seed);
        var untriedExits = new Dictionary<Guid, List<Direction>>();

        var startTemplate = templates.FirstOrDefault(t => t.Type == RoomType.Start) ?? templates[0];
        var objectiveTemplate = templates.FirstOrDefault(t => t.Type == RoomType.Objective);

        var transitTemplates = templates
            .Where(t => t.Type == RoomType.Normal)
            .Where(t => t.AvailableExits.GetActiveFlags().Count() > 1)
            .ToList();

        var currentRoom = new Room(Position.Zero, startTemplate);
        map.AddRoom(currentRoom);
        untriedExits[currentRoom.InstanceId] = currentRoom.Template.AvailableExits.GetActiveFlags().ToList();

        int minMainPath = Math.Max(3, config.MaxTotalRooms / 2);
        int currentPathLength = 1;
        bool objectivePlaced = false;

        while (!objectivePlaced && map.AllRooms.Count < config.MaxTotalRooms)
        {
            var exitsToTry = untriedExits[currentRoom.InstanceId];
            if (exitsToTry.Count == 0) break; // Trapped!

            Direction outDir = Direction.None;

            if (currentPathLength >= minMainPath && objectiveTemplate != null)
            {
                outDir = exitsToTry.FirstOrDefault(dir => (objectiveTemplate.AvailableExits & dir.GetOpposite()) != 0);
            }

            if (outDir == Direction.None)
            {
                outDir = exitsToTry[random.Next(exitsToTry.Count)];
            }

            exitsToTry.Remove(outDir);

            var targetPos = currentRoom.GridPosition.Move(outDir);
            if (map.TryGetRoomAt(targetPos, out _)) continue;

            var requiredInDir = outDir.GetOpposite();

            if (currentPathLength >= minMainPath && objectiveTemplate != null && (objectiveTemplate.AvailableExits & requiredInDir) != 0)
            {
                var objRoom = new Room(targetPos, objectiveTemplate);
                currentRoom.Connect(outDir, objRoom);
                objRoom.Connect(requiredInDir, currentRoom);
                map.AddRoom(objRoom);
                objectivePlaced = true;
                break; // Critical path is secured!
            }

            var validTransits = transitTemplates.Where(t => (t.AvailableExits & requiredInDir) != 0).ToList();
            if (validTransits.Count == 0) break;

            var nextTemplate = validTransits[random.Next(validTransits.Count)];
            var nextRoom = new Room(targetPos, nextTemplate);

            currentRoom.Connect(outDir, nextRoom);
            nextRoom.Connect(requiredInDir, currentRoom);
            map.AddRoom(nextRoom);

            var newExits = nextTemplate.AvailableExits.GetActiveFlags().ToList();
            newExits.Remove(requiredInDir);
            untriedExits[nextRoom.InstanceId] = newExits;

            currentRoom = nextRoom;
            currentPathLength++;
        }

        var activeRooms = map.AllRooms.ToList();

        while (activeRooms.Count > 0 && map.AllRooms.Count < config.MaxTotalRooms)
        {
            int roomIndex = random.Next(activeRooms.Count);
            var room = activeRooms[roomIndex];

            if (!untriedExits.TryGetValue(room.InstanceId, out var exits) || exits.Count == 0)
            {
                activeRooms.RemoveAt(roomIndex);
                continue;
            }

            var dir = exits[^1];
            exits.RemoveAt(exits.Count - 1);

            var targetPos = room.GridPosition.Move(dir);
            if (map.TryGetRoomAt(targetPos, out _)) continue;

            var reqInDir = dir.GetOpposite();

            var validTemplates = templates
                .Where(t => (t.AvailableExits & reqInDir) != 0)
                .Where(t => CanSpawnType(t.Type, map, config))
                .ToList();

            if (validTemplates.Count == 0) continue;

            validTemplates = ApplyPriorityOverride(validTemplates, map, config);

            var chosenTemplate = validTemplates[random.Next(validTemplates.Count)];
            var newRoom = new Room(targetPos, chosenTemplate);

            room.Connect(dir, newRoom);
            newRoom.Connect(reqInDir, room);
            map.AddRoom(newRoom);

            activeRooms.Add(newRoom);

            var newRoomExits = chosenTemplate.AvailableExits.GetActiveFlags().ToList();
            newRoomExits.Remove(reqInDir);

            for (int i = newRoomExits.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (newRoomExits[i], newRoomExits[j]) = (newRoomExits[j], newRoomExits[i]);
            }

            untriedExits[newRoom.InstanceId] = newRoomExits;
        }

        return map;
    }

    // --- HELPER METHODS ---

    private List<RoomTemplate> ApplyPriorityOverride(List<RoomTemplate> validTemplates, DungeonMap map, DungeonGenerationConfig config)
    {
        var deficientTypes = config.TypeLimits
            .Where(kvp => map.AllRooms.Count(r => r.Template.Type == kvp.Key) < kvp.Value.Min)
            .Select(kvp => kvp.Key)
            .ToList();

        if (deficientTypes.Count > 0)
        {
            var priorityTemplates = validTemplates.Where(t => deficientTypes.Contains(t.Type)).ToList();
            if (priorityTemplates.Count > 0)
            {
                return priorityTemplates;
            }
        }

        return validTemplates;
    }

    private bool CanSpawnType(RoomType type, DungeonMap map, DungeonGenerationConfig config)
    {
        if (!config.TypeLimits.TryGetValue(type, out var limit)) return true;
        return map.AllRooms.Count(r => r.Template.Type == type) < limit.Max;
    }
}
